using CTApp.Middleware;
using CTConfigurations;
using CTDao.Dao.Card;
using CTDao.Dao.Game;
using CTDao.Dao.RefreshToken;
using CTDao.Dao.Tournaments;
using CTDao.Dao.User;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.Game;
using CTDao.Interfaces.RefreshToken;
using CTDao.Interfaces.Tournaments;
using CTDao.Interfaces.User;
using CTDto.Validations.Tournament;
using CTDto.Validations.Users;
using CTDto.Validations.Users.LogIn;
using CTService.Implementation.Card;
using CTService.Implementation.Game;
using CTService.Implementation.RefreshToken;
using CTService.Implementation.Tournament;
using CTService.Implementation.User;
using CTService.Interfaces.Card;
using CTService.Interfaces.Game;
using CTService.Interfaces.RefreshToken;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddSingleton<ICardDao>(provider =>
{
    return new CardDao(connectionString);
});

builder.Services.AddSingleton<IUserDao>(provider =>
{
    return new UserDao(connectionString);
});

builder.Services.AddSingleton<IRefreshTokenDao>(provider =>
{
    return new RefreshTokenDao(connectionString);
});

builder.Services.AddSingleton<ITournamentDao>(provider =>
{
    return new TournamentDao(connectionString);
});

builder.Services.AddSingleton<IGameDao>(provider =>
{
    return new GameDao(connectionString);
});



//validaciones

builder.Services.AddValidatorsFromAssemblyContaining<FirstLogInRequestDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<LogOutRequestDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<TournamentDecksRequestDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<TournamentDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<TournamentRequestToResolveDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreationDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<AlterUserDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<SoftDeleteUserDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<DisqualificationDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<GetTournamentInformationDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<AssignCardToPlayerDtoValidation>();



builder.Services.AddFluentValidationAutoValidation();

// Registrar Servicios

builder.Services.AddScoped<CTDao.Dao.Security.PasswordHasher>();

builder.Services.AddScoped<ICardService, CardService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddScoped<ITournamentService, TournamentService>();

builder.Services.AddScoped<IGameService, GameService>();


//--------
builder.Services.Configure<KeysConfiguration>(builder.Configuration.GetSection("Jwt"));
//--------


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDevServer", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // puerto de Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            RoleClaimType = "UserRole" // IMPORTANTE: Configurar para que lea el rol desde "UserRole"
        };
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();


// middleware

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

//app.UseCors();

app.UseCors("AllowReactDevServer");


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
