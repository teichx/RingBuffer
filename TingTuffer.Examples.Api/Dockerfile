#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TingTuffer.Examples.Api/TingTuffer.Examples.Api.csproj", "TingTuffer.Examples.Api/"]
COPY ["TingTuffer/TingTuffer.csproj", "TingTuffer/"]
RUN dotnet restore "TingTuffer.Examples.Api/TingTuffer.Examples.Api.csproj"
COPY . .
WORKDIR "/src/TingTuffer.Examples.Api"
RUN dotnet build "TingTuffer.Examples.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TingTuffer.Examples.Api.csproj" -c Release -o /app/publish

RUN dotnet dev-certs https --trust

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TingTuffer.Examples.Api.dll"]