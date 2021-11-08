#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TingTuffer.Examples.WorkerStatic/TingTuffer.Examples.WorkerStatic.csproj", "TingTuffer.Examples.WorkerStatic/"]
RUN dotnet restore "TingTuffer.Examples.WorkerStatic/TingTuffer.Examples.WorkerStatic.csproj"
COPY . .
WORKDIR "/src/TingTuffer.Examples.WorkerStatic"
RUN dotnet build "TingTuffer.Examples.WorkerStatic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TingTuffer.Examples.WorkerStatic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TingTuffer.Examples.WorkerStatic.dll"]