FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_12.x | bash \
    && apt-get install nodejs -yq

WORKDIR /src

COPY WorkoutApp.csproj src/
RUN dotnet restore "src/WorkoutApp.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "WorkoutApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkoutApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet WorkoutApp.dll