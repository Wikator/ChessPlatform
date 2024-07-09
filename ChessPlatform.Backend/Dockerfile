FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChessPlatform.Backend/ChessPlatform.Backend.csproj", "ChessPlatform.Backend/"]
RUN dotnet restore "ChessPlatform.Backend/ChessPlatform.Backend.csproj"
COPY . .
WORKDIR "/src/ChessPlatform.Backend"
RUN dotnet build "ChessPlatform.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChessPlatform.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChessPlatform.Backend.dll"]
