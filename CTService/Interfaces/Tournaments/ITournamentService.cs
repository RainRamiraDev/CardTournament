using CTDataModels.Game;
using CTDto.Card;
using CTDto.Tournaments;
using CTDto.Users;
using CTDto.Users.Judge;
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

        Task AlterTournamentAsync(AlterTournamentDto tournamentDto);

        Task SoftDeleteTournamentAsync(TournamentRequestToResolveDto tournamentDto);

        Task<IEnumerable<TournamentsInformationDto>> GetTournamentsInformationAsync(GetTournamentInformationDto getTournamentInformation);

        //Judges
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

        Task DisqualifyPlayerFromTournamentAsync(DisqualificationDto disqualificationDto);

        //Cards
        Task<int> InsertTournamentDecksAsync(TournamentDecksDto tournamentDecksDto);

        Task<PlayerCapacityModel> CalculatePlayerCapacity(int id_tournament);

        Task<List<ShowTournamentPlayersDto>> ShowPlayersFromTournamentAsync(TournamentRequestToResolveDto showPlayersFromTournamentDto);
    }
}
