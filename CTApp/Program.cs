

using CTApp.Middleware;
using CTConfigurations;
using CTDao.Dao.Card;
using CTDao.Dao.RefreshToken;
using CTDao.Dao.User;
using CTDao.Interfaces.Card;
using CTDao.Interfaces.RefreshToken;
using CTDao.Interfaces.User;
using CTDto.Validations.Users.LogIn;
using CTService.Implementation.Card;
using CTService.Implementation.RefreshToken;
using CTService.Implementation.User;
using CTService.Interfaces.Card;
using CTService.Interfaces.RefreshToken;
using CTService.Interfaces.User;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Registrar servicios antes de Build
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar DAOs
builder.Services.AddScoped<ICardDao>(provider =>
{
    return new CardDao(connectionString);
});

builder.Services.AddScoped<IUserDao>(provider =>
{
    return new UserDao(connectionString);
});

// Registrar IRefreshTokenDao
builder.Services.AddScoped<IRefreshTokenDao>(provider =>
{
    return new RefreshTokenDao(connectionString);
});


//validaciones

builder.Services.AddValidatorsFromAssemblyContaining<LogInRequestDtoValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<LogOutRequestDtoValidation>();

builder.Services.AddFluentValidationAutoValidation();

// Registrar Servicios

builder.Services.AddScoped<CTDao.Dao.Security.PasswordHasher>();

builder.Services.AddScoped<ICardService, CardService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();


//--------
builder.Services.Configure<KeysConfiguration>(builder.Configuration.GetSection("Jwt"));
//--------



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("*");
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });






// Registrar otros servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// middleware

app.UseMiddleware<ErrorHandlerMiddleware>();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
