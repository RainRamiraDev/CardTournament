using CTDataModels.Game;
using CTDto.Card;
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
        //Tournament
        Task<int> CreateTournamentAsync(TournamentDto tournamentDto);
        Task<IEnumerable<TournamentsInformationDto>> GetTournamentsInformationAsync(GetTournamentInformationDto getTournamentInformation);

        //Judges
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

        //Cards
        Task<int> InsertTournamentDecksAsync(TournamentDecksDto tournamentDecksDto);

        Task<PlayerCapacityModel> CalculatePlayerCapacity(int id_tournament);


    }
}
