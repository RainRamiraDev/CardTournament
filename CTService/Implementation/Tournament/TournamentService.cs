﻿using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Game;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Tournaments;
using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CTDto.Users.Judge;
using CTDataModels.Users.Judge;
using CTDto.Users;

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
                throw new ArgumentException("Nombre de las serie invalido.");

            //var playerCapacity = await CalculatePlayerCapacity(tournamentDecksDto.Id_Tournament);
            //var capacityCompleted = await _tournamentDao.CheckTournamentCapacity(playerCapacity, tournamentDecksDto.Id_Tournament);
            //if (capacityCompleted)
            //    throw new ArgumentException("Torneo completo.");

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
                Start_datetime = DateTime.SpecifyKind(tournamentDto.Start_datetime, DateTimeKind.Utc),
                End_datetime = DateTime.SpecifyKind(tournamentDto.End_datetime, DateTimeKind.Utc),
                Current_Phase = 1,
                Judges_Id = tournamentDto.Judges_Id,
                Series_Id = tournamentDto.Series_Id,
            };

            await ValidateTournament(tournamentModel);
            return await _tournamentDao.CreateTournamentAsync(tournamentModel);
        }

        private int GetUserIdFromToken()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var userIdClaim = userClaims?.FindFirst("UserId");

            if (userIdClaim == null)
                throw new InvalidOperationException("Error al recuperar el dueño");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                throw new InvalidOperationException("El ID del usuario en el token no es válido.");

            return userId;
        }

        public async Task<PlayerCapacityModel> CalculatePlayerCapacity(int id_tournament, int AvailableDailyHours)
        {
            // Traigo las fechas establecidas del torneo 
            TournamentDto tournament = await GetTournamentById(id_tournament);

            // Corroboro que las fechas estén como UTC 
            var startDatetime = tournament.Start_datetime.ToUniversalTime();
            var endDatetime = tournament.End_datetime.ToUniversalTime();

            // Calculo el total de minutos disponibles por día, basándome en las horas disponibles
            int availableMatchMinutesPerDay = AvailableDailyHours * 60; // Convierte las horas disponibles en minutos

            // Calculo los minutos totales disponibles en el plazo de tiempo
            var totalMinutes = (endDatetime - startDatetime).TotalMinutes;

            // Calculo los días totales sabiendo cuál es el plazo de tiempo diario
            int totalDays = (int)(endDatetime.Date - startDatetime.Date).TotalDays + 1; // +1 porque incluye el día de inicio

            // Se calculan la cantidad de partidos que se podrán hacer en los días disponibles en base al tiempo disponible por día
            int totalAvailableMatchMinutes = totalDays * availableMatchMinutesPerDay;

            var maxMatches = totalAvailableMatchMinutes / 30; // 30 minutos por partido

            // El valor máximo calculado lo redondea a la potencia más cercana de 2
            maxMatches = (int)Math.Pow(2, Math.Floor(Math.Log(maxMatches) / Math.Log(2)));

            // Sabiendo que por cada match hay dos jugadores, calcula la cantidad máxima de jugadores
            var maxPlayers = maxMatches * 2;

            Console.WriteLine($"Max players calculated: {maxPlayers}");


            // Me establece que el mínimo son 16 jugadores, es decir, que mínimo voy a tener octavos en mi torneo
            // Si es más de 16, pone el cap en la potencia de 2 más cercana
            //int minPlayers = Math.Max(16, (int)Math.Pow(2, Math.Ceiling(Math.Log2(maxPlayers))));

            int minPlayers = 16;


            Console.WriteLine($"Min players calculated: {minPlayers}");



            minPlayers = Math.Min(minPlayers, maxPlayers);

            var playerCapacity = new PlayerCapacityModel
            {
                MaxPlayers = maxPlayers,
                MinPlayers = minPlayers
            };

            return playerCapacity;
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

        public async Task ValidateTournament(TournamentModel tournament)
        {

            var registeredCountries = await _tournamentDao.ValidateCountriesFromDbAsync(tournament.Id_Country);
            if (!registeredCountries.Any())
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

        public async Task AlterTournamentAsync(AlterTournamentDto tournamentDto)
        {
            var oldTournament = await _tournamentDao.GetTournamentByIdAsync(tournamentDto.Id_tournament)
                ?? throw new KeyNotFoundException("El torneo no existe");

            var organizerFromToken = GetUserIdFromToken();

            var isAdmin = await _tournamentDao.ValidateUsersFromDbAsync(2, new List<int> { organizerFromToken });

            if (oldTournament.Id_Organizer != organizerFromToken && !isAdmin.Contains(organizerFromToken))
                throw new UnauthorizedAccessException("No tiene permiso para alterar este torneo.");

            var alterTournamentModel = new AlterTournamentModel
            {
                Id_tournament = tournamentDto.Id_tournament,
                Id_Country = tournamentDto.Id_Country,
                Id_Organizer = organizerFromToken,
                Start_datetime = DateTime.SpecifyKind(tournamentDto.Start_datetime, DateTimeKind.Utc),
                End_datetime = DateTime.SpecifyKind(tournamentDto.End_datetime, DateTimeKind.Utc),       
                Judges_Id = tournamentDto.Judges_Id,
                Series_Id = tournamentDto.Series_Id
            };

            // Validación sin crear una variable extra
            await ValidateTournament(new TournamentModel
            {
                Id_Country = alterTournamentModel.Id_Country,
                Id_Organizer = alterTournamentModel.Id_Organizer,
                Start_datetime = alterTournamentModel.Start_datetime,
                End_datetime = alterTournamentModel.End_datetime,
                Judges_Id = alterTournamentModel.Judges_Id,
                Series_Id = alterTournamentModel.Series_Id
            });

            await _tournamentDao.AlterTournamentAsync(alterTournamentModel);
        }

        public async Task SoftDeleteTournamentAsync(TournamentRequestToResolveDto tournamentDto)
        {
            var isValidTournament = await _tournamentDao.TournamentExistsAsync(tournamentDto.Tournament_Id);
            if (!isValidTournament)
                throw new ArgumentException("El torneo especificado no ha sido encontrado");

            await _tournamentDao.SoftDeleteTournamentAsync(tournamentDto.Tournament_Id);
        }

        public async Task DisqualifyPlayerFromTournamentAsync(DisqualificationDto disqualificationDto)
        {

            var idJudgeFromToken = GetUserIdFromToken();
            var disqualificationRequest = new DisqualificationModel
            {
                Id_Tournament = disqualificationDto.Id_Tournament,
                Id_Player = disqualificationDto.Id_Player,
                Id_Judge = idJudgeFromToken,
            };

            await ValidatePlayerDisqualification(disqualificationRequest);

            await _tournamentDao.DisqualifyPlayerFromTournamentAsync(disqualificationRequest);


        }

        private async Task ValidatePlayerDisqualification(DisqualificationModel disqualificationRequest)
        {
            //validar que el torneo exista
            var tournamentExist = await _tournamentDao.TournamentExistsAsync(disqualificationRequest.Id_Tournament);
            if (!tournamentExist)
                throw new ArgumentException("El torneo especificado no se encuentra registrado");

            //validar que el juez pertenezca a ese torneo

            var isValidJudge = await _tournamentDao.ValidateJudgesFromTournament(disqualificationRequest.Id_Judge, disqualificationRequest.Id_Tournament);
            if (!isValidJudge)
                throw new ArgumentException("El juez especificado no se encuentra registrado en este torneo");


            //validar que el jugador este jugado a ese torneo y no haya sido previamente descalificado
            var isValidPlayer = await _tournamentDao.ValidateTournamentPlayersAsync(disqualificationRequest.Id_Tournament,disqualificationRequest.Id_Player);
            if(!isValidPlayer.Any())
                throw new ArgumentException("El jugador especificado no se encuentra registrado en el torneo");
        }

        public async Task<List<ShowTournamentPlayersDto>> ShowPlayersFromTournamentAsync(TournamentRequestToResolveDto showPlayersFromTournamentDto)
        {
            // Verificar si el torneo existe
            var tournamentExist = await _tournamentDao.TournamentExistsAsync(showPlayersFromTournamentDto.Tournament_Id);
            if (!tournamentExist)
                throw new ArgumentException("El torneo especificado no se encuentra registrado");

            var tournamentPlayers = await _tournamentDao.ShowPlayersFromTournamentAsync(showPlayersFromTournamentDto.Tournament_Id);

            var playerDtos = tournamentPlayers.Select(player => new ShowTournamentPlayersDto
            {
                Id_Player = player.Id_Player,
                Alias = player.Alias, 
                Disqualifications = player.Disqualifications,
                Avatar_Url = player.Avatar_Url,
            }).ToList();

            return playerDtos;
        }

    }
}
