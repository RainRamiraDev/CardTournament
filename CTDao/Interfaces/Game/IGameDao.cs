﻿using CTDataModels.Game;
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
        Task<int> SetGameWinnerAsync(int winner);
        Task<int> SetGameLoserAsync(List<int> losers);
        Task<List<int>> GetTournamentPlayers(int tournamentId);
        Task<int> CreateRoundAsync(RoundModel round); 
        Task<int> CreateMatchAsync(MatchModel match); 
        Task<int> SetNextRoundAsync(int tournament_id);
        Task<int> SetRoundCompletedAsync(int roundNumber, int tournament_id);
        Task<int> GetLastRoundAsync(int tournament_id);
    }
}
