# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TaskForge.API/TaskForge.API.csproj", "src/TaskForge.API/"]
COPY ["src/TaskForge.Application/TaskForge.Application.csproj", "src/TaskForge.Application/"]
COPY ["src/TaskForge.Domain/TaskForge.Domain.csproj", "src/TaskForge.Domain/"]
COPY ["src/TaskForge.Persistence/TaskForge.Persistence.csproj", "src/TaskForge.Persistence/"]
COPY ["src/TaskForge.StartupTasks/TaskForge.StartupTasks.csproj", "src/TaskForge.StartupTasks/"]
COPY ["src/TaskForge.EventProcessor/TaskForge.EventProcessor.csproj", "src/TaskForge.EventProcessor/"]
COPY ["src/TaskForge.MessageConsumer/TaskForge.MessageConsumer.csproj", "src/TaskForge.MessageConsumer/"]
RUN dotnet restore "./src/TaskForge.API/TaskForge.API.csproj"
COPY . .
WORKDIR "/src/src/TaskForge.API"
RUN dotnet build "./TaskForge.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskForge.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskForge.API.dll"]