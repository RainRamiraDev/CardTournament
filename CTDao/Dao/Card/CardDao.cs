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

        private static readonly object _lockObject = new object();

        public static List<int> createdTournamentSeries { get; set; }

        public static List<int> createdTournamentCards { get; set; }

        public static List<int> createdTournamentCardSeriesIds { get; set; }

        public void StorageTournamentSeries(List<int> series)
        {
            lock (_lockObject)
            {
                createdTournamentSeries = series;
            }
        }

        public void StorageTournamentCardSeriesIds(List<int> CardSeriesIds)
        {
            lock (_lockObject)
            {
                createdTournamentSeries = CardSeriesIds;
            }
        }

        public void StorageTournamentCards(List<int> cards)
        {
            lock (_lockObject)
            {
                createdTournamentCards = cards;
            }
        }

        private readonly string QueryGetIdCardSeries = @"SELECT distinct tcs.id_card_series
        FROM t_card_series tcs
        WHERE tcs.id_card IN @IdCard;";

        private readonly string QueryGetCards = "SELECT s.series_name, c.illustration, c.attack, c.deffense, s.release_date  FROM T_CARDS c JOIN T_CARD_SERIES cs ON c.id_card = cs.id_card JOIN T_SERIES s ON cs.id_series = s.id_series WHERE s.id_series IN @idSeries ORDER BY 1 asc";

        private readonly string QueryGetCardsByIllustration = @"SELECT tc.id_card
        FROM t_cards tc
        WHERE tc.illustration IN @Illustration";

        private readonly string QueryGetSeriesByName = @"SELECT ts.id_series FROM t_series ts WHERE ts.series_name IN @SeriesNames";
        
        private readonly string QueryGetCardsBySeriesName = @"SELECT s.series_name,
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


        public async Task<List<int>> GetIdCardSeriesByCardIdAsync(List<int> cardsId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var cardSeriesIds = await connection.QueryAsync<int>(QueryGetIdCardSeries, new { IdCard = cardsId });

            StorageTournamentCardSeriesIds(cardSeriesIds.ToList());

            return cardSeriesIds.ToList();
        }


        public async Task<IEnumerable<ShowCardsModel>> GetAllAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();   
                var cards = await connection.QueryAsync<ShowCardsModel>(QueryGetCards, new { idseries = createdTournamentSeries });

                return cards;
            }
        }

        public async Task<List<int>> GetCardIdsByIllustrationAsync(List<string> cardsIllustrations)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cardsIds = await connection.QueryAsync<int>(QueryGetCardsByIllustration, new { Illustration = cardsIllustrations });

                StorageTournamentCards(cardsIds.ToList());

                return cardsIds.ToList();
            }
        }

        public async Task<IEnumerable<ShowCardsModel>> GetCardsBySeriesNames(List<string> cardSeries)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cards = await connection.QueryAsync<ShowCardsModel>(QueryGetCardsBySeriesName, new { SeriesName = cardSeries });

                return cards.ToList();
            }
        }

        public async Task<List<int>> GetSeriesIdsByNameAsync(List<string> seriesNames)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var seriesIds = await connection.QueryAsync<int>(QueryGetSeriesByName, new { SeriesNames = seriesNames });

            StorageTournamentSeries(seriesIds.ToList());

            return seriesIds.ToList();
        }

      
    }
}
