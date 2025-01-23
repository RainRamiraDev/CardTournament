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

        //Card
        /*Task<int> InsertTournamentDecksAsync(int IdTournament, List<int> judgeIds, int IdOwner);*/ //meter los decks armados
        //Task<List<int>> GetCardIdsByIllustrationAsync(List<string> cardsIllustrations);          //elegir las cartas limitadas por la serie buscadas por la ilustracion
        //Task<IEnumerable<ShowCardsModel>> GetCardsBySeries();                                    //trae las cartas filtradas por las series elegidas

    }
}
