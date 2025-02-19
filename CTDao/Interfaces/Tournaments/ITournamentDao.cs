using CTDataModels.Card;
using CTDataModels.Tournamets;
using CTDto.Card;
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
        Task<IEnumerable<TournamentsInformationModel>> GetTournamentsInformationAsync(GetTournamentInformationModel tournamentInformationModel);

        Task<int> SetTournamentToNextPhase(int tournament_id);
        Task<int> GetTournamentCurrentPhase(int id_tournament);

        Task<bool> TournamentExistsAsync(int tournamentId);

        Task<List<int>> GetSeriesFromTournamentAsync(int tournamentId);

        Task<List<int>> GetCardsFromTournamentSeries(List<int> tournamentSeries);

        Task<DateTime> GetTournamentStartDateAsync(int id_tournament);

        Task<int> SetTournamentEndDate(TournamentUpdateEndDatetimeModel tournamentData);

        Task<List<int>> GetCountriesFromDb();

        Task<List<int>> GetTournamentPlayers(int tournamentId);

        Task<List<int>> GetTournamentJudges(int id_tournament);

        Task<List<int>> GetUsersFromDb(int id_rol);

        Task<TournamentModel> GetTournamentById(int id_tournament);

        //Judge
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

        //cards
        Task<int> InsertTournamentDecksAsync(TournamentDecksModel tournamentDecks);
        Task<int> InsertTournamentPlayersAsync(TournamentDecksModel tournamentDecks);
    }
}
