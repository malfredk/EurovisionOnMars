# EurovisionOnMars

in Package Manager Console
dotnet ef migrations add <nameOfMigration> --verbose --project .\EurovisionOnMars.Entity   --startup-project .\EurovisionOnMars.Api
dotnet ef database update --verbose --project .\EurovisionOnMars.Entity   --startup-project .\EurovisionOnMars.Api