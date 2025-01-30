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
        Task<int> CreateGameAsync(GameDto gameDto);

        Task<int> InsertGamePlayersAsync(GamePlayersDto gamePlayers);

        Task<int> CreateRoundAsync(int roundNumber);

        Task<int> CreateMatchAsync(MatchDto match);

        Task<GameResultDto> ResolveGameAsync();


    }
}
