using CTDao.Interfaces.Card;
using CTDataModels.Card;
using Dapper;
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

        private readonly string QueryGetAll = "SELECT * FROM T_Cards";

        public CardDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<CardModel>> GetAllAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cards = await connection.QueryAsync<CardModel>(QueryGetAll);
                return cards;
            }
        }
    }
}
