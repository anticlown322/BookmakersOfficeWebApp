# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 50013
EXPOSE 50003

# 2. Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2.1. Copy .csproj files
COPY ["UserService.Presentation/UserService.Presentation.csproj", "UserService.Presentation/"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]
COPY ["UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/"]

# 2.2. Restore dependencies
RUN dotnet restore "UserService.Presentation/UserService.Presentation.csproj"

# 2.3. Copy all code
COPY . .

# 2.4. Build
WORKDIR "/src/UserService.Presentation"
RUN dotnet build "UserService.Presentation.csproj" -c Release -o /app/build

# 3. Publish stage
FROM build AS publish
RUN dotnet publish "UserService.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Runtime stage
FROM base AS final
WORKDIR /app

# 4.1. Copying publish files
COPY --from=publish /app/publish .

# 4.2. Container optimizations
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "UserService.Presentation.dll"]