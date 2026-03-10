# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /app

# 1. Copy the solution and project files first
# This allows Docker to cache the 'restore' layer if dependencies haven't changed
COPY *.sln ./
COPY src/*.csproj ./src/
RUN dotnet restore

# 2. Copy the rest of the source code
COPY . ./

# 3. Publish the application to the 'output' folder
RUN dotnet publish src/*.csproj -c Release -o /app/output

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the compiled output from the builder stage
COPY --from=builder /app/output .

# Configure ASP.NET Core to listen on port 8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "SimpleNotesApp.API.dll"]
