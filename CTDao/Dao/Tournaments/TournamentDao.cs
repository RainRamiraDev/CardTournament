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

        private readonly string QueryGetAll = @" ";

        private readonly string QueryCreateTournament = @"
            INSERT INTO prestamos (id_country, id_organizer, start_datetime, end_datetime, current_phase) 
            VALUES (@id_country,@id_organizer,@start_date,@end_datetime,@current_phase);";


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
                    tournament.Country,
                    tournament.Organizer,
                    tournament.Start_datetime,
                    tournament.End_datetime,
                    tournament.Current_Phase
                });

                return affectedRows;
            }
        }

        public Task<IEnumerable<TournamentModel>> GetAllTournamentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
