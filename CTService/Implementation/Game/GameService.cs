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

        public async Task<int> CreateGameAsync(GameDto gameDto)
        {

            if (gameDto == null)
            {
                throw new ArgumentException("Invalid tournament data.");
            }

            var gameModel = new GameModel
            {
                Id_Tournament = TournamentDao.createdtournamentId,
                Start_Date = DateTime.Now,
            };

            return await _gameDao.CreateGameAsync(gameModel);
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
                Id_Game = match.Id_Game,
                Id_Player1 = match.Id_Player1,
                Id_Player2 = match.Id_Player2,
                Winner = match.Winner
            };

            return await _gameDao.CreateMatchAsync(matchModel);
        }

        public async Task<int> CreateRoundAsync()
        {
            var roundModel = new RoundModel
            {
                Id_Tournament = TournamentDao.createdtournamentId,
                Round_Number = 0,
                Is_Completed = false,

            };
            return await _gameDao.CreateRoundAsync(roundModel);
        }

        public async Task<int> InsertGamePlayersAsync(GamePlayersDto gamePlayers)
        {
            if (gamePlayers == null || gamePlayers.Id_Player.Count == 0)
            {
                throw new ArgumentException("Invalid series name provided.");
            }

            var tournamentPlayersId = await _gameDao.GetTournamentPlayers(TournamentDao.createdtournamentId);

            Console.WriteLine(string.Join(","+tournamentPlayersId));

            var gamePlayersModel = new GamePlayersModel
            {
                Id_Game = gamePlayers.Id_Game,
                Id_Player = tournamentPlayersId,
            };

            return await _gameDao.InsertGamePlayersAsync(gamePlayersModel);

        }

        public async Task<GameResultDto> ResolveGameAsync()
        {
            List<int> playersIds = await _gameDao.GetTournamentPlayers(TournamentDao.createdtournamentId);

            if (playersIds.Count < 2)
            {
                throw new InvalidOperationException("Se necesitan al menos dos jugadores para iniciar el torneo.");
            }

            int roundNumber = await _gameDao.GetLastRoundAsync();

            HashSet<int> eliminatedPlayers = new HashSet<int>();

            // torneo fase 2
            await _tournamentDao.SetTournamentToNextPhase();

            while (playersIds.Count > 1)
            {
                int createdRoundId = await CreateRoundAsync(roundNumber);
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
                        Id_Game = GameDao.createdGameId,
                        Id_Player1 = player1,
                        Id_Player2 = player2,
                        Winner = winner
                    };

                    await CreateMatchAsync(match);
                }

                playersIds = winners;

                await _gameDao.SetRoundCompletedAsync(roundNumber);

                roundNumber++;

                Console.WriteLine("[Round number incrised]");
            }

            // torneo fase 3
            await _tournamentDao.SetTournamentToNextPhase();

            await _gameDao.SetRoundCompletedAsync(roundNumber - 1);
            await _gameDao.SetGameWinnerAsync(playersIds[0]);

            if (eliminatedPlayers.Count > 0)
            {
                await _gameDao.SetGameLoserAsync(eliminatedPlayers.ToList());
            }

            return new GameResultDto
            {
                WinnerId = playersIds[0],
                Message = $"El ganador del torneo es el jugador {playersIds[0]}.",
            };
        }

        public async Task<int> CreateRoundAsync(int roundNumber)
        {
            var roundModel = new RoundModel
            {
                Id_Tournament = TournamentDao.createdtournamentId,
                Round_Number = roundNumber,
                Is_Completed = false,
            };
            return await _gameDao.CreateRoundAsync(roundModel);
        }

    }
}
