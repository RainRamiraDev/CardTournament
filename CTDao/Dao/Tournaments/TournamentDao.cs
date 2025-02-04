using CTDao.Dao.Card;
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.Tournaments
{
    public class TournamentDao : ITournamentDao
    {
        private static readonly object _lockObject = new object();

        public static int createdtournamentId { get; set; }

        public static int createdtournamentOrganizer { get; set; }

        public static List<int> createdtournamentPlayers { get; set; }

        public void StorageTournamentPlayersId(List<int> ids)
        {
            lock (_lockObject)
            {
                createdtournamentPlayers = ids;
            }
        }

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
        INSERT INTO T_TOURNAMENTS (id_country, id_organizer, start_datetime,current_phase) 
        VALUES (@IdCountry, @IdOrganizer, @StartDatetime,@CurrentPhase); SELECT LAST_INSERT_ID();";

        private readonly string QueryInsertJudges = @"INSERT INTO T_TOURN_JUDGES (id_tournament, id_judge) VALUES (@id_tournament, @id_judge);";

        private readonly string QueryGetJudgeByAlias = @"SELECT id_user FROM T_USERS WHERE alias IN @Aliases AND Id_rol = 3;";

        private readonly string QueryAvailableTournaments = @"
        SELECT 
            t.start_datetime,
            t.end_datetime,
            c.country_name AS tournament_country,
            u.alias AS organizer_alias,
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
        LEFT JOIN T_MATCHES m ON g.id_game = m.id_game 
        LEFT JOIN T_ROUNDS r ON m.id_round = r.id_round 
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

        private readonly string QueryInsertPlayers = @"INSERT INTO T_TOURN_PLAYERS (id_tournament, id_player) VALUES (@Id_tournament,@id_player);";

        private readonly string QuerySetTournamentNextPhase = @"UPDATE T_TOURNAMENTS
        SET current_phase = current_phase + 1
        WHERE id_tournament = @id_tournament AND current_phase < 3";


        private readonly string QueryGetCurrentPhase = @"SELECT current_phase FROM T_TOURNAMENTS WHERE id_tournament = @id_tournament;";


        public async Task<int> CreateTournamentAsync(TournamentModel tournament)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var tournamentId = await connection.ExecuteScalarAsync<int>(QueryCreateTournament, new
                {
                    IdCountry = tournament.Id_Country,
                    IdOrganizer = tournament.Id_Organizer,
                    StartDatetime = tournament.Start_datetime,
                    CurrentPhase = tournament.Current_Phase
                }, transaction);

                Console.WriteLine($"Tournament ID: {tournamentId}");

                foreach (var judgeId in tournament.Judges)
                {
                    Console.WriteLine($"Inserting Judge {judgeId} into tournament {tournamentId}");

                    await connection.ExecuteAsync(QueryInsertJudges, new
                    {
                        Id_tournament = tournamentId,
                        Id_Judge = judgeId
                    }, transaction);
                }

                foreach (var cardId in tournament.Series_name)
                {

                    Console.WriteLine($"Inserting Series {cardId} into tournament {tournamentId}");

                    await connection.ExecuteAsync(QueryInsertSeries, new
                    {
                        Id_tournament = tournamentId,
                        Id_Series = cardId
                    }, transaction);
                }

                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed successfully.");



                return tournamentId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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
                        Id_Card_Series = cardId,
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

        public async Task<int> InsertTournamentPlayersAsync(int player)
        {
            if (player == null)
            {
                throw new ArgumentException("La lista de players no puede estar vacía.", nameof(player));
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var affectedRows = 0;


                        affectedRows += await connection.ExecuteAsync(QueryInsertPlayers, new
                        {
                            Id_tournament = createdtournamentId,
                            Id_player = player
                        }, transaction);

                        await transaction.CommitAsync();

                        return affectedRows;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error al insertar players en el torneo: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public async Task<int> SetTournamentToNextPhase()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var id_tournament = createdtournamentId;

             
                int current_phase = await GetTournamentCurrentPhase(id_tournament);

                if (current_phase >= 3)
                {
                    throw new InvalidOperationException("El torneo ya está en la fase final (fase 3).");
                }

                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int rowsAffected = await connection.ExecuteAsync(QuerySetTournamentNextPhase,
                                                                         new { id_tournament = id_tournament },
                                                                         transaction);

                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException("No se pudo actualizar la fase del torneo, podría estar ya en la fase 3.");
                        }

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

        public async Task<int> GetTournamentCurrentPhase(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                var current_phase = await connection.QuerySingleOrDefaultAsync<int>(QueryGetCurrentPhase,
                                                                                    new { id_tournament = id_tournament });

                Console.WriteLine("[Creation Current Phase]:"+current_phase);

                if (current_phase == 0)
                {
                    throw new InvalidOperationException("El torneo no existe o no tiene una fase definida.");
                }

                return current_phase;
            }
        }
    }
}
 
  

