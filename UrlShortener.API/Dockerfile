FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY UrlShortener.API/UrlShortener.API.csproj UrlShortener.API/
COPY UrlShortener.Common/UrlShortener.Common.csproj UrlShortener.Common/
COPY UrlShortener.Data/UrlShortener.Data.csproj UrlShortener.Data/
COPY UrlShortener.Services/UrlShortener.Services.csproj UrlShortener.Services/
COPY UrlShortener.UnitTests/UrlShortener.UnitTests.csproj UrlShortener.UnitTests/
COPY UrlShortener.IntegrationTests/UrlShortener.IntegrationTests.csproj UrlShortener.IntegrationTests/

RUN dotnet restore UrlShortener.API/UrlShortener.API.csproj

COPY . .

RUN dotnet publish UrlShortener.API/UrlShortener.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]