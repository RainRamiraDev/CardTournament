using CTDao.Dao.Tournaments;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using CTDto.Tournaments;
using CTService.Interfaces.Tournaments;
using MySql.Data.MySqlClient;
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


        public TournamentService(ITournamentDao tournamentDao)
        {
            _tournamentDao = tournamentDao;
        }

        public async Task<int> CreateTournamentAsync(TournamentDto tournamentDto)
        {

            var tournamentModel = new TournamentModel
            {
                Id_Country = tournamentDto.Id_Country,
                Id_Organizer = tournamentDto.Id_Country,
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
            return await _tournamentDao.InsertTournamentJudgesAsync(tournamentJudgeDto.Id_Tournament, judgeIds);
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
                Current_Phase = tournament.Current_Phase,
                Tournament_Country = tournament.Tournament_Country ?? string.Empty,  // Asignar vacío si es null
                Organizer_Name = tournament.Organizer_Name ?? string.Empty,  // Asignar vacío si es null
                Organizer_Alias = tournament.Organizer_Alias ?? string.Empty,  // Asignar vacío si es null
                Organizer_Email = tournament.Organizer_Email ?? string.Empty,  // Asignar vacío si es null
                Judges = tournament.Judges ?? string.Empty,  // Asignar vacío si es null
                Series_Played = tournament.Series_Played ?? string.Empty,  // Asignar vacío si es null
                Players = tournament.Players ?? string.Empty,  // Asignar vacío si es null
                Disqualified_Players = tournament.Disqualified_Players ?? string.Empty,  // Asignar vacío si es null
                Total_Games = tournament.Total_Games,
                Total_Rounds = tournament.Total_Rounds,
            });
        }
    }
}
