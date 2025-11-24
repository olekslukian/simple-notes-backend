FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY SimpleNotesApp.sln ./
COPY src/SimpleNotesApp.API/SimpleNotesApp.API.csproj ./src/SimpleNotesApp.API/
COPY src/SimpleNotesApp.Core/SimpleNotesApp.Core.csproj ./src/SimpleNotesApp.Core/
COPY src/SimpleNotesApp.Infrastructure/SimpleNotesApp.Infrastructure.csproj ./src/SimpleNotesApp.Infrastructure/

RUN dotnet restore

COPY src/ ./src/

WORKDIR /src/src/SimpleNotesApp.API
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

EXPOSE 8080

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SimpleNotesApp.API.dll"]
