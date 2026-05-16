# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first (this makes builds faster)
COPY ["ServerApp/ServerApp.csproj", "ServerApp/"]
COPY ["ClientApp/ClientApp.csproj", "ClientApp/"]
COPY ["SharedApp/SharedApp.csproj", "SharedApp/"]

# Restore dependencies
RUN dotnet restore "ServerApp/ServerApp.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ServerApp"
RUN dotnet build "ServerApp.csproj" -c Release -o /app/build

# Stage 2: Publish the app
FROM build AS publish
RUN dotnet publish "ServerApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: The final Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerApp.dll"]