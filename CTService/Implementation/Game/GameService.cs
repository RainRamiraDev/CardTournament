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

        public async Task<GameResultDto> ResolveGameAsync(TournamentRequestToResolveDto tournamentDto)
        {
            var tournamentModel = new TournamentRequestToResolveModel
            {
               Tournament_Id = tournamentDto.Tournament_Id,
            };

            bool tournamentExists = await _tournamentDao.TournamentExistsAsync(tournamentModel.Tournament_Id);

            if (!tournamentExists)
                throw new InvalidOperationException("El torneo especificado no existe.");


            List<int> playersIds = await _gameDao.GetTournamentPlayersAsync(tournamentModel.Tournament_Id);

            if (playersIds.Count < 2)
            {
                throw new InvalidOperationException("Se necesitan al menos dos jugadores para iniciar el torneo.");
            }

            List<int> tournamentJudgesIds = await _tournamentDao.GetTournamentJudgesAsync(tournamentDto.Tournament_Id);

            int roundNumber = await _gameDao.GetLastRoundAsync(tournamentModel.Tournament_Id);

            HashSet<int> eliminatedPlayers = new HashSet<int>();

            // torneo fase 2
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
                        Winner = winner
                    };

                    await CreateMatchAsync(match);
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
                throw new ArgumentException("La lista de jueces no puede estar vacía.");
            }
            Random random = new Random();
            int randomIndex = random.Next(judges.Count);

            return await Task.FromResult(judges[randomIndex]);
        }


    }
}
