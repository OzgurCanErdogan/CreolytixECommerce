FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set build configuration argument with default as Release
ARG BUILD_CONFIGURATION=Release

# Copy the main project file and restore dependencies
COPY ["CreolytixECommerce.API/CreolytixECommerce.API.csproj", "CreolytixECommerce.API/"]
COPY ["CreolytixECommerce.Application/CreolytixECommerce.Application.csproj", "CreolytixECommerce.Application/"]
COPY ["CreolytixECommerce.Domain/CreolytixECommerce.Domain.csproj", "CreolytixECommerce.Domain/"]
COPY ["CreolytixECommerce.Infrastructure/CreolytixECommerce.Infrastructure.csproj", "CreolytixECommerce.Infrastructure/"]

RUN dotnet restore "CreolytixECommerce.API/CreolytixECommerce.API.csproj"

# Copy the entire source code
COPY . .

# Build the application
WORKDIR "/src/CreolytixECommerce.API"
RUN dotnet build "CreolytixECommerce.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CreolytixECommerce.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage to create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for RabbitMQ and MongoDB if needed

ENTRYPOINT ["dotnet", "CreolytixECommerce.API.dll"]