using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.Tournaments
{
    public class TournamentDao : ITournamentDao
    {

        private readonly string _connectionString;

        private readonly string QueryGetAll = @"SELECT * FROM T_TOURNAMENTS";

        private readonly string QueryCreateTournament = @"
    INSERT INTO T_TOURNAMENTS (id_country, id_organizer, start_datetime, end_datetime, current_phase) 
    VALUES (@IdCountry, @IdOrganizer, @StartDatetime, @EndDatetime, @CurrentPhase);";



        public TournamentDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateTournamentAsync(TournamentModel tournament)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var affectedRows = await connection.ExecuteAsync(QueryCreateTournament, new
                {
                    IdCountry = tournament.Id_Country,  // Debe ser INT
                    IdOrganizer = tournament.Id_Organizer,  // Debe ser INT
                    StartDatetime = tournament.Start_datetime,  // DATETIME
                    EndDatetime = tournament.End_datetime,  // DATETIME
                    CurrentPhase = tournament.Current_Phase  // VARCHAR(20)
                });


                return affectedRows;
            }
        }

        public async Task<IEnumerable<TournamentModel>> GetAllTournamentAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var prestamos = await connection.QueryAsync<TournamentModel>(QueryGetAll);

                return prestamos;
            }
        }
    }
}
