using CTDao.Interfaces.Card;
using CTDataModels.Card;
using Dapper;
using DataAccess;
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

        public CardDao(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<List<int>> GetAllSeries()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var seriesIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetAllSeries"));

                return seriesIds.ToList();
            }
        }

        public async Task<List<int>> GetIdCardSeriesByCardIdAsync(List<int> cardsId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var cardSeriesIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetIdCardSeries"), new { IdCard = cardsId });

            return cardSeriesIds.ToList();
        }

        public async Task<IEnumerable<ShowCardsModel>> GetAllAsync(List<int> Series)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();   
                var cards = await connection.QueryAsync<ShowCardsModel>(QueryLoader.GetQuery("QueryGetCards"), new { idseries = Series });

                return cards;
            }
        }

        public async Task<List<int>> GetCardIdsByIllustrationAsync(List<string> cardsIllustrations)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cardsIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetCardsByIllustration"), new { Illustration = cardsIllustrations });


                Console.WriteLine("Ids de las cards "+string.Join(cardsIds.ToString()));

                return cardsIds.ToList();
            }
        }

        public async Task<IEnumerable<ShowCardsModel>> GetCardsBySeriesNames(List<string> cardSeries)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cards = await connection.QueryAsync<ShowCardsModel>(QueryLoader.GetQuery("QueryGetCardsBySeriesName"), new { SeriesName = cardSeries });

                return cards.ToList();
            }
        }

        public async Task<List<int>> GetSeriesIdsByNameAsync(List<string> seriesNames)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var seriesIds = await connection.QueryAsync<int>(QueryLoader.GetQuery("QueryGetSeriesByName"), new { SeriesNames = seriesNames });

            return seriesIds.ToList();
        }

        public async Task<List<string>> GetCardIllustrationById(List<int>id_card)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var illustrations = await connection.QueryAsync<string>(
                  QueryLoader.GetQuery("QueryGetCardIllustrationById"),
                  new { id_card = id_card }
                  );


                return illustrations.ToList();
            }
        }



    }
}
