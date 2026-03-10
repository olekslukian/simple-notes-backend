# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /app

# 1. Copy the solution file
COPY *.sln ./

# 2. Recreate the folder structure and copy each .csproj file
# This is required for 'dotnet restore' to work with a solution file
COPY src/SimpleNotesApp.API/*.csproj ./src/SimpleNotesApp.API/
COPY src/SimpleNotesApp.Core/*.csproj ./src/SimpleNotesApp.Core/
COPY src/SimpleNotesApp.Infrastructure/*.csproj ./src/SimpleNotesApp.Infrastructure/

# Also copy test projects as they are referenced in your .sln
COPY tests/SimpleNotesApp.Core.Tests/*.csproj ./tests/SimpleNotesApp.Core.Tests/
COPY tests/SimpleNotesApp.API.Tests/*.csproj ./tests/SimpleNotesApp.API.Tests/

# 3. Restore dependencies
RUN dotnet restore

# 4. Copy the rest of the source code
COPY . ./

# 5. Publish the application
# Specify the path to the API project file
RUN dotnet publish src/SimpleNotesApp.API/SimpleNotesApp.API.csproj -c Release -o /app/output

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=builder /app/output .

# Configure ASP.NET Core to listen on port 8080 [cite: 3]
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleNotesApp.API.dll"]
