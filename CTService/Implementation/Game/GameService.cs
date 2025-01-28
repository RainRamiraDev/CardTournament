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

        public GameService(IGameDao gameDao, ITournamentDao touurnamentDao)
        {
            _gameDao = gameDao;
            _tournamentDao = touurnamentDao;
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

        public async Task<GameResultDto> ResolveGameAsync(RoundDto roundDto)
        {
            throw new NotImplementedException();


            //1) CREAR LA RONDA

             var createdRound = await CreateRoundAsync(roundDto);

            //2) TRAER LA LISTA DE USUARIOS Y SEPARARLOS POR PARES DENTRO DE LOS MATCH

            var players = await _gameDao.GetTournamentPlayers(TournamentDao.createdtournamentId); //hacaer un get tournament players ids 

            foreach (int p in players)
            {
                
            }

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


        //    public async Task CreateMatchesAsync(List<int> playerIds, int id_round, int id_game)
        //{
        //    if (playerIds.Count % 2 != 0)
        //    {
        //        throw new ArgumentException("La lista de jugadores debe tener una cantidad par de jugadores.");
        //    }

        //    // Dividir la lista de jugadores en pares de player1 y player2
        //    for (int i = 0; i < playerIds.Count; i += 2)
        //    {
        //        int player1 = playerIds[i]; // Primer jugador del par
        //        int player2 = playerIds[i + 1]; // Segundo jugador del par

        //        // Lógica para asignar el ganador si lo tienes (por ejemplo, basándote en tu lógica de ifs)
        //        int winner = 0; // Aquí iría la lógica que defines para obtener el ganador, si corresponde

        //        // Si decides usar una lógica para asignar el ganador de alguna manera, por ejemplo:
        //        if (player1 == /* algún criterio */)
        //        {
        //            winner = player1;
        //        }
        //        else if (player2 == /* otro criterio */)
        //        {
        //            winner = player2;
        //        }

        //        // Consulta SQL para insertar el partido en la tabla T_MATCHES
        //        var query = @"
        //    INSERT INTO T_MATCHES (id_round, id_game, id_player1, id_player2, winner)
        //    VALUES (@id_round, @id_game, @id_player1, @id_player2, @winner);
        //    SELECT LAST_INSERT_ID();";

        //        using (var connection = new MySqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();
        //            using (var transaction = await connection.BeginTransactionAsync())
        //            {
        //                try
        //                {
        //                    // Ejecutar la inserción del partido
        //                    var matchId = await connection.ExecuteScalarAsync<int>(query, new
        //                    {
        //                        id_round,
        //                        id_game,
        //                        id_player1 = player1,
        //                        id_player2 = player2,
        //                        winner
        //                    }, transaction);

        //                    // Si todo fue bien, confirmamos la transacción
        //                    await transaction.CommitAsync();
        //                }
        //                catch (Exception)
        //                {
        //                    // Si hay algún error, se revierte la transacción
        //                    await transaction.RollbackAsync();
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //}









    }
}
}
