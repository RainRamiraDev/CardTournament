using CTDao.Interfaces.Card;
using CTDao.Interfaces.Tournaments;
using CTDto.Card;
using CTDto.Tournaments;
using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.Card
{
    public class CardService : ICardService
    {
        private readonly ICardDao _cardDao;

        private readonly ITournamentDao _tournamentDao;

        public CardService(ICardDao cardDao, ITournamentDao tournamentDao)
        {
            _cardDao = cardDao;
            _tournamentDao = tournamentDao;
        }


        public async Task<IEnumerable<ShowCardsDto>> GetAllCardsAsync(TournamentRequestToResolveDto tournamentRequestDto)
        {

            var seriesIds = await _tournamentDao.GetSeriesFromTournamentAsync(tournamentRequestDto.Tournament_Id);

            if (seriesIds.Count < 1)
            {
                throw new ArgumentException("Las series no pueden estar vacias");
            }

            var cards = await _cardDao.GetAllAsync(seriesIds);
            var cardDtos = cards.Select(card => new ShowCardsDto
            {
                Series_name = card.Series_name,
                Illustration = card.Illustration,
                Attack = card.Attack,
                Deffense = card.Deffense,
                Release_Date = card.Release_Date,
            }).ToList();

            return cardDtos;
        }

        public async Task<IEnumerable<ShowCardsDto>> GetCardsBySeriesNamesAsync(List<string> cardSeries)
        {
            if (cardSeries == null || !cardSeries.Any())
            {
                return Enumerable.Empty<ShowCardsDto>();
            }

            var cards = await _cardDao.GetCardsBySeriesNamesAsync(cardSeries);

            return cards.Select(card => new ShowCardsDto
            {
                Series_name = card.Series_name,
                Illustration = card.Illustration,
                Attack = card.Attack,
                Deffense = card.Deffense,
                Release_Date = card.Release_Date,
            });
        }

        public async Task<List<int>> GetCardsIdsByIllustrationAsync(List<string> cardsIllustrations)
        {
            if (cardsIllustrations == null || !cardsIllustrations.Any())
            {
                throw new ArgumentException("La lista de cartas no puede estar vacia.", nameof(cardsIllustrations));
            }

            return await _cardDao.GetCardIdsByIllustrationAsync(cardsIllustrations);
        }

        public async Task<List<int>> GetSeriesIdsByNameAsync(List<string> names)
        {
            if (names == null || !names.Any())
            {
                throw new ArgumentException("La lista de cartas no puede estar vacia.", nameof(names));
            }

            return await _cardDao.GetSeriesIdsByNameAsync(names);
        }


        public async Task<IEnumerable<SeriesListDto>> GetAllSeriesAsync()
        {
            var seriesModels = await _cardDao.GetAllSeriesNamesAsync();
            return seriesModels.Select(series => new SeriesListDto
            {
                Id_series = series.Id_series,
                Series_Name = series.Series_Name,
            });
        }


    }
}
