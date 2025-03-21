﻿using CTDataModels.Card;
using CTDataModels.Game;
using CTDataModels.Tournamets;
using CTDataModels.Users;
using CTDataModels.Users.Judge;
using CTDto.Card;
using CTDto.Users;
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

        Task AlterTournamentAsync(AlterTournamentModel tournament);

        Task SoftDeleteTournamentAsync(int id_tournament);


        Task<IEnumerable<TournamentsInformationModel>> GetTournamentsInformationAsync(GetTournamentInformationModel tournamentInformationModel);
        Task<int> SetTournamentToNextPhaseAsync(int tournament_id);
        Task<int> GetTournamentCurrentPhaseAsync(int id_tournament);
        Task<bool> TournamentExistsAsync(int tournamentId);

        Task<List<int>> GetSeriesFromTournamentAsync(int tournamentId);

        Task<List<int>> GetCardsFromTournamentSeriesAsync(List<int> tournamentSeries, List<int> tournamentCards);

        Task<DateTime> GetTournamentStartDateAsync(int id_tournament);


        Task<List<int>> ValidateCountriesFromDbAsync(int id_country);

        Task<List<int>> ValidateTournamentPlayersAsync(int tournamentId, int id_player);

        Task<List<int>> GetTournamentJudgesAsync(int id_tournament);

        Task<List<int>> ValidateUsersFromDbAsync(int id_rol, List<int>id_user);

        Task<TournamentModel> GetTournamentByIdAsync(int id_tournament);

        Task<List<int>> ValidateAvailableTournamentsAsync(int id_tournament);

        //Judge
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

        //cards
        Task<int> InsertTournamentDecksAsync(TournamentDecksModel tournamentDecks);
        Task<int> InsertTournamentPlayersAsync(TournamentDecksModel tournamentDecks);

        Task<bool> CheckTournamentCapacity(PlayerCapacityModel capacity, int id_tournament);
        Task DisqualifyPlayerFromTournamentAsync(DisqualificationModel disqualificationRequest);

        Task<bool> ValidateJudgesFromTournament(int id_Judge, int id_Tournament);
        Task<List<ShowTournamentPlayersModel>> ShowPlayersFromTournamentAsync(int tournament_Id);
    }
}
