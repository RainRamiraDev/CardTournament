using CTApp.Response;
using CTDto.Card;
using CTService.Interfaces.Card;

using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.Card
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {

        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        
    }
}
