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

            var fakeResponseId = 5; // Simulamos que el servicio devuelve un ID válido

            // Crear mocks de los servicios requeridos
            var userService = A.Fake<IUserService>();
            var tournamentService = A.Fake<ITournamentService>();
            var cardService = A.Fake<ICardService>();

            // Configurar comportamiento del servicio de torneos
            A.CallTo(() => tournamentService.CreateTournamentAsync(tournamentDto))
                .Returns(Task.FromResult(fakeResponseId));

            // Crear el controlador con todos los servicios requeridos
            var controller = new OrganizerController(userService, tournamentService, cardService);

            // Act
            var actionResult = await controller.CreateTournament(tournamentDto);

            // Assert
            var result = actionResult as CreatedResult;
            Assert.NotNull(result); // Verificamos que la respuesta sea Created (201)
            Assert.Equal(201, result.StatusCode); // Aseguramos que el código HTTP sea 201

            var response = result.Value as ApiResponse<object>;
            Assert.NotNull(response); // Verificamos que haya un ApiResponse
            Assert.True(response.Success, $"Error en la API: {response.Message}");

            // Verificamos que el ID devuelto sea un número válido
            var data = response.Data as dynamic;
            Assert.NotNull(data);

            var id = data.id_Tournament;
            Assert.IsType<int>(id); // Aseguramos que el tipo sea int
            Assert.True(id > 0, "El ID devuelto no es válido.");

            // ✅ Verificar que el servicio de torneos fue llamado exactamente una vez con los datos correctos
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

            // Crear mocks de los servicios requeridos
            var tournamentService = A.Fake<ITournamentService>();
            var cardService = A.Fake<ICardService>();

            // Configurar comportamiento del servicio de torneos
            A.CallTo(() => tournamentService.InsertTournamentDecksAsync(tournamentDeckDto))
                .Returns(Task.FromResult(1)); // Simulamos que la inserción devuelve 1 como éxito

            // Crear el controlador con los servicios mockeados
            var controller = new PlayerController(tournamentService, cardService);

            // Act
            var actionResult = await controller.SetTournamentDecks(tournamentDeckDto);

            // Assert
            var result = actionResult as CreatedResult;
            Assert.NotNull(result); // Verificamos que la respuesta sea Created (201)
            Assert.Equal(201, result.StatusCode); // Aseguramos que el código HTTP sea 201

            var response = result.Value as ApiResponse<object>;
            Assert.NotNull(response); // Verificamos que haya un ApiResponse
            Assert.True(response.Success, $"Error en la API: {response.Message}");
            Assert.Equal("Deck agregado exitosamente", response.Message);

            // ✅ Verificamos que el servicio fue llamado exactamente una vez con los datos correctos
            A.CallTo(() => tournamentService.InsertTournamentDecksAsync(tournamentDeckDto))
                .MustHaveHappenedOnceExactly();
        }

    }
}