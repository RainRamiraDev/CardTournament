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
    }
}
