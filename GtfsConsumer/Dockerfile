#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GtfsConsumer/GtfsConsumer.csproj", "GtfsConsumer/"]
COPY ["AcTransitMap.Shared.Entities/AcTransitMap.Shared.Entities.csproj", "AcTransitMap.Shared.Entities/"]
RUN dotnet restore "GtfsConsumer/GtfsConsumer.csproj"
COPY . .
WORKDIR "/src/GtfsConsumer"
RUN dotnet build "GtfsConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GtfsConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GtfsConsumer.dll"]