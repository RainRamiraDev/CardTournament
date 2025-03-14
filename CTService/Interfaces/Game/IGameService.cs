﻿using CTDataModels.Game;
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
        Task<int> CreateRoundAsync(int roundNumber, int tournament_id, int id_judge);
        Task<int> CreateMatchAsync(MatchDto match);
        Task<GameResultDto> ResolveGameAsync(TournamentRequestToResolveDto tournamentDto, string timeZone);
        Task<List<MatchScheduleDto>> CalculateMatchScheduleAsync(TournamentRequestToResolveDto request, string timeZone);
    }
}
