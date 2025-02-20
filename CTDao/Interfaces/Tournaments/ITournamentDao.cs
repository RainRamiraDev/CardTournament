﻿using CTDataModels.Card;
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
        Task<IEnumerable<TournamentsInformationModel>> GetTournamentsInformationAsync(GetTournamentInformationModel tournamentInformationModel);
        Task<int> SetTournamentToNextPhaseAsync(int tournament_id);
        Task<int> GetTournamentCurrentPhaseAsync(int id_tournament);
        Task<bool> TournamentExistsAsync(int tournamentId);

        Task<List<int>> GetSeriesFromTournamentAsync(int tournamentId);

        Task<List<int>> GetCardsFromTournamentSeriesAsync(List<int> tournamentSeries);

        Task<DateTime> GetTournamentStartDateAsync(int id_tournament);


        Task<List<int>> GetCountriesFromDbAsync();

        Task<List<int>> GetTournamentPlayersAsync(int tournamentId);

        Task<List<int>> GetTournamentJudgesAsync(int id_tournament);

        Task<List<int>> GetUsersFromDbAsync(int id_rol);

        Task<TournamentModel> GetTournamentByIdAsync(int id_tournament);

        Task<List<int>> GetAvailableTournamentsAsync();

        //Judge
        Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases);

        //cards
        Task<int> InsertTournamentDecksAsync(TournamentDecksModel tournamentDecks);
        Task<int> InsertTournamentPlayersAsync(TournamentDecksModel tournamentDecks);
    }
}
