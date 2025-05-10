FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevRoutine/DevRoutine.Api/DevRoutine.Api.csproj", "DevRoutine/DevRoutine.Api/"]
RUN dotnet restore "DevRoutine/DevRoutine.Api/DevRoutine.Api.csproj"
COPY . .
WORKDIR "/src/DevRoutine/DevRoutine.Api"
RUN dotnet build "./DevRoutine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DevRoutine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevRoutine.Api.dll"]
