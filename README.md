# DIO Vehicle API

This is a simple portifolio project to showcase my knowledge in OOP, DDD, .NET, EntityFramework, Docker and others.

Also an activity for the DIO course that I'm doing. Later I'll document it more.

# Docker Setup

## Quick Start

1. **Start SQL Server:**
   ```bash
   cd docker
   docker-compose up -d
   ```

2. **Stop SQL Server:**
   ```bash
   docker-compose down
   ```

## Database Connection

- **Server:** `localhost,1433`
- **Username:** `sa`
- **Password:** `Dev@12345`
- **Database:** Will be created by EF Core migrations

## Running the Application

1. **Start Docker database:**
   ```bash
   cd docker
   docker-compose up -d
   ```

2. **Run migrations (from project root):**
   ```bash
   dotnet ef database update --project src/DioVehicleApi.Infrastructure --startup-project src/DioVehicleApi.Api
   ```

3. **Start the API:**
   ```bash
   dotnet run --project src/DioVehicleApi.Api
   ```

4. **Access Swagger:** http://localhost:5197/swagger

## Connection String

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DioVehicleApiDb;User Id=sa;Password=Dev@12345;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

## Useful Commands

```bash
# View SQL Server logs
docker-compose logs sqlserver

# Connect to SQL Server directly
docker exec -it dio-vehicle-api-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Dev@12345'

# Remove everything (including data)
docker-compose down -v
```