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

        private readonly string QueryGetAll = "SELECT s.series_name, c.illustration, c.attack, c.deffense, s.release_date FROM T_CARDS c JOIN T_CARD_SERIES cs ON c.id_card = cs.id_card JOIN T_SERIES s ON cs.id_series = s.id_series ORDER BY 1 asc";

        public CardDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ShowCardsModel>> GetAllAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cards = await connection.QueryAsync<ShowCardsModel>(QueryGetAll);
                return cards;
            }
        }
    }
}
