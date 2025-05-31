using CTDao.Dao.Game;
using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Game;
using CTDao.Interfaces.Tournaments;
using CTDao.Interfaces.User;
using CTDataModels.Game;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Game;
using CTDto.Tournaments;
using CTService.Interfaces.Game;
using CTService.Interfaces.Tournaments;
using CTService.Tools;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.Game
{
    public class GameService : IGameService
    {
        private readonly IGameDao _gameDao;

        private readonly ITournamentDao _tournamentDao;

        private readonly IUserDao _userDao;

        private readonly ITournamentService _tournamentService;


        public GameService(IGameDao gameDao, ITournamentDao touurnamentDao,IUserDao userDao, ITournamentService tournamentService)
        {
            _gameDao = gameDao;
            _tournamentDao = touurnamentDao;
            _userDao = userDao;
            _tournamentService = tournamentService;
        }

        public async Task<int> CreateMatchAsync(MatchDto match)
        {
            if (match == null)
                throw new ArgumentException("Invalid match data.");
            
            var matchModel = new MatchModel
            {
                Id_Round = match.Id_Round,
                Id_Player1 = match.Id_Player1,
                Id_Player2 = match.Id_Player2,
                Winner = match.Winner,
                Match_Date = match.Match_Date,
            };
            return await _gameDao.CreateMatchAsync(matchModel);
        }

        public async Task<int> CreateRoundAsync(int tournament_id)
        {
            var roundModel = new RoundModel
            {
                Id_Tournament = tournament_id,
                Round_Number = 0,
                Is_Completed = false,

            };
            return await _gameDao.CreateRoundAsync(roundModel);
        }

        public async Task<GameResultDto> ResolveGameAsync(TournamentRequestToResolveDto tournamentDto, string timeZone)
        {
            var tournamentModel = new TournamentRequestToResolveModel
            {
               Tournament_Id = tournamentDto.Tournament_Id,
            };

            bool tournamentExists = await _tournamentDao.TournamentExistsAsync(tournamentModel.Tournament_Id);

            if (!tournamentExists)
                throw new KeyNotFoundException("El torneo especificado no existe.");

            var playerCapacity = await _tournamentService.CalculatePlayerCapacity(tournamentDto.Tournament_Id, tournamentDto.availableHoursPerDay);
            var capacityCompleted = await _tournamentDao.CheckTournamentCapacity(playerCapacity, tournamentDto.Tournament_Id);
            if (!capacityCompleted)
                throw new InvalidOperationException("El torneo aun debe completar su capacidad.");


            List<int> playersIds = await _gameDao.GetTournamentPlayersAsync(tournamentModel.Tournament_Id);

            if (playersIds.Count < 2)
                throw new InvalidOperationException("Se necesitan al menos dos jugadores para iniciar el torneo.");


            List<int> tournamentJudgesIds = await _tournamentDao.GetTournamentJudgesAsync(tournamentDto.Tournament_Id);

            int roundNumber = await _gameDao.GetLastRoundAsync(tournamentModel.Tournament_Id);

            HashSet<int> eliminatedPlayers = new HashSet<int>();

            var matchSchedule = await CalculateTournamentScheduleAsync(tournamentDto.Tournament_Id, tournamentDto.availableHoursPerDay,timeZone);
            int scheduleIndex = 0; 

            await _tournamentDao.SetTournamentToNextPhaseAsync(tournamentModel.Tournament_Id);

            int totalMatches = 0; 

            while (playersIds.Count > 1)
            {
                var judgeId = await GetRandomJudge(tournamentJudgesIds);
                int createdRoundId = await CreateRoundAsync(roundNumber, tournamentModel.Tournament_Id,judgeId);

                List<int> winners = new List<int>();

                for (int i = 0; i < playersIds.Count; i += 2)
                {
                    if (i + 1 >= playersIds.Count)
                    {
                        winners.Add(playersIds[i]);
                        continue;
                    }

                    int player1 = playersIds[i];
                    int player2 = playersIds[i + 1];

                    int player1Ki = await _userDao.GetPlayerKiByIdAsync(player1);
                    int player2Ki = await _userDao.GetPlayerKiByIdAsync(player2);

                    int winner = player1Ki > player2Ki ? player1 : player2;
                    int loser = player1Ki > player2Ki ? player2 : player1;


                    winners.Add(winner);
                    eliminatedPlayers.Add(loser);


                    var match = new MatchDto
                    {
                        Id_Round = createdRoundId,
                        Id_Player1 = player1,
                        Id_Player2 = player2,
                        Winner = winner,
                        Match_Date = matchSchedule[scheduleIndex].Match_Date // Asignar la fecha y hora
                    };


                    await CreateMatchAsync(match);

                    scheduleIndex++; 
                }

                playersIds = winners;

                await _gameDao.SetRoundCompletedAsync(roundNumber, tournamentModel.Tournament_Id);

                roundNumber++;
            }

            // torneo fase 3
            await _tournamentDao.SetTournamentToNextPhaseAsync(tournamentModel.Tournament_Id);

            await _gameDao.SetRoundCompletedAsync(roundNumber - 1, tournamentModel.Tournament_Id);
            await _gameDao.SetGameWinnerAsync(playersIds[0]);


            if (eliminatedPlayers.Count > 0)
            {
                await _gameDao.SetGameLoserAsync(eliminatedPlayers.ToList());
            }


            return new GameResultDto
            {
                WinnerId = playersIds[0],
                Message = $"El ganador del torneo es el jugador {playersIds[0]}."
            };
        }


        public async Task DisqualifyPlayerAsync(int playerId, int tournamentId, int judgeId)
        {
            await ValidatePlayerDisqualification(playerId, tournamentId, judgeId);

            await _gameDao.InsertDisqualificationAsync(playerId, tournamentId, judgeId);

        }

        public async Task ValidatePlayerDisqualification(int playerId, int tournamentId, int judgeId)
        {
            var isValidTournament = await _tournamentDao.TournamentExistsAsync(tournamentId);
            if (!isValidTournament)
                throw new KeyNotFoundException("El torneo especificado no es valido");

            var isValidPlayer = await _tournamentDao.ValidateTournamentPlayersAsync(tournamentId,playerId);
            if(!isValidPlayer.Contains(playerId))
                throw new InvalidOperationException("El jugador especificado no esta registrado en este torneo");

            var isValidJudge = await _tournamentDao.ValidateUsersFromDbAsync(tournamentId, new List<int>{judgeId});

            bool alreadyDisqualified = await _gameDao.IsPlayerDisqualifiedAsync(playerId, tournamentId);
            if (alreadyDisqualified)
                throw new InvalidOperationException("El jugador ya ha sido descalificado en este torneo.");
        }

        public async Task<int> CreateRoundAsync(int roundNumber, int tournament_id, int id_judge)
        {
            var roundModel = new RoundModel
            {
                Id_Tournament = tournament_id,
                Round_Number = roundNumber,
                Judge = id_judge,
                Is_Completed = false,
            };
            return await _gameDao.CreateRoundAsync(roundModel);
        }

        public async Task<int> GetRandomJudge(List<int> judges)
        {
            if (judges == null || judges.Count == 0)
            {
                throw new InvalidOperationException("La lista de jueces no puede estar vacía.");
            }
            Random random = new Random();
            int randomIndex = random.Next(judges.Count);

            return await Task.FromResult(judges[randomIndex]);
        }

        public async Task<List<MatchScheduleModel>> CalculateTournamentScheduleAsync(int tournamentId, int availableHoursPerDay, string clientTimeZonez)
        {
            PlayerCapacityModel playerCapacity = await _tournamentService.CalculatePlayerCapacity(tournamentId, availableHoursPerDay);

            TournamentDto tournament = await _tournamentService.GetTournamentById(tournamentId);
            var startDatetime = tournament.Start_datetime.ToUniversalTime();
            var endDatetime = tournament.End_datetime.ToUniversalTime();

            DateTime startDateTimeLocal = DateTimeConverter.ConvertToTimeZone(startDatetime, clientTimeZonez);
            DateTime endDateTimeLocal = DateTimeConverter.ConvertToTimeZone(endDatetime, clientTimeZonez);

            int totalDays = (int)(endDatetime.Date - startDatetime.Date).TotalDays + 1; 

            var matchStartTime = new TimeSpan(9, 0, 0); 
            var matchEndTime = matchStartTime.Add(TimeSpan.FromHours(availableHoursPerDay)); 

            int availableMatchMinutesPerDay = (int)(matchEndTime - matchStartTime).TotalMinutes;

            int totalMatches = playerCapacity.MaxPlayers / 2; 

            List<MatchScheduleModel> matchSchedule = new List<MatchScheduleModel>();


            int matchId = 1;  

            for (int day = 0; day < totalDays; day++)
            {
                DateTime currentDate = startDatetime.AddDays(day);
                TimeSpan currentMatchTime = matchStartTime;

                for (int match = 0; match < totalMatches; match++)
                {
                    if (currentMatchTime >= matchEndTime)
                    {
                        break; 
                    }

                    var matchDto = new MatchScheduleModel
                    {
                        Match_Date = currentDate.Add(currentMatchTime),
                        Id_Match = matchId  
                    };

                    matchSchedule.Add(matchDto);

                    currentMatchTime = currentMatchTime.Add(TimeSpan.FromMinutes(30)); 

                    matchId++;
                }
            }


            return matchSchedule;
        }

        public async Task<List<MatchScheduleDto>> CalculateMatchScheduleAsync(TournamentRequestToResolveDto request, string timeZone)
        {
            var isValidTournament = await _tournamentDao.TournamentExistsAsync(request.Tournament_Id);
            if (!isValidTournament)
                throw new KeyNotFoundException("El torneo especificado no existe");

            var matchShoudelModel = await CalculateTournamentScheduleAsync(request.Tournament_Id,request.availableHoursPerDay, timeZone);

            List<MatchScheduleDto> matchResponse = new List<MatchScheduleDto>();

            foreach (MatchScheduleModel m in matchShoudelModel)
            {
                MatchScheduleDto dto = new MatchScheduleDto
                {
                    Id_Match = m.Id_Match,
                    Match_Date = m.Match_Date,
                };
                matchResponse.Add(dto);
            };
            return matchResponse;
        }
    }
}
