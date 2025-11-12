# ============================================
# Dockerfile - Control de Vehiculos API
# .NET 8.0 Web API
# ============================================

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["ControlVehiculos.sln", "./"]
COPY ["src/ControlVehiculos/ControlVehiculos.csproj", "src/ControlVehiculos/"]
COPY ["src/ControlVehiculos.Tests/ControlVehiculos.Tests.csproj", "src/ControlVehiculos.Tests/"]

# Restore dependencies
RUN dotnet restore "ControlVehiculos.sln"

# Copy all source files
COPY . .

# Build the main project
WORKDIR "/src/src/ControlVehiculos"
RUN dotnet build "ControlVehiculos.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ControlVehiculos.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/v1/health || exit 1

ENTRYPOINT ["dotnet", "ControlVehiculos.dll"]