using CTDto.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Interfaces.Tournaments
{
    public interface ITournamentService
    {
        Task<int> CreateTournamentAsync(TournamentDto tournamentDto);
        Task<IEnumerable<TournamentDto>> GetAllTournamentAsync();
        Task<int> InsertTournamentJudgesAsync(TournamentJudgeDto tournamentJudgeDto);
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);
        Task<IEnumerable<AvailableTournamentsDto>> GetAllAvailableTournamentsAsync();
    }
}
