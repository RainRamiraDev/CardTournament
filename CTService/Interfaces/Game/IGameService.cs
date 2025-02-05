using CTDto.Game;
using CTDto.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Interfaces.Game
{
    public interface IGameService
    {

        Task<int> CreateRoundAsync(int roundNumber, int tournament_id);

        Task<int> CreateMatchAsync(MatchDto match);

        Task<GameResultDto> ResolveGameAsync(int tournament_id);


    }
}
