using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class JudgeController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ICardService _cardService;

        public JudgeController(IUserService userService, ITournamentService tournamentService, ICardService cardService)
        {
            _userService = userService;
            _tournamentService = tournamentService;
            _cardService = cardService;
        }

        //Puede descalificar un jugador si es necesario. por endpoint


        //puede ver los jugadores de un torneo determinado  por endpoint

        
        //

        


    }
}
