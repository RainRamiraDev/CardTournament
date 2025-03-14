using CTApp.Controllers.User;
using CTApp.Response;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.Game;
using CTDao.Interfaces.Tournaments;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Game;
using CTDto.Tournaments;
using CTService.Implementation.Tournament;
using CTService.Interfaces.Card;
using CTService.Interfaces.Game;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using FakeItEasy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CTTests
{
    public class TournamentTests
    {
        private readonly ITournamentDao _tournamentDao;
        private readonly IGameDao _gameDao;
        private readonly ICardDao _cardDao;
        private readonly IGameService _gameService;
        private readonly TournamentService _tournamentService;

        public TournamentTests()
        {
            _gameDao = A.Fake<IGameDao>();
            _tournamentDao = A.Fake<ITournamentDao>();
            _cardDao = A.Fake<ICardDao>();
            _tournamentService = A.Fake<TournamentService>();
            _gameService = A.Fake<IGameService>();
        }


        [Fact]
        public async Task CreateTournament_Returns_Valid_Id()
        {
            // Arrange
            var tournamentDto = new TournamentDto
            {
                Id_Country = 125,
                Start_datetime = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc),
                End_datetime = new DateTime(2025, 9, 30, 0, 0, 0, DateTimeKind.Utc),
                Judges_Id = new List<int> { 21, 22, 23 },
                Series_Id = new List<int> { 1, 2, 3 }
            };

            var fakeResponseId = 5; 

            var userService = A.Fake<IUserService>();
            var tournamentService = A.Fake<ITournamentService>();
            var cardService = A.Fake<ICardService>();

            A.CallTo(() => tournamentService.CreateTournamentAsync(tournamentDto))
                .Returns(Task.FromResult(fakeResponseId));

            var controller = new OrganizerController(userService, tournamentService, cardService);

            // Act
            var actionResult = await controller.CreateTournament(tournamentDto);

            // Assert
            var result = actionResult as CreatedResult;
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode); 

            var response = result.Value as ApiResponse<object>;
            Assert.NotNull(response); 
            Assert.True(response.Success, $"Error en la API: {response.Message}");

            var data = response.Data as dynamic;
            Assert.NotNull(data);

            var id = data.id_Tournament;
            Assert.IsType<int>(id); 
            Assert.True(id > 0, "El ID devuelto no es válido.");

            A.CallTo(() => tournamentService.CreateTournamentAsync(tournamentDto))
                .MustHaveHappenedOnceExactly();
        }


        [Fact]
        public async Task SetTournamentDecks_Returns_Created_Response()
        {
            // Arrange
            var tournamentDeckDto = new TournamentDecksDto
            {
                Id_Tournament = 11,
                Cards = new List<int> { 101, 102, 103 }
            };

            var tournamentService = A.Fake<ITournamentService>();
            var cardService = A.Fake<ICardService>();

            A.CallTo(() => tournamentService.InsertTournamentDecksAsync(tournamentDeckDto))
                .Returns(Task.FromResult(1)); 

            var controller = new PlayerController(tournamentService, cardService);

            // Act
            var actionResult = await controller.SetTournamentDecks(tournamentDeckDto);

            // Assert
            var result = actionResult as CreatedResult;
            Assert.NotNull(result); 
            Assert.Equal(201, result.StatusCode); 

            var response = result.Value as ApiResponse<object>;
            Assert.NotNull(response); 
            Assert.True(response.Success, $"Error en la API: {response.Message}");
            Assert.Equal("Deck agregado exitosamente", response.Message);

            A.CallTo(() => tournamentService.InsertTournamentDecksAsync(tournamentDeckDto))
                .MustHaveHappenedOnceExactly();
        }

    }
}