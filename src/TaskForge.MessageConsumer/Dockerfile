# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем ТОЛЬКО .csproj файлы для restore (включая зависимости)
COPY ["src/TaskForge.MessageConsumer/TaskForge.MessageConsumer.csproj", "src/TaskForge.MessageConsumer/"]
COPY ["src/TaskForge.Application/TaskForge.Application.csproj", "src/TaskForge.Application/"]
COPY ["src/TaskForge.Domain/TaskForge.Domain.csproj", "src/TaskForge.Domain/"]
COPY ["src/TaskForge.Persistence/TaskForge.Persistence.csproj", "src/TaskForge.Persistence/"]
COPY ["src/TaskForge.StartupTasks/TaskForge.StartupTasks.csproj", "src/TaskForge.StartupTasks/"]

RUN dotnet restore "./src/TaskForge.MessageConsumer/TaskForge.MessageConsumer.csproj"

# Копируем исходный код (включая зависимости)
COPY ["src/TaskForge.MessageConsumer/", "src/TaskForge.MessageConsumer/"]
COPY ["src/TaskForge.Application/", "src/TaskForge.Application/"]
COPY ["src/TaskForge.Domain/", "src/TaskForge.Domain/"]
COPY ["src/TaskForge.Persistence/", "src/TaskForge.Persistence/"]
COPY ["src/TaskForge.StartupTasks/", "src/TaskForge.StartupTasks/"]

WORKDIR "/src/src/TaskForge.MessageConsumer"
RUN dotnet build "./TaskForge.MessageConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskForge.MessageConsumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

USER root
RUN apt-get update && apt-get install -y --no-install-recommends wget \
    && wget -O /wait-for-it.sh https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh \
    && chmod +x /wait-for-it.sh \
    && apt-get clean && rm -rf /var/lib/apt/lists/*
USER appuser

ENTRYPOINT ["dotnet", "TaskForge.MessageConsumer.dll"]

