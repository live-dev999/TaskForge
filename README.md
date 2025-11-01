# TaskForge 
Task Management Service with Events
<!-- ![logo](/design/logo.jpg?raw=true) { height=200px } -->
<img width="250" src="design/logo.jpeg?raw=true">

# About project
The project is a test task. The full description of the title can be read here [EN](/docs/TASK-EN.MD) of [RU](/docs/TASK-RU.MD)


## Build info
Azure Build Status (master)/(dev)
| dev   |      master      |
|----------|:-------------:|
|  [![Build Status](https://dev.azure.com/live-dev/TaskForge/_apis/build/status%2FTaskForge?branchName=dev)](https://dev.azure.com/live-dev/TaskForge/_build/latest?definitionId=2&branchName=dev) | [![Build Status](https://dev.azure.com/live-dev/TaskForge/_apis/build/status%2FTaskForge?branchName=master)](https://dev.azure.com/live-dev/TaskForge/_build/latest?definitionId=2&branchName=master)  |



GitHub Build Status (master)/(dev)

| dev   |      master      |
|----------|:-------------:|
|  |  |


## **Preinstalled software**
### Windows:
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Database (choose one):
  - [PostgreSQL 15+](https://www.postgresql.org/download/) or Docker image
  - [Microsoft SQL Server 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Mac
- [Visual Studio 2022 for Mac](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio-mac/?sku=communitymac&rel=17) or [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Database (choose one):
    - [PostgreSQL 15+](https://www.postgresql.org/download/) or Docker image
    or 
    - (MSSQL/Azure SQL Edge)
        - Intel CPU
            - [Microsoft SQL Server or Docker image with (minimal version version 2019 and up)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) 
        - or Apple Silicon CPU
            - [Azure SQL Edge](https://learn.microsoft.com/en-us/azure/azure-sql-edge/disconnected-deployment)

### Linux
- [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.Net Core 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
 [Microsoft SQL Server or Docker image with (minimal version version 2019 and up)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) 



 ## **Getting Started**
### Steps: 
1. Install [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) database or deploy database using docker
2. Set environment in appSettings.json and appSettings.Development.json
3. Migrate EF CORE or deploy a database backup
4. Build and run project (use dotnet commands or use IDEs([Visual Studio 2022 / Visual Studio for Mac](https://visualstudio.microsoft.com/downloads/) or [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/))


## **Deploy databases**
Possible Database deployment scenarios:
+ use Azure SQL databse in Microsoft Azure Cloud
+ use docker or docker-compose
+ deploy local database


### Use Azure SQL databse in Microsoft Azure Cloud (main method)
To work with the database in Microsoft Azure, you need to remember to set a firewall rule for your IP address. [Firewall configuration is done through the Microsoft Azure panel.](https://learn.microsoft.com/en-us/azure/azure-sql/database/firewall-configure?view=azuresql)


### Use Docker or Docker compose(alternative method)
Run database use docker:

```
sudo docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sql1 --hostname sql1 \
   -d \
   mcr.microsoft.com/mssql/server:2019-latest
```


### Run database use docker-compose (Cross-Platform)

Docker Compose automatically detects your platform (Mac Intel/AMD, Mac Apple Silicon M1/M2/M3, Windows, Linux) and uses the appropriate SQL Server image.

#### Quick Start (Automatic Platform Detection)

**For Mac/Linux:**
```bash
# Make script executable (first time only)
chmod +x docker-compose-up.sh

# Run with automatic platform detection
./docker-compose-up.sh
```

**For Windows PowerShell:**
```powershell
.\docker-compose-up.ps1
```

**For Windows Command Prompt:**
```cmd
docker-compose-up.bat
```

#### Manual Setup (Optional)

If you prefer to set environment variables manually:

**For Intel/AMD (x86_64) systems:**
```bash
export PLATFORM=linux/amd64
export SQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest
docker-compose up
```

**For Apple Silicon (ARM64) systems:**
```bash
export PLATFORM=linux/arm64
export SQL_IMAGE=mcr.microsoft.com/azure-sql-edge:latest
docker-compose up
```

**For Windows:**
```cmd
set PLATFORM=linux/amd64
set SQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest
docker-compose up
```

#### Platform Detection Details

The docker-compose configuration automatically:
- Detects your system architecture (ARM64 or AMD64/x86_64)
- Selects the appropriate SQL Server image:
  - **AMD64/x86_64**: `mcr.microsoft.com/mssql/server:2019-latest`
  - **ARM64**: `mcr.microsoft.com/azure-sql-edge:latest`
- Sets the correct platform for all containers

All services (API, EventProcessor, SQL Server) will use the detected platform automatically.


### **Deploy local database in your machine (alternative method)**
Go to link [for download Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads). Install Microsoft SQL Server using the installer or any other method available.


### **Deploy local database in your machine (alternative method)**
Go to link [for download Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads). IInstall Microsoft SQL Server using the installer or any other method available.


## Commit Formats
#### Types
* API relevant changes
    * `feat` Commits, that adds a new feature
    * `fix` Commits, that fixes a bug
* `refactor` Commits, that rewrite/restructure your code, however does not change any behaviour
    * `perf` Commits are special `refactor` commits, that improves performance
* `style` Commits, that do not affect the meaning (white-space, formatting, missing semi-colons, etc)
* `test` Commits, that add missing tests or correcting existing tests
* `docs` Commits, that affect documentation only
* `build` Commits, that affect build components like build tool, ci pipeline, dependencies, project version, ...
* `devops` Commits, that affect operational components like infrastructure, deployment, backup, recovery, ...
* `chore` Miscellaneous commits e.g. modifying `.gitignore`

#### Subject
* use imperative, present tense (eg: use "add" instead of "added" or "adds")
* don't use dot(.) at end
* don't capitalize first letter

### Examples
* ```
  feat(service): add and setup swagger
  ```
* ```
  feat: remove ticket list endpoint
  
  refers to JIRA-999
  BREAKING CHANGES: ticket enpoints no longer supports list all entites.
  ```
* ```
  fix: add missing parameter to service call
  
  The error occurred because of <reasons>.
  ```
* ```
  build(release): bump version to 1.0.0
  ```
* ```
  build: update dependencies
  ```
* ```
  refactor: implement calculation method as recursion
  ```
* ```
  style: remove empty line


## Build and run applications
Before launching, be sure to set the variables in the appsettings.json configuration files. It is important to specify the correct database connection string
```
 "ConnectionStrings": {
    "TaskForgeConnection": "Server=tcp:<server>,<port>;Initial Catalog=<databasename>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
```
Can use commands use terminal or use IDEs(Microsoft Visual Studio 2022 or VS Code):
```
dotnet build [options]
dotnet run [options]
```


