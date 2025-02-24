using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Tournaments;
using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.Tournament
{
    public class TournamentService : ITournamentService
    {

        private readonly ITournamentDao _tournamentDao;

        private readonly ICardDao _cardDao;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public TournamentService(ITournamentDao tournamentDao, ICardDao cardDao, IHttpContextAccessor httpContextAccessor)
        {
            _tournamentDao = tournamentDao;
            _cardDao = cardDao;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> InsertTournamentDecksAsync(TournamentDecksDto tournamentDecksDto)
        {
            if (tournamentDecksDto == null)
                throw new ArgumentException("Invalid tournament data.");

            var idOwner = GetUserIdFromToken();

            var validatedCards = await ValidatePlayersDecks(tournamentDecksDto, idOwner);

            var cardSeriesIds = await _cardDao.GetIdCardSeriesByCardIdAsync(validatedCards);

            if (cardSeriesIds == null || !cardSeriesIds.Any())
                throw new ArgumentException("Invalid series name provided.");

            //TODO: Refactorizar lo referido al player capacity

            //var tournamentsPlayers = await _tournamentDao.GetTournamentPlayersAsync(tournamentDecksDto.Id_Tournament);

            //var playersCapacity = await CalculatePlayerCapacity(tournamentDecksDto.Id_Tournament);

            //if (tournamentsPlayers.Count >= playersCapacity)
            //    throw new ArgumentException("Asistencia del torneo Completo, no entran mas jugadores");

            var tournamentDecksModel = new TournamentDecksModel
            {
                Id_Tournament = tournamentDecksDto.Id_Tournament,
                Id_card_series = cardSeriesIds,
                Id_Owner = idOwner
            };

            var userCreationResponse = await _tournamentDao.InsertTournamentPlayersAsync(tournamentDecksModel);
            return await _tournamentDao.InsertTournamentDecksAsync(tournamentDecksModel);
        }

        public async Task<List<int>> ValidatePlayersDecks(TournamentDecksDto tournament, int idOwner)
        {
            bool tournamentExists = await _tournamentDao.TournamentExistsAsync(tournament.Id_Tournament);
            if (!tournamentExists)
                throw new InvalidOperationException("El torneo especificado no existe.");

            var isAvailableTournaments = await _tournamentDao.ValidateAvailableTournamentsAsync(tournament.Id_Tournament);
            if (!isAvailableTournaments.Any())
                throw new ArgumentException("Este torneo ya se encuentra finalizado");

            var duplicateCards = tournament.Cards
                .GroupBy(i => i)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateCards.Any())
                throw new InvalidOperationException($"Las siguientes ilustraciones están duplicadas: {string.Join(", ", duplicateCards)}");

            if (tournament.Cards == null || !tournament.Cards.Any())
                throw new ArgumentException("No se proporcionaron cartas para validar.");

            var playerAlreadyRecorded = await _tournamentDao.ValidateTournamentPlayersAsync(tournament.Id_Tournament, idOwner);
            if (playerAlreadyRecorded.Any())
                throw new InvalidOperationException($"El Jugador {idOwner} ya se encuentra inscripto en el torneo");

            var tournamentSeries = await _tournamentDao.GetSeriesFromTournamentAsync(tournament.Id_Tournament);
            if (tournamentSeries == null || !tournamentSeries.Any())
                throw new InvalidOperationException("El torneo no tiene series pactadas.");

            var validCardsFromSeries = await _tournamentDao.GetCardsFromTournamentSeriesAsync(tournamentSeries,tournament.Cards);
            var invalidCardFromSeries = tournament.Cards.Except(validCardsFromSeries).ToList();
    
            if (invalidCardFromSeries.Any())
                throw new ArgumentException($"Las siguientes series no están registradas: {string.Join(", ", invalidCardFromSeries)}");


            return validCardsFromSeries;
        }

        public async Task<int> CreateTournamentAsync(TournamentDto tournamentDto)
        {
            var idOrganizer = GetUserIdFromToken();

            var tournamentModel = new TournamentModel
            {
                Id_Country = tournamentDto.Id_Country,
                Id_Organizer = idOrganizer,
                Start_datetime = tournamentDto.Start_datetime,
                End_datetime = tournamentDto.End_datetime,
                Current_Phase = 1,
                Judges_Id = tournamentDto.Judges_Id,
                Series_Id = tournamentDto.Series_Id,
            };

            var validatedTournament = await ValidateTournament(tournamentModel);
            return await _tournamentDao.CreateTournamentAsync(tournamentModel);
        }

        private int GetUserIdFromToken()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var userIdClaim = userClaims?.FindFirst(ClaimTypes.NameIdentifier); //CAMBIAR POR ID

            if (userIdClaim == null)
                throw new InvalidOperationException("Error al recuperar el owner");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                throw new InvalidOperationException("El ID del usuario en el token no es válido.");

            return userId;
        }



        public async Task<int> CalculatePlayerCapacity(int id_tournament)
        {
            TournamentDto tournament = await GetTournamentById(id_tournament);

            var start_datetime = tournament.Start_datetime;
            var end_datetime = tournament.End_datetime;

            if (end_datetime <= start_datetime)
            {
                throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            TimeSpan startTimeLimit = TimeSpan.FromHours(7);  // 07:00 AM
            TimeSpan endTimeLimit = TimeSpan.FromHours(19);   // 07:00 PM

            int numberOfSegments = 0;

            DateTime current = start_datetime;
            while (current < end_datetime)
            {
                TimeSpan currentTime = current.TimeOfDay;

                if (currentTime >= startTimeLimit && currentTime < endTimeLimit)
                {
                    numberOfSegments++;
                }

                current = current.AddMinutes(30);
            }

            int maxPlayers = numberOfSegments * 2;
            return await Task.FromResult(maxPlayers);
        }

        public async Task<TournamentDto> GetTournamentById(int id_tournament)
        {
            var tournamentModel = await _tournamentDao.GetTournamentByIdAsync(id_tournament);

            var tournamentDto = new TournamentDto
            {
                Start_datetime = tournamentModel.Start_datetime,
                End_datetime = tournamentModel.End_datetime,
                Id_Country = tournamentModel.Id_Country,
            };

            return tournamentDto;
        }

        public async Task<bool> ValidateTournament(TournamentModel tournament)
        {

            var registeredCountries = await _tournamentDao.GetCountriesFromDbAsync();
            if (!registeredCountries.Contains(tournament.Id_Country))
                throw new ArgumentException("El país especificado no está registrado.");


            var isValidOrganizer = await _tournamentDao.ValidateUsersFromDbAsync(1, new List<int> { tournament.Id_Organizer });
            if (!isValidOrganizer.Any())
                throw new ArgumentException("El organizador especificado no está registrado.");


            var areValidJudges = await _tournamentDao.ValidateUsersFromDbAsync(3, tournament.Judges_Id);
            var invalidJudges = (tournament.Judges_Id ?? new List<int>()).Except(areValidJudges).ToList();
             if (invalidJudges.Any())
                throw new ArgumentException($"Los siguientes jueces no están registrados: {string.Join(", ", invalidJudges)}");


            var areValidSeries = await _cardDao.ValidateSeriesAsync(tournament.Series_Id);
            var invalidSeries = tournament.Series_Id.Except(areValidSeries).ToList();
            if (invalidSeries.Any())
                throw new ArgumentException($"Las siguientes series no están registradas: {string.Join(", ", invalidSeries)}");

            return true;
        }

        public async Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases)
        {
            if (judgeAliases == null || !judgeAliases.Any())
            {
                throw new ArgumentException("The list of judge aliases cannot be empty.", nameof(judgeAliases));
            }
            return await _tournamentDao.GetJudgeIdsByAliasAsync(judgeAliases);
        }

        public async Task<IEnumerable<TournamentsInformationDto>> GetTournamentsInformationAsync(GetTournamentInformationDto getTournamentInformation)
        {
            var getTournamentInformationModel = new GetTournamentInformationModel
            {
                Current_phase = getTournamentInformation.Current_phase
            };

            var tournaments = await _tournamentDao.GetTournamentsInformationAsync(getTournamentInformationModel);

            return tournaments.Select(tournament => new TournamentsInformationDto
            {
                Id_Torneo = tournament.Id_Torneo,
                Pais = tournament.Pais,
                FechaDeInicio = tournament.FechaDeInicio,
                FechaDeFinalizacion = tournament.FechaDeFinalizacion,
                Jueces = tournament.Jueces,
                Series = tournament.Series,
                Jugadores = tournament.Jugadores,
                Ganador = tournament.Ganador,
            });
        }

        public async Task<int> SetTournamentToNextPhase(int tournament_id)
        {
            var id_tournament = tournament_id;
            return await _tournamentDao.GetTournamentCurrentPhaseAsync(id_tournament);
        }
    }
}
