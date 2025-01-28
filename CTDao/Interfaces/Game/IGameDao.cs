using CTDataModels.Game;
using CTDataModels.Tournamets;
using CTDto.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Interfaces.Game
{
    public interface IGameDao
    {
        Task<int> CreateGameAsync(GameModel game); 
        Task<int> InsertGamePlayersAsync(GamePlayersModel playerModel);
        Task<int> SetGameMatchWinnerAsync(int winner, int match);
        Task<int> SetGameLoserAsync(int loser);
        Task<List<int>> GetTournamentPlayers(int tournamentId);
        Task<int> CreateRoundAsync(RoundModel round); // !!!
        Task<int> CreateMatchAsync(MatchModel match); // !!!
        Task<int> SetNextRoundAsync();
    }
}
