# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 50032
EXPOSE 50022

# 2. Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2.1. Copy .csproj files
COPY ["../../SportDataService.GrpcService/SportDataService.GrpcService.csproj", "SportDataService.GrpcService/"]
COPY ["../../SportDataService.Application/SportDataService.Application.csproj", "SportDataService.Application/"]
COPY ["../../SportDataService.Domain/SportDataService.Domain.csproj", "SportDataService.Domain/"]
COPY ["../../SportDataService.Infrastructure/SportDataService.Infrastructure.csproj", "SportDataService.Infrastructure/"]

# 2.2. Restore dependencies
RUN dotnet restore "SportDataService.GrpcService/SportDataService.GrpcService.csproj"

# 2.3. Copy all code
COPY . .

# 2.4. Build
WORKDIR "/src/SportDataService.GrpcService"
RUN dotnet build "SportDataService.GrpcService.csproj" -c Release -o /app/build

# 3. Publish stage
FROM build AS publish
RUN dotnet publish "SportDataService.GrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Runtime stage
FROM base AS final
WORKDIR /app

# 4.1. Copying publish files
COPY --from=publish /app/publish .

# 4.2. Container optimizations
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "SportDataService.GrpcService.dll"]