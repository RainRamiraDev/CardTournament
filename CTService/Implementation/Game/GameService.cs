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


        public GameService(IGameDao gameDao, ITournamentDao touurnamentDao,IUserDao userDao)
        {
            _gameDao = gameDao;
            _tournamentDao = touurnamentDao;
            _userDao = userDao;
        }


        public async Task<int> CreateMatchAsync(MatchDto match)
        {
            if (match == null)
            {
                throw new ArgumentException("Invalid match data.");
            }

            var matchModel = new MatchModel
            {
                Id_Round = match.Id_Round,
                Id_Player1 = match.Id_Player1,
                Id_Player2 = match.Id_Player2,
                Winner = match.Winner
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

        public async Task<GameResultDto> ResolveGameAsync(TournamentRequestDto tournamentDto)
        {
            var tournamentModel = new TournamentRequestModel
            {
               Tournament_Id = tournamentDto.Tournament_Id,
               dailyHoursAvailable = tournamentDto.dailyHoursAvailable,
            };


            bool tournamentExists = await _tournamentDao.TournamentExistsAsync(tournamentModel.Tournament_Id);

            if (!tournamentExists)
            {
                throw new InvalidOperationException("El torneo especificado no existe.");
            }


            List<int> playersIds = await _gameDao.GetTournamentPlayers(tournamentModel.Tournament_Id);

            if (playersIds.Count < 2)
            {
                throw new InvalidOperationException("Se necesitan al menos dos jugadores para iniciar el torneo.");
            }

            int roundNumber = await _gameDao.GetLastRoundAsync(tournamentModel.Tournament_Id);


            HashSet<int> eliminatedPlayers = new HashSet<int>();

            // torneo fase 2
            await _tournamentDao.SetTournamentToNextPhase(tournamentModel.Tournament_Id);

            int totalMatches = 0; 

            while (playersIds.Count > 1)
            {
                int createdRoundId = await CreateRoundAsync(roundNumber, tournamentModel.Tournament_Id);

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
                        Winner = winner
                    };

                    await CreateMatchAsync(match);

                    totalMatches++; 

                    Console.WriteLine("Bandera de Match aumentada "+ totalMatches);
                }

                playersIds = winners;

                await _gameDao.SetRoundCompletedAsync(roundNumber, tournamentModel.Tournament_Id);

                roundNumber++;

                //Console.WriteLine("[Round number incrised]");
            }

            // torneo fase 3
            await _tournamentDao.SetTournamentToNextPhase(tournamentModel.Tournament_Id);

            await _gameDao.SetRoundCompletedAsync(roundNumber - 1, tournamentModel.Tournament_Id);
            await _gameDao.SetGameWinnerAsync(playersIds[0]);


            if (eliminatedPlayers.Count > 0)
            {
                await _gameDao.SetGameLoserAsync(eliminatedPlayers.ToList());
            }

            Console.WriteLine("Bandera pre definicion " + totalMatches);

            var tournamentDuration =  await UpdateTournamentEndDate(totalMatches, tournamentModel.Tournament_Id, tournamentModel.dailyHoursAvailable); //💡

            Console.WriteLine("Duracion del torneo "+ tournamentDuration);
           
            return new GameResultDto
            {
                WinnerId = playersIds[0],
                Message = $"El ganador del torneo es el jugador {playersIds[0]}." +
                          $"La cantidad de rondas ha sido {totalMatches}" +
                          $"La Duracion Total del Torneo ha sido de {tournamentDuration}",
            };
        }


        public async Task<TimeSpan> UpdateTournamentEndDate(int total_Matches, int id_tournament, int dailyHoursAvailable)
        {

            int totalDurationMinutes = total_Matches * 30;

            int dailyMinutesAvailable = dailyHoursAvailable * 60;

            DateTime startDatetime = await _tournamentDao.GetTournamentStartDateAsync(id_tournament);

            DateTime endDatetime = startDatetime;

            while (totalDurationMinutes > 0)
            {
                if (totalDurationMinutes > dailyMinutesAvailable)
                {
                    // Si excede el tiempo disponible en un día, sumamos un día y restamos el tiempo jugado
                    endDatetime = endDatetime.AddDays(1).Date.AddHours(9);
                    totalDurationMinutes -= dailyMinutesAvailable;
                }
                else
                {
                    // Si cabe en el mismo día, simplemente sumamos la duración restante
                    endDatetime = endDatetime.AddMinutes(totalDurationMinutes);
                    totalDurationMinutes = 0;
                }
            }

            var setTournamentEndDateTime = new TournamentUpdateEndDatetimeModel
            {
                Id_tournament = id_tournament,
                End_DateTime = endDatetime,
            };


            await _tournamentDao.SetTournamentEndDate(setTournamentEndDateTime);

            TimeSpan tournamentDuration = endDatetime - startDatetime;

            return tournamentDuration;
        }

        public async Task<int> CreateRoundAsync(int roundNumber, int tournament_id)
        {
            var roundModel = new RoundModel
            {
                Id_Tournament = tournament_id,
                Round_Number = roundNumber,
                Is_Completed = false,
            };
            return await _gameDao.CreateRoundAsync(roundModel);
        }

    }
}
