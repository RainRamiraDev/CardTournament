using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Game;
using CTDataModels.Game;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.Game
{
    public class GameDao : IGameDao
    {
        private static readonly object _lockObject = new object();

        public static int createdGameId { get; set; }

        public void StorageGameId(int id)
        {
            lock (_lockObject)
            {
                createdGameId = id;
            }
        }

        private readonly string _connectionString;

        public GameDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string QueryCreateGame = @"INSERT INTO T_GAMES (id_tournament,start_datetime) VALUES (@id_tournament,@start_datetime); SELECT LAST_INSERT_ID();";

        private readonly string QueryInsertGamePlayers = @"INSERT INTO T_GAME_PLAYERS (id_game, id_player) VALUES (@id_game, @id_player);";

        private readonly string QuerySetWinner = @"UPDATE T_GAME_PLAYERS 
        SET is_winner = TRUE 
        WHERE id_game_players = @id_game_players;
        ";

        private readonly string QueryGetPlayersIds = @"SELECT id_player FROM T_TOURN_PLAYERS WHERE id_tournament = @id_tournament";
        public async Task<int> CreateGameAsync(GameModel game)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var gameId = await connection.ExecuteScalarAsync<int>(QueryCreateGame, new
                        {
                            Id_Tournament = TournamentDao.createdtournamentId,
                            start_datetime = DateTime.Now,
                        }, transaction);

                        await transaction.CommitAsync();

                        Console.WriteLine("game id = "+gameId);

                        StorageGameId(gameId);

                        return gameId;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> InsertGamePlayersAsync(GamePlayersModel playerModel)
        {
            if (playerModel == null || !playerModel.Id_Player.Any())
            {
                throw new ArgumentException("La lista de judadores no puede estar vacía.", nameof(playerModel));
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var affectedRows = 0;

                        foreach (var player in playerModel.Id_Player)
                        {
                            affectedRows += await connection.ExecuteAsync(QueryInsertGamePlayers, new
                            {
                                Id_game = createdGameId,
                                Id_player = player
                            }, transaction);
                        }

                        await transaction.CommitAsync();
                        return affectedRows;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error al insertar jugadores en el juego: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<int> SetGameWinnerAsync(int winner)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var winnerId = await connection.ExecuteScalarAsync<int>(QuerySetWinner, new
                        {
                            id_game_players = winner,

                        }, transaction);

                        await transaction.CommitAsync();

                        return winnerId;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<List<int>> GetTournamentPlayers(int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var playersIds = await connection.QueryAsync<int>(QueryGetPlayersIds, new { id_tournament = tournamentId });

                return playersIds.ToList();
            }
        }
    }
 }

