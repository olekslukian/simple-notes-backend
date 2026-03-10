# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /app

# Copy solution and project files first to cache restore layer
COPY *.sln ./
COPY src/*.csproj ./src/
RUN dotnet restore

# Copy the rest of the source code and build
COPY . ./
RUN dotnet publish -c Release -o /app/output

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /app/output .

# Environment variables for the container
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleNotesApp.API.dll"]
