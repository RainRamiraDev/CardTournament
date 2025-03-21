﻿using CTDao.Dao.Card;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Game;
using CTDataModels.Tournamets;
using CTDataModels.Users;
using CTDataModels.Users.Judge;
using CTDto.Card;
using Dapper;
using DataAccess;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections;
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
        public TournamentDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string _connectionString;


        public async Task<List<int>> ValidateTournamentPlayersAsync(int tournamentId, int id_player)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournamentPlayers = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryValidateTournamentPlayers"),new {id_tournament = tournamentId, id_player = id_player});

                return tournamentPlayers.ToList();
            }
        }

        public async Task<List<int>> ValidateCountriesFromDbAsync(int id_country)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var countriesIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryVerifyCountriesFromDb") ,new{id_country = id_country});

                return countriesIds.ToList();
            }
        }

        public async Task<int> CreateTournamentAsync(TournamentModel tournament)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var tournamentId = await connection.ExecuteScalarAsync<int>(QueryLoader.GetQuery("QueryCreateTournament"), new
                {
                    IdCountry = tournament.Id_Country,
                    IdOrganizer = tournament.Id_Organizer,
                    StartDatetime = tournament.Start_datetime,
                    End_Datetime = tournament.End_datetime,
                    CurrentPhase = tournament.Current_Phase,
                }, transaction);

                foreach (var judgeId in tournament.Judges_Id)
                {
                    await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertJudges"), new
                    {
                        Id_tournament = tournamentId,
                        Id_Judge = judgeId
                    }, transaction);
                }

                foreach (var cardId in tournament.Series_Id)
                {
                    await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertSeries"), new
                    {
                        Id_tournament = tournamentId,
                        Id_Series = cardId
                    }, transaction);
                }

                await transaction.CommitAsync();
                return tournamentId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var judgeIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetJudgeByAlias"), new { Aliases = judgeAliases });

                return judgeIds.ToList();
            }
        }

        public async Task<IEnumerable<AvailableTournamentsModel>> GetAllAvailableTournamentsAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournaments = await connection.QueryAsync<AvailableTournamentsModel>(QueryLoader.GetQuery("QueryAvailableTournaments"));

                return tournaments;
            }
        }

        public async Task<int> InsertTournamentDecksAsync(TournamentDecksModel tournamentDecks)
        {
            if (tournamentDecks.Id_card_series == null || !tournamentDecks.Id_card_series.Any())
            {
                throw new ArgumentException("La lista de series no puede estar vacía.", nameof(tournamentDecks.Id_card_series));
            }

            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            await using var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var affectedRows = 0;

                foreach (var cardId in tournamentDecks.Id_card_series)
                {
                    affectedRows += await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertDecks"), new
                    {
                        Id_tournament = tournamentDecks.Id_Tournament,
                        Id_Card_Series = cardId,
                        Id_Owner = tournamentDecks.Id_Owner,
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

        public async Task<int> InsertTournamentPlayersAsync(TournamentDecksModel tournamentDecks)
        {
            if (tournamentDecks.Id_Owner == null)
            {
                throw new ArgumentException("La lista de players no puede estar vacía.", nameof(tournamentDecks.Id_Owner));
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var affectedRows = 0;


                        affectedRows += await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertPlayers"), new
                        {
                            Id_tournament = tournamentDecks.Id_Tournament,
                            Id_player = tournamentDecks.Id_Owner
                        }, transaction);

                        await transaction.CommitAsync();

                        return affectedRows;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<int> SetTournamentToNextPhaseAsync(int tournament_id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var id_tournament = tournament_id;

             
                int current_phase = await GetTournamentCurrentPhaseAsync(id_tournament);

                if (current_phase >= 3)
                {
                    throw new InvalidOperationException("El torneo ya está en la fase final (fase 3).");
                }

                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        int rowsAffected = await connection.ExecuteAsync(QueryLoader.GetQuery("QuerySetTournamentNextPhase"),
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

        public async Task<int> GetTournamentCurrentPhaseAsync(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                var current_phase = await connection.QuerySingleOrDefaultAsync<int>(QueryLoader.GetQuery("QueryGetCurrentPhase"),
                                                                                    new { id_tournament = id_tournament });

                if (current_phase == 0)
                {
                    throw new InvalidOperationException("El torneo no existe o no tiene una fase definida.");
                }

                return current_phase;
            }
        }

        public async Task<bool> TournamentExistsAsync(int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                int count = await connection.ExecuteScalarAsync<int>(QueryLoader.GetQuery("QueryTournamentExist"), new { id_tournament = tournamentId });
                return count > 0;
            }
        }

        public async Task<List<int>> GetSeriesFromTournamentAsync(int tournamentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var seriesIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetSeriesFromTournament"), new { id_tournament = tournamentId });

                return seriesIds.ToList();
            }
        }

        public async Task<DateTime> GetTournamentStartDateAsync(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                DateTime? startDateTime = await connection.QuerySingleOrDefaultAsync<DateTime?>(
                    QueryLoader.GetQuery("QueryGetTournamentStartDatetime"),
                    new { id_tournament }
                );

                if (!startDateTime.HasValue)
                {
                    throw new InvalidOperationException("El torneo no existe o no tiene una fecha definida.");
                }

                return startDateTime.Value;
            }
        }

        public async Task<List<int>> GetCardsFromTournamentSeriesAsync(List<int> tournamentSeries, List<int> tournamentCards)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var parameters = new { SeriesIds = tournamentSeries };
                var validCardIds = (await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetCardsFromTournamentSeries"), new { id_series = tournamentSeries, id_card = tournamentCards })).ToList();

                return validCardIds;
            }
        }

        public async Task<List<int>> ValidateUsersFromDbAsync(int id_rol, List<int> id_user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var userIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryValidateUsersFromDb"), new { id_rol = id_rol, id_user = id_user.ToArray() });

                return userIds.ToList();
            }
        }

        public async Task<IEnumerable<TournamentsInformationModel>> GetTournamentsInformationAsync(GetTournamentInformationModel tournamentInformationModel)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournaments = await connection.QueryAsync<TournamentsInformationModel>(QueryLoader.GetQuery("QueryGetTournamentsInformation"),new {current_phase = tournamentInformationModel.Current_phase });

                return tournaments;
            }
        }

        public async Task<List<int>> GetTournamentJudgesAsync(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var judges = await connection.QueryAsync<int>(
                QueryLoader.GetQuery("QueryGetTournamentJudgesIds"),
                new { id_tournament = id_tournament });

                var judgesList = judges.ToList();

                if (judgesList.Count == 0)
                {
                    throw new InvalidOperationException("No se pudieron encontrar los jueces del torneo");
                }

                return judgesList;
            }
        }

        public async Task<TournamentModel> GetTournamentByIdAsync(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournament = await connection.QuerySingleOrDefaultAsync<TournamentModel>(
                    QueryLoader.GetQuery("QueryGetTournamentById"),
                    new { Id_tournament = id_tournament });

                if (tournament == null)
                {
                    throw new InvalidOperationException("Torneo no encontrado.");
                }

                return tournament;
            }
        }

        public async Task<List<int>> ValidateAvailableTournamentsAsync(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var tournaments = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryVerifyTournament"), new { id_tournament = id_tournament });

                return tournaments.ToList();
            }
        }

        public async Task<bool> CheckTournamentCapacity(PlayerCapacityModel capacity, int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                int currentPlayerCount = await connection.ExecuteScalarAsync<int>(
                    QueryLoader.GetQuery("QueryCheckTournamentCapacity"),
                    new { id_tournament }
                );

                if (currentPlayerCount < capacity.MinPlayers) return false;
                if (currentPlayerCount > capacity.MaxPlayers) return false;
            }

            return true;
        }

        public async Task AlterTournamentAsync(AlterTournamentModel tournament)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var rowsAffected = await connection.ExecuteAsync(QueryLoader.GetQuery("QueryUpdateTournament"), new
                {
                    Id_Country = tournament.Id_Country,
                    Id_Organizer = tournament.Id_Organizer,
                    Start_Datetime = tournament.Start_datetime,
                    End_Datetime = tournament.End_datetime,
                    Id_tournament = tournament.Id_tournament
                }, transaction);

                if (rowsAffected == 0)
                {
                    await transaction.RollbackAsync();
                }

                await connection.ExecuteAsync(QueryLoader.GetQuery("QueryDeleteJudges"), new { Id_tournament = tournament.Id_tournament }, transaction);
                foreach (var judgeId in tournament.Judges_Id)
                {
                    await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertJudges"), new
                    {
                        Id_tournament = tournament.Id_tournament,
                        Id_Judge = judgeId
                    }, transaction);
                }

                await connection.ExecuteAsync(QueryLoader.GetQuery("QueryDeleteSeries"), new { Id_tournament = tournament.Id_tournament }, transaction);
                foreach (var cardId in tournament.Series_Id)
                {
                    await connection.ExecuteAsync(QueryLoader.GetQuery("QueryInsertSeries"), new
                    {
                        Id_tournament = tournament.Id_tournament,
                        Id_Series = cardId
                    }, transaction);
                }
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SoftDeleteTournamentAsync(int id_tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var affectedRows = await connection.ExecuteAsync(QueryLoader.GetQuery("QuerySoftDeleteTournament"), new
                {
                    id_tournament = id_tournament
                });
            }
        }

        public async Task DisqualifyPlayerFromTournamentAsync(DisqualificationModel disqualificationRequest)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var affectedRows = await connection.ExecuteAsync(QueryLoader.GetQuery("QueryDisqualifyPlayer"), new
                {
                    id_tournament = disqualificationRequest.Id_Tournament,
                    id_player = disqualificationRequest.Id_Player,
                    id_judge = disqualificationRequest.Id_Judge,
                });
            }
        }


        public async Task<bool> ValidateJudgesFromTournament(int id_Judge, int id_Tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.QueryFirstOrDefaultAsync<int?>(
                    QueryLoader.GetQuery("QueryValidateJudgesFromTournament"),
                    new { id_Judge, id_Tournament }
                );

                return result.HasValue;
            }
        }

        public async Task<List<ShowTournamentPlayersModel>> ShowPlayersFromTournamentAsync(int tournament_Id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var judges = await connection.QueryAsync<ShowTournamentPlayersModel>(
                QueryLoader.GetQuery("QueryGetPlayersFromTournament"),
                new { id_tournament = tournament_Id });

                var playerList = judges.ToList();

                if (playerList.Count == 0)
                {
                    throw new InvalidOperationException("No se pudieron encontrar los jugadores del torneo");
                }

                return playerList;
            }
        }
    }
}
 
  

