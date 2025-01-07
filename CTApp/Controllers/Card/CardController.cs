using CTService.Interfaces.Card;
using GameApp.Dtos.Card;
using GameApp.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Controllers.Card
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
