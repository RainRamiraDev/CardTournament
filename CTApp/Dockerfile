# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7276
EXPOSE 8081
ENV DB_CONNECTION_STRING="Server=host.docker.internal;Port=3306;Database=card_tournamentdbv5;User=root;Password=1234;"

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CTApp/CTApp.csproj", "CTApp/"]
COPY ["CTConfigurations/CTConfigurations.csproj", "CTConfigurations/"]
COPY ["CTDto/CTDto.csproj", "CTDto/"]
COPY ["CTService/CTService.csproj", "CTService/"]
COPY ["CTDao/CTDao.csproj", "CTDao/"]
COPY ["CTDataModels/CTDataModels.csproj", "CTDataModels/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
RUN dotnet restore "./CTApp/CTApp.csproj"
COPY . .
WORKDIR "/src/CTApp"
RUN dotnet build "./CTApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CTApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CTApp.dll"]