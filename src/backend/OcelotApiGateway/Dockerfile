# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 5001

# 2. Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2.1. Copy .csproj files
COPY ["OcelotApiGateway/OcelotApiGateway.csproj", "OcelotApiGateway/"]

# 2.2. Restore dependencies
RUN dotnet restore "OcelotApiGateway/OcelotApiGateway.csproj"

# 2.3. Copy all code
COPY . .

# 2.4. Build
WORKDIR "/src/OcelotApiGateway"
RUN dotnet build "./OcelotApiGateway.csproj" -c Release -o /app/build

# 3. Publish stage
FROM build AS publish
RUN dotnet publish "./OcelotApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Runtime stage
FROM base AS final
WORKDIR /app

# 4.1. Copying publish files
COPY --from=publish /app/publish/. .

# 4.2. Copying ocelot config file
COPY ["ocelot.json", "./"]

# 4.2. Container optimizations
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "OcelotApiGateway.dll"]