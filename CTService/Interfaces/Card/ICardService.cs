using CTDataModels.Card;
using CTDto.Card;
using CTDto.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Interfaces.Card
{
    public interface ICardService
    {
        Task<IEnumerable<ShowCardsDto>> GetAllCardsAsync(TournamentRequestToResolveDto tournamentRequestDto);
        Task<List<int>> GetCardsIdsByIllustrationAsync(List<string> cardsIllustrations);
        Task<List<int>> GetSeriesIdsByNameAsync(List<string> names);
        Task<IEnumerable<ShowCardsDto>> GetCardsBySeriesNamesAsync(List<string> cardSeries);

        Task<IEnumerable<SeriesListDto>> GetAllSeriesAsync();

    }
}
