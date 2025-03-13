using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Game;
using CTDataModels.Game;
using Dapper;
using DataAccess;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections;
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

        private readonly string _connectionString;

        public GameDao(string connectionString)
        {
            _connectionString = connectionString;
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
                        int rowsAffected = await connection.ExecuteAsync(QueryLoader.GetQuery("QuerySetWinner"), new { id_player = winner }, transaction);

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

        public async Task<List<int>> GetTournamentPlayersAsync(int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var playersIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetPlayersIds"), new { id_tournament = tournamentId });

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
                        int rowsAffected = await connection.ExecuteAsync(QueryLoader.GetQuery("QuerySetLosers"), new { losers }, transaction);

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

        public async Task<int> GetLastRoundAsync(int tournament_id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int lastRoundNumber = await connection.ExecuteScalarAsync<int>(
                    QueryLoader.GetQuery("QueryGetLastRound"),
                    new { id_tournament = tournament_id });
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
                        await connection.ExecuteAsync(QueryLoader.GetQuery("QueryCreateRound"), new
                        {
                            Id_Tournament = round.Id_Tournament,
                            round_number = round.Round_Number,
                            judge = round.Judge,
                        }, transaction);

                        int roundId = await connection.ExecuteScalarAsync<int>(QueryLoader.GetQuery("QueryGetLastInsertId"), transaction);

                        await transaction.CommitAsync();

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
                        var matchId = await connection.ExecuteScalarAsync<int>(QueryLoader.GetQuery("QueryCreateMatch"), new
                        {
                            id_round = match.Id_Round,
                            id_player1 = match.Id_Player1,
                            id_player2 = match.Id_Player2,
                            winner = match.Winner,
                            Match_Date = match.Match_Date,
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

        public async Task<int> SetNextRoundAsync(int tournament_id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int lastRoundNumber = await GetLastRoundAsync(tournament_id);
                        int affectedRows = await connection.ExecuteAsync(
                            QueryLoader.GetQuery("QuerySetNextRound"),
                            new { id_tournament = tournament_id, round_number = lastRoundNumber },
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

        public async Task<int> SetRoundCompletedAsync(int roundNumber, int tournament_id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int affectedRows = await connection.ExecuteAsync(QueryLoader.GetQuery("QueryCompleteRound"), new
                        {
                            id_tournament = tournament_id,
                            round_number = roundNumber
                        }, transaction: transaction);

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

        public async Task InsertDisqualificationAsync(int playerId, int tournamentId, int judgeId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertDisqualification"), new
                        {
                            Id_Tournament = tournamentId,
                            Id_Player = playerId,
                            Id_Judge = judgeId,
                        }, transaction);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new InvalidOperationException("Error al registrar la descalificación", ex);
                    }
                }
            }
        }


        public async Task<bool> IsPlayerDisqualifiedAsync(int playerId, int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    string query = QueryLoader.GetQuery("QueryCheckPlayerDisqualification");
                    var disqualified = await connection.ExecuteScalarAsync<bool>(query, new
                    {
                        Id_Player = playerId,
                        Id_Tournament = tournamentId
                    });

                    return disqualified;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error al verificar la descalificación del jugador", ex);
                }
            }
        }

    }
}

