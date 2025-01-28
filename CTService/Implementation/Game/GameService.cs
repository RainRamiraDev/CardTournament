using CTDao.Dao.Game;
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

            if (gameDto == null)
            {
                throw new ArgumentException("Invalid tournament data.");
            }

            var gameModel = new GameModel
            {
                Id_Tournament = TournamentDao.createdtournamentId,
                Start_Date = DateTime.Now,
            };

            Console.WriteLine("Torneo actual:"+gameModel.Id_Tournament);

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
            };

            //    Console.WriteLine("Match actual:" + gameModel.Id_Tournament);

            return await _gameDao.CreateMatchAsync(matchModel);
        }

        public async Task<int> CreateRoundAsync(RoundDto round)
        {
            if (round == null)
            {
                throw new ArgumentException("Invalid round data.");
            }

            var roundModel = new RoundModel
            {
                Id_Tournament = TournamentDao.createdtournamentId,
                Round_Number = round.Round_Number,
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

        public Task<int> ResolveGameAsync(GameResultDto result)
        {
            throw new NotImplementedException();


            //1) CREAR LA RONDA
           
           
       
            //2) TRAER LA LISTA DE USUARIOS Y SEPARARLOS POR PARES DENTRO DE LOS MATCH

            //3) AVANZA A LA SIGUIENTE MATCH

            //4) TERMINA TODOS LOS MATCH Y DAR EL RESULTADO 

            //5)


            //7)

            //8)
            //9)


            //10)
            //)

            //)
            //)

            //)
            //)

            //)
            //)











        }
    }
}
