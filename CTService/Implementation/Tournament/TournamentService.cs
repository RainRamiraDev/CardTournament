using CTDao.Dao.Tournaments;
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

            bool tournamentExists = await _tournamentDao.TournamentExistsAsync(tournamentDecksDto.Id_Tournament);

            if (!tournamentExists)
            {
                throw new InvalidOperationException("El torneo especificado no existe.");
            }

            // Validar si hay ilustraciones duplicadas
            var duplicateIllustrations = tournamentDecksDto.illustration
                .GroupBy(i => i)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIllustrations.Any())
            {
                throw new InvalidOperationException($"Las siguientes ilustraciones están duplicadas: {string.Join(", ", duplicateIllustrations)}");
            }

            var cardIds = await _cardDao.GetCardIdsByIllustrationAsync(tournamentDecksDto.illustration);

            Console.WriteLine($"[INFO] CARDS IDS: {string.Join(", ", cardIds)}");


            Console.WriteLine($"[INFO] Se encontraron {cardIds.Count} cartas para la ilustración proporcionada.");


            var validatedCards = await ValidatePlayersDecks(cardIds, tournamentDecksDto);

            Console.WriteLine($"[INFO] Se validaron {validatedCards.Count} cartas.");


            var cardSeriesIds = await _cardDao.GetIdCardSeriesByCardIdAsync(validatedCards);

            Console.WriteLine($"[INFO] Se encontraron {cardSeriesIds.Count} series de cartas.");


            if (cardSeriesIds == null || !cardSeriesIds.Any())
            {
                throw new ArgumentException("Invalid series name provided.");
            }

            var tournamentDecksModel = new TournamentDecksModel
            {
                Id_Tournament = tournamentDecksDto.Id_Tournament,
                Id_card_series = cardSeriesIds,
                Id_Owner = tournamentDecksDto.Id_Owner,
            };

            var userCreationResponse = await _tournamentDao.InsertTournamentPlayersAsync(tournamentDecksModel);

            return await _tournamentDao.InsertTournamentDecksAsync(tournamentDecksModel);
        }

        public async Task<List<int>> ValidatePlayersDecks(List<int> cardIds, TournamentDecksDto tournament)
        {
            if (cardIds == null || !cardIds.Any())
                throw new ArgumentException("No se proporcionaron cartas para validar.");


            Console.WriteLine($"[INFO] Cantidad de cartas recibidas: {cardIds.Count}");

            var registeredPlayers = await _tournamentDao.GetUsersFromDb(4);


            if (registeredPlayers == null || !registeredPlayers.Any())
                throw new ArgumentException("No se encontraron Jugadores en la Base de datos");


            Console.WriteLine($"[INFO] Se encontraron {registeredPlayers?.Count ?? 0} jugadores registrados.");

            if (!registeredPlayers.Contains(tournament.Id_Owner))
                throw new ArgumentException("Este Dueño no es un jugador");

            var tournamentPlayers = await _tournamentDao.GetTournamentPlayers(tournament.Id_Tournament);

            Console.WriteLine($"[INFO] Se encontraron {tournamentPlayers?.Count ?? 0} jugadores registrados en el torneo.");

            if (tournamentPlayers.Contains(tournament.Id_Owner))
                throw new ArgumentException("Este jugador ya se encuentra registrado");


            var tournamentSeries = await _tournamentDao.GetSeriesFromTournamentAsync(tournament.Id_Tournament);

            Console.WriteLine($"[INFO] Se encontraron {tournamentSeries?.Count ?? 0} series para el torneo.");

            if (tournamentSeries == null || !tournamentSeries.Any())
                throw new InvalidOperationException("El torneo no tiene series pactadas.");

            var validCardsFromSeries = await _tournamentDao.GetCardsFromTournamentSeries(tournamentSeries);

            Console.WriteLine($"[INFO] Se encontraron {validCardsFromSeries.Count} cartas válidas en las series del torneo.");


            var validCards = new List<int>();

            var invalidCards = new List<int>();

            foreach (var card in cardIds)
            {
                if (validCardsFromSeries.Contains(card))
                {
                    validCards.Add(card);
                }
                else
                {
                    invalidCards.Add(card);
                }
            }

            Console.WriteLine($"[INFO] {validCards.Count} cartas son válidas. {invalidCards.Count} cartas son inválidas.");


            if (invalidCards.Any())
            {
                var wrongCardIllustrations = await _cardDao.GetCardIllustrationById(invalidCards);

                Console.WriteLine($"[ERROR] Cartas inválidas detectadas: {string.Join(", ", wrongCardIllustrations)}");

                throw new InvalidOperationException($"Las siguientes cartas no pertenecen a las series permitidas:\n{string.Join("\n", wrongCardIllustrations)}");
            }

            return validCards;
        }

        public async Task<int> CreateTournamentAsync(TournamentDto tournamentDto)
        {

            // Obtener los IDs de los jueces a partir de los alias
            var judgeIds = await _tournamentDao.GetJudgeIdsByAliasAsync(tournamentDto.Judges);
            Console.WriteLine($"Judges IDs: {string.Join(", ", judgeIds)}");


            // Obtener los IDs de las series a partir de los nombres
            var seriesIds = await _cardDao.GetSeriesIdsByNameAsync(tournamentDto.Series_name);
            Console.WriteLine($"Series IDs: {string.Join(", ", seriesIds)}");


            var tournamentModel = new TournamentModel
            {
                Id_Country = tournamentDto.Id_Country,
                Id_Organizer = tournamentDto.Id_Organizer,
                Start_datetime = tournamentDto.Start_datetime,
                Current_Phase = 1,
                Judges = judgeIds,
                Series_name = seriesIds
            };

            var validatedTournament = await ValidateTournament(tournamentModel);

            return await _tournamentDao.CreateTournamentAsync(tournamentModel);
        }

        public async Task<bool> ValidateTournament( TournamentModel tournament)
        {
            // Validar el país
            var registeredCountries = await _tournamentDao.GetCountriesFromDb();
            if (!registeredCountries.Contains(tournament.Id_Country))
                throw new ArgumentException("El país especificado no está registrado.");

            Console.WriteLine($"[INFO] Se encontraron {registeredCountries.Count} países registrados en la BD.");


            // Validar que el organizador sea un organizador
            var registeredOrganizers = await _tournamentDao.GetUsersFromDb(2);
            if (!registeredOrganizers.Contains(tournament.Id_Organizer))
                throw new ArgumentException("El organizador especificado no está registrado.");

            Console.WriteLine($"[INFO] Se encontraron {registeredOrganizers.Count} organizadores registrados en la BD.");


            // Validar los jueces
            var registeredJudges = await _tournamentDao.GetUsersFromDb(3);

            Console.WriteLine($"[INFO] Se encontraron {registeredJudges.Count} jueces registrados en la BD.");

            var invalidJudges = tournament.Judges.Where(idJudge => !registeredJudges.Contains(idJudge)).ToList();

            if (invalidJudges.Any())
            {
                Console.WriteLine($"[ERROR] Jueces no registrados detectados: {string.Join(", ", invalidJudges)}");
                throw new ArgumentException($"Los siguientes jueces no están registrados: {string.Join(", ", invalidJudges)}");
            }

            // Validar las series
            var registeredSeries = await _cardDao.GetAllSeries();

            var invalidSeries = tournament.Series_name.Where(seriesId => !registeredSeries.Contains(seriesId)).ToList();

            if (invalidSeries.Any())
            {
                Console.WriteLine($"[ERROR] Series no registradas detectadas: {string.Join(", ", invalidSeries)}");
                throw new ArgumentException($"Las siguientes series no están registradas: {string.Join(", ", invalidSeries)}");
            }

            return true;
        }

        public async Task<IEnumerable<TournamentDto>> GetAllTournamentAsync()
        {
            var tournaments = await _tournamentDao.GetAllTournamentAsync();

            return tournaments.Select(tournament => new TournamentDto
            {
                Id_Country = tournament.Id_Country,
                Id_Organizer = tournament.Id_Country,
                Start_datetime = tournament.Start_datetime,
                //End_datetime = tournament.End_datetime,
                //Current_Phase = tournament.Current_Phase,
            });
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

        public async Task<int> SetTournamentToNextPhase(int tournament_id)
        {
            var id_tournament = tournament_id;
            return await _tournamentDao.GetTournamentCurrentPhase(id_tournament);
        }
    }
}
