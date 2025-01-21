using CTDataModels.Tournamets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Interfaces.Tournaments
{
    public interface ITournamentDao
    {
        Task<int> CreateTournamentAsync(TournamentModel tournament);
        Task<IEnumerable<TournamentModel>> GetAllTournamentAsync();
        Task<IEnumerable<AvailableTournamentsModel>> GetAllAvailableTournamentsAsync();
        Task<int> InsertTournamentJudgesAsync(int IdTournament, List<int> judgeIds);
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

    }
}
