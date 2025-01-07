using CTDao.Interfaces.User;
using Dapper;
using DataAccessApp.Models.Card;
using DataAccessApp.Models.Player;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.User
{
    public class UserDao : IUserDao
    {
        private readonly string _connectionString;

        private readonly string QueryGetAllAdmins = "SELECT * FROM T_Admin";
        private readonly string QueryGetAllJudges = "SELECT * FROM T_Judge";
        private readonly string QueryGetAllOrganizers = "SELECT * FROM T_Organizer";
        private readonly string QueryGetAllPlayers = "SELECT * FROM T_Users";

        public UserDao(string connectionString)
        {
            _connectionString = connectionString;
        }


        //public async Task<IEnumerable<AdminModel>> GetAllAdminsAsync()
        //{
        //    using (var connection = new MySqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        var admins = await connection.QueryAsync<AdminModel>(QueryGetAllAdmins);
        //        return admins;
        //    }
        //}

        //public async Task<IEnumerable<JudgeModel>> GetAllJudgesAsync()
        //{
        //    using (var connection = new MySqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        var judges = await connection.QueryAsync<JudgeModel>(QueryGetAllJudges);
        //        return judges;
        //    }
        //}

        //public async Task<IEnumerable<OrganizerModel>> GetAllOrganizersAsync()
        //{
        //    using (var connection = new MySqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        var organizers = await connection.QueryAsync<OrganizerModel>(QueryGetAllOrganizers);
        //        return organizers;
        //    }
        //}

        //public async Task<IEnumerable<PlayerModel>> GetAllPlayersAsync()
        //{
        //    using (var connection = new MySqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        var players = await connection.QueryAsync<PlayerModel>(QueryGetAllPlayers);
        //        return players;
        //    }
        //}
    }
}
