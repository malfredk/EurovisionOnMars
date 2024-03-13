# EurovisionOnMars

## create local database 

in Package Manager Console
dotnet ef migrations add <nameOfMigration> --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api
example:
dotnet ef migrations add Initial --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api

create folder "Database" in EurovisionOnMars.Entity
dotnet ef database update --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api