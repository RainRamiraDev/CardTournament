﻿using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Tournaments;
using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.Tournament
{
    public class TournamentService : ITournamentService
    {

        private readonly ITournamentDao _tournamentDao;

        private readonly ICardDao _cardDao;


        public TournamentService(ITournamentDao tournamentDao, ICardDao cardDao)
        {
            _tournamentDao = tournamentDao;
            _cardDao = cardDao;
        }


        public async Task<int> InsertTournamentDecksAsync(TournamentDecksDto tournamentDecksDto)
        {
            if (tournamentDecksDto == null)
            {
                throw new ArgumentException("Invalid tournament data.");
            }

            var cardIds = await _cardDao.GetCardIdsByIllustrationAsync(tournamentDecksDto.illustration);
            var cardSeriesIds = await _cardDao.GetIdCardSeriesByCardIdAsync(cardIds);

            if (cardSeriesIds == null || cardSeriesIds.Count == 0)
            {
                throw new ArgumentException("Invalid series name provided.");
            }

           var userCreationResponse =  await _tournamentDao.InsertTournamentPlayersAsync(tournamentDecksDto.Id_Owner);

            return await _tournamentDao.InsertTournamentDecksAsync(cardSeriesIds,tournamentDecksDto.Id_Owner);
        }

        public async Task<int> CreateTournamentAsync(TournamentDto tournamentDto)
        {

            var tournamentModel = new TournamentModel
            {
                Id_Country = tournamentDto.Id_Country,
                Id_Organizer = tournamentDto.Id_Organizer,
                Start_datetime = tournamentDto.Start_datetime,
                End_datetime = tournamentDto.End_datetime,
                Current_Phase = tournamentDto.Current_Phase
            };

            return await _tournamentDao.CreateTournamentAsync(tournamentModel);
        }

        public async Task<IEnumerable<TournamentDto>> GetAllTournamentAsync()
        {
            var tournaments = await _tournamentDao.GetAllTournamentAsync();

            return tournaments.Select(tournament => new TournamentDto
            {
                Id_Country = tournament.Id_Country,
                Id_Organizer = tournament.Id_Country,
                Start_datetime = tournament.Start_datetime,
                End_datetime = tournament.End_datetime,
                Current_Phase = tournament.Current_Phase,
            });
        }

        public async Task<int> InsertTournamentJudgesAsync(TournamentJudgeDto tournamentJudgeDto)
        {
            if (tournamentJudgeDto == null || tournamentJudgeDto.Judges == null || !tournamentJudgeDto.Judges.Any())
            {
                throw new ArgumentException("Invalid tournament data.");
            }

            // Obtener los IDs de los jueces a partir de los alias
            var judgeIds = await _tournamentDao.GetJudgeIdsByAliasAsync(tournamentJudgeDto.Judges);

            if (judgeIds == null || judgeIds.Count == 0)
            {
                throw new ArgumentException("Invalid judge aliases provided.");
            }

            // Insertar los jueces en el torneo
            return await _tournamentDao.InsertTournamentJudgesAsync(judgeIds);
        }

        public async Task<List<int>> GetJudgeIdsByAliasAsync(List<string> judgeAliases)
        {
            if (judgeAliases == null || !judgeAliases.Any())
            {
                throw new ArgumentException("The list of judge aliases cannot be empty.", nameof(judgeAliases));
            }

            return await _tournamentDao.GetJudgeIdsByAliasAsync(judgeAliases);
        }

        public async Task<IEnumerable<AvailableTournamentsDto>> GetAllAvailableTournamentsAsync()
        {
            var tournaments = await _tournamentDao.GetAllAvailableTournamentsAsync();

            return tournaments.Select(tournament => new AvailableTournamentsDto
            {
                Start_DateTime = tournament.Start_DateTime,
                End_DateTime = tournament.End_DateTime,
                Tournament_Country = tournament.Tournament_Country ?? string.Empty, 
                Organizer_Alias = tournament.Organizer_Alias ?? string.Empty,  
                Judges = tournament.Judges ?? string.Empty,  
                Series_Played = tournament.Series_Played ?? string.Empty,  
                Players = tournament.Players ?? string.Empty,  
                Disqualified_Players = tournament.Disqualified_Players ?? string.Empty,
                Total_Games = tournament.Total_Games,
                Total_Rounds = tournament.Total_Rounds,
            });
        }

        public async Task<int> InsertTournamentSeriesAsync(TournamentSeriesDto tournamentseriesDto)
        {
            if (tournamentseriesDto == null || tournamentseriesDto.Series_name == null || !tournamentseriesDto.Series_name.Any())
            {
                throw new ArgumentException("Invalid tournament data.");
            }

            var seriesIds = await _cardDao.GetSeriesIdsByNameAsync(tournamentseriesDto.Series_name);

            if (seriesIds == null || seriesIds.Count == 0)
            {
                throw new ArgumentException("Invalid series name provided.");
            }

            return await _tournamentDao.InsertTournamentSeriesAsync(seriesIds);
        }

        public async Task<int> SetTournamentToNextPhase()
        {
            var id_tournament = TournamentDao.createdtournamentId;
            return await _tournamentDao.GetTournamentCurrentPhase(id_tournament);
        }
    }
}
