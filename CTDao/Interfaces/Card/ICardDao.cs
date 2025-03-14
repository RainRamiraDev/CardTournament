
using CTDataModels.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Interfaces.Card
{
    public interface ICardDao
    {
        Task<IEnumerable<ShowCardsModel>> GetAllCardsFromTournamentAsync(List<int>Series);
        Task<List<int>> GetCardIdsByIllustrationAsync(List<string> cardsIllustrations);         
        Task<IEnumerable<ShowCardsModel>> GetCardsBySeriesNamesAsync(List<string> cardSeries);     
        Task<List<int>> GetSeriesIdsByNameAsync(List<string> seriesNames);
        Task<List<int>> GetIdCardSeriesByCardIdAsync(List<int> cardsId);

        Task<List<int>> ValidateSeriesAsync(List<int>id_series);
        Task<List<string>> GetCardIllustrationByIdAsync(List<int> cardsIds);
        Task<IEnumerable<SeriesListModel>> GetAllSeriesNamesAsync();


    }
}
