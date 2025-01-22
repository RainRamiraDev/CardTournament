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

        private readonly string QueryGetCardsByIllustration = @"SELECT tc.id_card,
		tc.illustration,
		tc.attack,
		tc.deffense
        FROM t_cards tc
        WHERE tc.illustration = @Illustration";

        private readonly string QueryGetCardsBySeriesNames = @"SELECT s.series_name,
        c.illustration,
        c.attack,
        c.deffense,
        s.release_date 
        FROM T_CARDS c 
	        JOIN T_CARD_SERIES cs ON c.id_card = cs.id_card 
	        JOIN T_SERIES s ON cs.id_series = s.id_series 
        WHERE s.series_name IN @SeriesName
        ORDER BY 1 asc";
        
        
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

        public async Task<List<int>> GetCardIdsByIllustrationAsync(List<string> cardsIllustrations)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cardsIds = await connection.QueryAsync<int>(QueryGetCardsByIllustration, new { Illustration = cardsIllustrations });

                return cardsIds.ToList();
            }
        }

        public async Task<IEnumerable<ShowCardsModel>> GetCardsBySeriesNames(List<string> cardSeries)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cards = await connection.QueryAsync<ShowCardsModel>(QueryGetCardsBySeriesNames, new { SeriesName = cardSeries });

                return cards.ToList();
            }
        }
    }
}
