using CTDao.Interfaces.Card;
using CTDto.Card;
using CTService.Interfaces.Card;
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

        public CardService(ICardDao cardDao)
        {
            _cardDao = cardDao;
        }

        public async Task<IEnumerable<ShowCardsDto>> GetAllCardsAsync()
        {
            var cards = await _cardDao.GetAllAsync();

            // Mapeo de CardModel a CardDto
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
    }
}
