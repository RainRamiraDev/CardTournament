using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Game;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Game;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Game;
using CTDto.Tournaments;
using CTService.Interfaces.Game;
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

        private readonly ITournamentDao _touurnamentDao;

        public GameService(IGameDao gameDao, ITournamentDao touurnamentDao)
        {
            _gameDao = gameDao;
            _touurnamentDao = touurnamentDao;
        }

        public async Task<int> CreateGameAsync(GameDto gameDto)
        {
            // pasara los datos de la creacion del torneo y seteara los usuarios para el InsertGamePlayersAsync

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

        public async Task<int> InsertGamePlayersAsync(GamePlayersDto gamePlayers)
        {
            if (gamePlayers == null || gamePlayers.Id_Player.Count == 0)
            {
                throw new ArgumentException("Invalid series name provided.");
            }

            var tournamentPlayersId = await _gameDao.GetTournamentPlayers(TournamentDao.createdtournamentId);

            var gamePlayersModel = new GamePlayersModel
            {
                Id_Game = gamePlayers.Id_Game,
                Id_Player = tournamentPlayersId,
            };

            return await _gameDao.InsertGamePlayersAsync(gamePlayersModel);

        }
    }
}
