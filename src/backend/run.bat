@echo off
setlocal enabledelayedexpansion

set COMPOSE_DIR=docker
set ENV_FILE=.env

set INFRA_FILES=-f %COMPOSE_DIR%/docker-compose.base.yml ^
                -f %COMPOSE_DIR%/docker-compose.postgres.yml ^
                -f %COMPOSE_DIR%/docker-compose.mongodb.yml ^
                -f %COMPOSE_DIR%/docker-compose.redis.yml ^
                -f %COMPOSE_DIR%/docker-compose.elk.yml ^
                -f %COMPOSE_DIR%/docker-compose.kafka.yml

set APP_FILES=-f %COMPOSE_DIR%/user/docker-compose.user.yml ^
              -f %COMPOSE_DIR%/user-grpc/docker-compose.user-grpc.yml ^
              -f %COMPOSE_DIR%/sport-data/docker-compose.sport-data.yml ^
              -f %COMPOSE_DIR%/sport-data-grpc/docker-compose.sport-data-grpc.yml ^
              -f %COMPOSE_DIR%/betting/docker-compose.betting.yml ^
              -f %COMPOSE_DIR%/gateway/docker-compose.gateway.yml

:menu
echo.
echo  === Bookmaker Management ===
echo  1. Build and start all
echo  2. Stop all
echo  3. Clean Docker (cache, unused)
echo  4. Exit
echo.
set /p choice="Enter your choice: "

if "%choice%"=="1" goto start_all
if "%choice%"=="2" goto stop_all
if "%choice%"=="3" goto clean_docker
if "%choice%"=="4" exit /b

:start_all
echo Checking volumes...
docker volume inspect redis_data >nul 2>&1 || docker volume create redis_data
docker volume inspect postgres_data >nul 2>&1 || docker volume create postgres_data
docker volume inspect mongodb_data >nul 2>&1 || docker volume create mongodb_data
docker volume inspect elasticsearch_data >nul 2>&1 || docker volume create elasticsearch_data

echo Starting infrastructure annd application services...
docker-compose --env-file %ENV_FILE% %INFRA_FILES% %APP_FILES% up -d --build
goto menu

:clean_docker
echo Cleaning Docker cache and unused objects...
docker system prune -f
docker builder prune -f
goto menu

:stop_all
echo Stopping all services...
docker-compose --env-file %ENV_FILE% %INFRA_FILES% %APP_FILES% down
goto menu