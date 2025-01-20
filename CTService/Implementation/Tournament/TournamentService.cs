using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using CTDto.Tournaments;
using CTService.Interfaces.Tournaments;
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
    }
}
