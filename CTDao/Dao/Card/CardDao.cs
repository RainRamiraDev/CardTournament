using CTDao.Interfaces.Card;
using Dapper;
using DataAccessApp.Models.Card;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.Card
{
    public class CardDao : ICardDao
    {
        private readonly string _connectionString;

        private readonly string QueryGetAll = "";

        public CardDao(string connectionString)
        {
            _connectionString = connectionString;
        }

    }
}
