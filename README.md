# EurovisionOnMars

## Development 

### Creating local database
In Package Manager Console:
1. Create migrations by running the following command
dotnet ef migrations add <nameOfMigration> --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api

2. Create database by running the following command (ensure folder "Database" in EurovisionOnMars.Entity is exists first)
dotnet ef database update --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api

## Production