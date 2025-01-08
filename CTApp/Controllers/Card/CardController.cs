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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCards()
        {
            try
            {
                var cards = await _cardService.GetAllCardsAsync();

                if (cards == null || !cards.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<CardDto>>.ErrorResponse("Cartas no encontradas."));
                }

                var response = ApiResponse<IEnumerable<CardDto>>.SuccessResponse("Cartas obtenidas exitosamente.", cards);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errors = new List<string> { "Ocurrió un error al obtener las cartas." };
                var stackTrace = ex.StackTrace;
                var response = ApiResponse<CardDto>.ErrorResponse(errors, stackTrace);
                return BadRequest(response);
            }
        }
    }
}
