using CTDao.Interfaces.User;
using Dapper;
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

    }
}
