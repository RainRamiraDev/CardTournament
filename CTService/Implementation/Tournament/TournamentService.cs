using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Tournaments;
using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.Tournament
{
    public class TournamentService : ITournamentService
    {

        private readonly ITournamentDao _tournamentDao;

        private readonly ICardDao _cardDao;


        public TournamentService(ITournamentDao tournamentDao, ICardDao cardDao)
        {
            _tournamentDao = tournamentDao;
            _cardDao = cardDao;
        }

        public async Task<int> InsertTournamentDecksAsync(TournamentDecksDto tournamentDecksDto)
        {

            if (tournamentDecksDto == null)
            {
                throw new ArgumentException("Invalid tournament data.");
            }

            bool tournamentExists = await _tournamentDao.TournamentExistsAsync(tournamentDecksDto.Id_Tournament);

            if (!tournamentExists)
            {
                throw new InvalidOperationException("El torneo especificado no existe.");
            }

            var duplicateIllustrations = tournamentDecksDto.illustration
                .GroupBy(i => i)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIllustrations.Any())
            {
                throw new InvalidOperationException($"Las siguientes ilustraciones están duplicadas: {string.Join(", ", duplicateIllustrations)}");
            }

            var cardIds = await _cardDao.GetCardIdsByIllustrationAsync(tournamentDecksDto.illustration);

            var validatedCards = await ValidatePlayersDecks(cardIds, tournamentDecksDto);

            var cardSeriesIds = await _cardDao.GetIdCardSeriesByCardIdAsync(validatedCards);

            if (cardSeriesIds == null || !cardSeriesIds.Any())
            {
                throw new ArgumentException("Invalid series name provided.");
            }

            var tournamentsPlayers = await _tournamentDao.GetTournamentPlayers(tournamentDecksDto.Id_Tournament);
            var playersCapacity = await CalculatePlayerCapacity(tournamentDecksDto.Id_Tournament);

            if (tournamentsPlayers.Count >= playersCapacity)
                throw new ArgumentException("Asistencia del torneo Completo, no entran mas jugadores");

            var tournamentDecksModel = new TournamentDecksModel
            {
                Id_Tournament = tournamentDecksDto.Id_Tournament,
                Id_card_series = cardSeriesIds,
                Id_Owner = tournamentDecksDto.Id_Owner,
            };

            var userCreationResponse = await _tournamentDao.InsertTournamentPlayersAsync(tournamentDecksModel);
            return await _tournamentDao.InsertTournamentDecksAsync(tournamentDecksModel);
        }

        public async Task<List<int>> ValidatePlayersDecks(List<int> cardIds, TournamentDecksDto tournament)
        {
            if (cardIds == null || !cardIds.Any())
                throw new ArgumentException("No se proporcionaron cartas para validar.");

            var registeredPlayers = await _tournamentDao.GetUsersFromDb(4);


            if (registeredPlayers == null || !registeredPlayers.Any())
                throw new ArgumentException("No se encontraron Jugadores en la Base de datos");

            if (!registeredPlayers.Contains(tournament.Id_Owner))
                throw new ArgumentException("Este Dueño no es un jugador");

            var tournamentPlayers = await _tournamentDao.GetTournamentPlayers(tournament.Id_Tournament);

            if (tournamentPlayers.Contains(tournament.Id_Owner))
                throw new ArgumentException("Este jugador ya se encuentra registrado");

            var tournamentSeries = await _tournamentDao.GetSeriesFromTournamentAsync(tournament.Id_Tournament);

            if (tournamentSeries == null || !tournamentSeries.Any())
                throw new InvalidOperationException("El torneo no tiene series pactadas.");

            var validCardsFromSeries = await _tournamentDao.GetCardsFromTournamentSeries(tournamentSeries);

            var validCards = new List<int>();

            var invalidCards = new List<int>();

            foreach (var card in cardIds)
            {
                if (validCardsFromSeries.Contains(card))
                {
                    validCards.Add(card);
                }
                else
                {
                    invalidCards.Add(card);
                }
            }

            if (invalidCards.Any())
            {
                var wrongCardIllustrations = await _cardDao.GetCardIllustrationById(invalidCards);
                throw new InvalidOperationException($"Las siguientes cartas no pertenecen a las series permitidas:\n{string.Join("\n", wrongCardIllustrations)}");
            }

            return validCards;
        }

        public async Task<int> CreateTournamentAsync(TournamentDto tournamentDto)
        {

            var judgeIds = await _tournamentDao.GetJudgeIdsByAliasAsync(tournamentDto.Judges_Alias);
            var seriesIds = await _cardDao.GetSeriesIdsByNameAsync(tournamentDto.Series_name);

            var tournamentModel = new TournamentModel
            {
                Id_Country = tournamentDto.Id_Country,
                Id_Organizer = tournamentDto.Id_Organizer,
                Start_datetime = tournamentDto.Start_datetime,
                End_datetime = tournamentDto.End_datetime,
                Current_Phase = 1,
                Judges = judgeIds,
                Series_name = seriesIds,
            };

            var validatedTournament = await ValidateTournament(tournamentModel);
            return await _tournamentDao.CreateTournamentAsync(tournamentModel);
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
            var tournamentModel = await _tournamentDao.GetTournamentById(id_tournament);

            var tournamentDto = new TournamentDto
            {
                Start_datetime = tournamentModel.Start_datetime,
                End_datetime = tournamentModel.End_datetime,
                Id_Country = tournamentModel.Id_Country,
                Id_Organizer = tournamentModel.Id_Organizer,
            };

            return tournamentDto;
        }

        public async Task<bool> ValidateTournament( TournamentModel tournament)
        {

            var registeredCountries = await _tournamentDao.GetCountriesFromDb();
            if (!registeredCountries.Contains(tournament.Id_Country))
                throw new ArgumentException("El país especificado no está registrado.");

            var registeredOrganizers = await _tournamentDao.GetUsersFromDb(1);
            if (!registeredOrganizers.Contains(tournament.Id_Organizer))
                throw new ArgumentException("El organizador especificado no está registrado.");

            var registeredJudges = await _tournamentDao.GetUsersFromDb(3);

            var invalidJudges = tournament.Judges.Where(idJudge => !registeredJudges.Contains(idJudge)).ToList();

            if (invalidJudges.Any())
            {
                throw new ArgumentException($"Los siguientes jueces no están registrados: {string.Join(", ", invalidJudges)}");
            }

            var registeredSeries = await _cardDao.GetAllSeries();

            var invalidSeries = tournament.Series_name.Where(seriesId => !registeredSeries.Contains(seriesId)).ToList();

            if (invalidSeries.Any())
            {
                throw new ArgumentException($"Las siguientes series no están registradas: {string.Join(", ", invalidSeries)}");
            }

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
                RondasTotales = tournament.RondasTotales,
                MatchesTotales = tournament.MatchesTotales,
                Ganador = tournament.Ganador,
            });
        }

        public async Task<int> SetTournamentToNextPhase(int tournament_id)
        {
            var id_tournament = tournament_id;
            return await _tournamentDao.GetTournamentCurrentPhase(id_tournament);
        }
    }
}
