﻿using CTDao.Dao.Card;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using Dapper;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.Tournaments
{
    public class TournamentDao : ITournamentDao
    {
        private static readonly object _lockObject = new object();

        public static int createdtournamentId { get; set; }

        public static int createdtournamentOrganizer { get; set; }

        public void StorageTournamentId(int id)
        {
            lock (_lockObject)
            {
                createdtournamentId = id;
            }
        }

        public void StorageTournamentOrganizer(int id)
        {
            lock (_lockObject)
            {
                createdtournamentOrganizer = id;
            }
        }

        public TournamentDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string _connectionString;

        
        private readonly string QueryGetAll = @"SELECT * FROM T_TOURNAMENTS";

        private readonly string QueryCreateTournament = @"
        INSERT INTO T_TOURNAMENTS (id_country, id_organizer, start_datetime, end_datetime, current_phase) 
        VALUES (@IdCountry, @IdOrganizer, @StartDatetime, @EndDatetime, @CurrentPhase); SELECT LAST_INSERT_ID();";

        private readonly string QueryInsertJudges = @"INSERT INTO T_TOURN_JUDGES (id_tournament, id_judge) VALUES (@id_tournament, @id_judge);";

        private readonly string QueryGetJudgeByAlias = @"SELECT id_user FROM T_USERS WHERE alias IN @Aliases AND Id_rol = 3;";

        private readonly string QueryAvailableTournaments = @"
SELECT 
    t.start_datetime,
    t.end_datetime,
    t.current_phase,
    c.country_name AS tournament_country,
    u.fullname AS organizer_name,
    u.alias AS organizer_alias,
    u.email AS organizer_email,
    GROUP_CONCAT(DISTINCT j.fullname SEPARATOR ', ') AS judges,
    GROUP_CONCAT(DISTINCT s.series_name SEPARATOR ', ') AS series_played,
    GROUP_CONCAT(DISTINCT p.fullname SEPARATOR ', ') AS players,
    GROUP_CONCAT(DISTINCT d.fullname SEPARATOR ', ') AS disqualified_players,
    COUNT(DISTINCT g.id_game) AS total_games,
    COUNT(DISTINCT r.id_round) AS total_rounds
FROM T_TOURNAMENTS t
LEFT JOIN T_COUNTRIES c ON t.id_country = c.id_country
LEFT JOIN T_USERS u ON t.id_organizer = u.id_user
LEFT JOIN T_TOURN_JUDGES tj ON t.id_tournament = tj.id_tournament
LEFT JOIN T_USERS j ON tj.id_judge = j.id_user
LEFT JOIN T_TOURN_SERIES ts ON t.id_tournament = ts.id_tournament
LEFT JOIN T_SERIES s ON ts.id_series = s.id_series
LEFT JOIN T_TOURN_PLAYERS tp ON t.id_tournament = tp.id_tournament
LEFT JOIN T_USERS p ON tp.id_player = p.id_user
LEFT JOIN T_TOURN_DISQUALIFICATIONS td ON t.id_tournament = td.id_tournament
LEFT JOIN T_USERS d ON td.id_player = d.id_user
LEFT JOIN T_GAMES g ON t.id_tournament = g.id_tournament
LEFT JOIN T_ROUNDS r ON g.id_round = r.id_round
WHERE t.current_phase = 1
GROUP BY 
    t.start_datetime,
    t.end_datetime,
    t.current_phase,
    c.country_name,
    u.fullname, 
    u.alias, 
    u.email;
";

        private readonly string QueryInsertSeries = @"INSERT INTO T_TOURN_SERIES (id_tournament, id_series) VALUES (@id_tournament, @id_series);";

        private readonly string QueryInsertDecks = @"INSERT INTO T_TOURN_DECKS (id_tournament, id_card_series, id_owner) 
                                             VALUES (@id_tournament, @id_card_series, @id_owner);";


        public async Task<int> CreateTournamentAsync(TournamentModel tournament)
        {

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var tournamentId = await connection.ExecuteScalarAsync<int>(QueryCreateTournament, new
                        {
                            IdCountry = tournament.Id_Country,
                            IdOrganizer = tournament.Id_Organizer,
                            StartDatetime = tournament.Start_datetime,
                            EndDatetime = tournament.End_datetime,
                            CurrentPhase = tournament.Current_Phase
                        }, transaction);
   
                        await transaction.CommitAsync();

                        StorageTournamentId(tournamentId);
                        StorageTournamentOrganizer(tournament.Id_Organizer);

                        return tournamentId;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }

        }

        public async Task<IEnumerable<TournamentModel>> GetAllTournamentAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournaments = await connection.QueryAsync<TournamentModel>(QueryGetAll);

                return tournaments;
            }
        }

        public async Task<int> InsertTournamentJudgesAsync(List<int> judgeIds)
        {
            if (judgeIds == null || !judgeIds.Any())
            {
                throw new ArgumentException("La lista de jueces no puede estar vacía.", nameof(judgeIds));
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var affectedRows = 0;

                        foreach (var judgeId in judgeIds)
                        {
                            affectedRows += await connection.ExecuteAsync(QueryInsertJudges, new
                            {
                                Id_tournament = createdtournamentId,
                                Id_Judge = judgeId
                            }, transaction);
                        }

                        await transaction.CommitAsync();
                        return affectedRows;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error al insertar jueces en el torneo: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var judgeIds = await connection.QueryAsync<int>(QueryGetJudgeByAlias, new { Aliases = judgeAliases });

                return judgeIds.ToList();
            }
        }

        public async Task<IEnumerable<AvailableTournamentsModel>> GetAllAvailableTournamentsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournaments = await connection.QueryAsync<AvailableTournamentsModel>(QueryAvailableTournaments);

                return tournaments;
            }
        }

        public async Task<int> InsertTournamentSeriesAsync(List<int> cardsIds)
        {
            if (cardsIds == null || !cardsIds.Any())
            {
                throw new ArgumentException("La lista de series no puede estar vacía.", nameof(cardsIds));
            }

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            await using var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var affectedRows = 0;

                foreach (var cardId in cardsIds)
                {
                    affectedRows += await connection.ExecuteAsync(QueryInsertSeries, new
                    {
                        Id_tournament = createdtournamentId,
                        Id_Series = cardId
                    }, transaction).ConfigureAwait(false);
                }

                await transaction.CommitAsync().ConfigureAwait(false);
                return affectedRows;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("Error al insertar series en el torneo.", ex);
            }
        }

        public async Task<int> InsertTournamentDecksAsync(List<int> cardsIds, int owner)
        {
            if (cardsIds == null || !cardsIds.Any())
            {
                throw new ArgumentException("La lista de series no puede estar vacía.", nameof(cardsIds));
            }

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            await using var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var affectedRows = 0;

                foreach (var cardId in cardsIds)
                {
                    affectedRows += await connection.ExecuteAsync(QueryInsertDecks, new
                    {
                        Id_tournament = createdtournamentId,
                        Id_Card_Series = cardId,  // Correcto
                        Id_Owner = owner
                    }, transaction).ConfigureAwait(false);
                }

                await transaction.CommitAsync().ConfigureAwait(false);
                return affectedRows;
            }


            catch (Exception ex)
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("Error al insertar series en el torneo.", ex);

            }
        }
}
    }

