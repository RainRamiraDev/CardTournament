
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
        Task<IEnumerable<ShowCardsModel>> GetAllAsync(List<int>Series);
        Task<List<int>> GetCardIdsByIllustrationAsync(List<string> cardsIllustrations);          //elegir las cartas limitadas por la serie buscadas por la ilustracion
        Task<IEnumerable<ShowCardsModel>> GetCardsBySeriesNames(List<string> cardSeries);           //trae las cartas filtradas por las series elegidas
        Task<List<int>> GetSeriesIdsByNameAsync(List<string> seriesNames);
        Task<List<int>> GetIdCardSeriesByCardIdAsync(List<int> cardsId);

        Task<List<string>> GetCardIllustrationById(List<int> cardsIds);


    }
}
