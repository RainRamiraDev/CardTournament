﻿using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Game;
using CTDataModels.Game;
using Dapper;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CTDao.Dao.Game
{
    public class GameDao : IGameDao
    {
        private static readonly object _lockObject = new object();

        public static int createdGameId { get; set; }

        public static int createdRoundId { get; set; }

        public void StorageGameId(int id)
        {
            lock (_lockObject)
            {
                createdGameId = id;
            }
        }

        public void StorageRoundId(int id)
        {
            lock (_lockObject)
            {
                createdRoundId = id;
            }
        }

        private readonly string _connectionString;

        public GameDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string QueryCreateGame = @"INSERT INTO T_GAMES (id_tournament,start_datetime) VALUES (@id_tournament,@start_datetime); SELECT LAST_INSERT_ID();";

        private readonly string QueryInsertGamePlayers = @"INSERT INTO T_GAME_PLAYERS (id_game, id_player) VALUES (@id_game, @id_player);";


        private readonly string QuerySetWinner = @"
        UPDATE T_USERS 
        SET games_won = games_won + 1 
        WHERE id_user = @id_player;
    ";

        private readonly string QuerySetLosers = @"
            UPDATE T_USERS 
            SET games_lost = games_lost + 1 
            WHERE id_user IN @losers;
        ";


        private readonly string QueryGetPlayersIds = @"SELECT id_player FROM T_TOURN_PLAYERS WHERE id_tournament = @id_tournament";

        private readonly string QueryCreateRound = @"
            INSERT INTO T_ROUNDS (id_tournament,round_number) 
            VALUES (@id_tournament,@round_number);
        ";

        private readonly string QueryCreateMatch = @"INSERT INTO T_MATCHES (id_round, id_game, id_player1, id_player2, winner) 
        VALUES (@id_round, @id_game, @id_player1, @id_player2,@winner);
        SELECT LAST_INSERT_ID();
        ";

        private readonly string QuerySetNextRound = @"
        INSERT INTO T_ROUNDS (id_tournament, round_number, is_completed) VALUES (@id_tournament, @round_number, FALSE)";

        private readonly string QueryGetLastRound = @"
        SELECT COALESCE(MAX(round_number), 0) FROM T_ROUNDS WHERE id_tournament = @id_tournament";

        
        private readonly string QueryCompleteRound = @"
         UPDATE T_ROUNDS
         SET is_completed = TRUE
         WHERE id_tournament = @id_tournament AND
	  	 round_number = @round_number;
    ";

        private readonly string QueryGetLastInsertId = "SELECT LAST_INSERT_ID();";

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
                        int rowsAffected = await connection.ExecuteAsync(QuerySetWinner, new { id_player = winner }, transaction);

                        await transaction.CommitAsync();
                        return rowsAffected;
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

        public async Task<int> SetGameLoserAsync(List<int> losers)
        {
            if (losers == null || losers.Count == 0)
            {
                throw new ArgumentException("La lista de perdedores no puede estar vacía.");
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int rowsAffected = await connection.ExecuteAsync(QuerySetLosers, new { losers }, transaction);

                        await transaction.CommitAsync();
                        return rowsAffected;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> GetLastRoundAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int lastRoundNumber = await connection.ExecuteScalarAsync<int>(
                    QueryGetLastRound,
                    new { id_tournament = TournamentDao.createdtournamentId });
                int nextRoundNumber = lastRoundNumber + 1;
                return nextRoundNumber;
            }
        }

        public async Task<int> CreateRoundAsync(RoundModel round)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await connection.ExecuteAsync(QueryCreateRound, new
                        {
                            Id_Tournament = TournamentDao.createdtournamentId,
                            round_number = round.Round_Number
                        }, transaction);

                        int roundId = await connection.ExecuteScalarAsync<int>(QueryGetLastInsertId, transaction);

                        await transaction.CommitAsync();

                        StorageRoundId(roundId);

                        return roundId;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> CreateMatchAsync(MatchModel match)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var matchId = await connection.ExecuteScalarAsync<int>(QueryCreateMatch, new
                        {
                            id_round = match.Id_Round,
                            id_game = GameDao.createdGameId,
                            id_player1 = match.Id_Player1,
                            id_player2 = match.Id_Player2,
                            winner = match.Winner

                        }, transaction);

                        await transaction.CommitAsync();
                        return matchId;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> SetNextRoundAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int lastRoundNumber = await GetLastRoundAsync();
                        Console.WriteLine("[Next Round number]:" + lastRoundNumber);

                        int affectedRows = await connection.ExecuteAsync(
                            QuerySetNextRound,
                            new { id_tournament = TournamentDao.createdtournamentId, round_number = lastRoundNumber },
                            transaction);

                        await transaction.CommitAsync();
                        return affectedRows;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> SetRoundCompletedAsync(int roundNumber)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int affectedRows = await connection.ExecuteAsync(QueryCompleteRound, new
                        {
                            id_tournament = TournamentDao.createdtournamentId,
                            round_number = roundNumber
                        }, transaction: transaction);


                        Console.WriteLine("[Completed Round Number]:"+roundNumber);

                        await transaction.CommitAsync();
                        return affectedRows;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }


    }
}

