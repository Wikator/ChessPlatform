FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChessPlatform.Frontend.Client/ChessPlatform.Frontend.Client.csproj", "ChessPlatform.Frontend.Client/"]
RUN dotnet restore "ChessPlatform.Frontend.Client/ChessPlatform.Frontend.Client.csproj"
COPY . .
WORKDIR "/src/ChessPlatform.Frontend.Client"
RUN dotnet build "ChessPlatform.Frontend.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChessPlatform.Frontend.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChessPlatform.Frontend.Client.dll"]
