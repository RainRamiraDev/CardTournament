using CTDataModels.Card;
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
        //Tournament
        Task<int> CreateTournamentAsync(TournamentModel tournament);
        Task<IEnumerable<TournamentModel>> GetAllTournamentAsync();
        Task<IEnumerable<AvailableTournamentsModel>> GetAllAvailableTournamentsAsync();


        //Judge
        Task<int> InsertTournamentJudgesAsync(List<int> judgeIds);
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

        //cards
        Task<int> InsertTournamentSeriesAsync(List<int> cardsIds);
        Task<int> InsertTournamentDecksAsync(List<int> cardsIds, int owner);
        Task<int> InsertTournamentPlayersAsync(int player);
    }
}
