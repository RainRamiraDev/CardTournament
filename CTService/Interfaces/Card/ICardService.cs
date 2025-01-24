using CTDataModels.Card;
using CTDto.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Interfaces.Card
{
    public interface ICardService
    {
        Task<IEnumerable<ShowCardsDto>> GetAllCardsAsync();
        Task<List<int>> GetCardsIdsByIllustrationAsync(List<string> cardsIllustrations);

        Task<List<int>> GetSeriesIdsByNameAsync(List<string> names);

        Task<IEnumerable<ShowCardsDto>> GetCardsBySeriesNames(List<string> cardSeries);


    }
}
