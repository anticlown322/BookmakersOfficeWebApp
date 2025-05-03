# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 50011
EXPOSE 50001

# 2. Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2.1. Copy .csproj files
COPY ["BettingService.API/BettingService.API.csproj", "BettingService.API/"]
COPY ["BettingService.BLL/BettingService.BLL.csproj", "BettingService.BLL/"]
COPY ["BettingService.DAL/BettingService.DAL.csproj", "BettingService.DAL/"]

# 2.2. Restore dependencies
RUN dotnet restore "BettingService.API/BettingService.API.csproj"

# 2.3. Copy all code
COPY . .

# 2.4. Build
WORKDIR "/src/BettingService.API"
RUN dotnet build "BettingService.API.csproj" -c Release -o /app/build

# 3. Publish stage
FROM build AS publish
RUN dotnet publish "BettingService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Runtime stage
FROM base AS final
WORKDIR /app

# 4.1. Copying publish files
COPY --from=publish /app/publish/. .

# 4.2. Container optimizations
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "BettingService.API.dll"]