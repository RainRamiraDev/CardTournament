using CTDao.Interfaces.Game;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Game;
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

        public Task<int> CreateGameAsync(GameDto gameDto)
        {
            // pasara los datos de la creacion del torneo y seteara los usuarios para el InsertGamePlayersAsync

            if (gameDto == null)
            {
                throw new ArgumentException("Invalid tournament data.");
            }


            throw new NotImplementedException();
        }

        public Task<int> ResolveGameAsync()
        {
            throw new NotImplementedException();
        }
    }
}
