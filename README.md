# EurovisionOnMars

This is a simple game web app for Eurovision Song Contest. It is a gamification of the classic scorecard used when watching Eurovision with friends. 

All players are entered in the same game. Instead of filling out a paper sheet, each player can rate the different countries in this web app. The rating closes before the results are revealed to prevent cheating. The host will need to manually enter the participating countries and their order*. When the Eurovision results are ready, the host can enter the rankings* and trigger calculation of the game result**.

A player gets points based on the (absolute) difference between their predicted ranking and the actual ranking. In addition, a player is rewarded (negative) bonus points if a ranking is perfect. (The bonus points amounts are inspired by the Formula 1 points system.) The player with the least amount of points wins. 

<sub>*This can be done in the database directly or through the country API which is accessible on the country page (/land).  
**This can be done through the result API which is accessible through the calculation button on the country page (/land).</sub>

## Development 

### Database
In Package Manager Console:
1. Install the dotnet-ef tool by running
`dotnet tool install --global dotnet-ef`

2. Create migrations by running the following command  
`dotnet ef migrations add <nameOfMigration> --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api`

3. Create database by running the following command (ensure folder "Database" in EurovisionOnMars.Entity exists first)  
`dotnet ef database update --verbose --project .\src\EurovisionOnMars.Entity   --startup-project .\src\EurovisionOnMars.Api`

### Web App
Simply run the web app by starting the *.Api* project with *https*.

## Production

This guide utilizes Azure. Start by creating a resource group.

### Database
While creating a SQL database, create a SQL server. 

Settings for the server:  
* Use both SQL and Microsoft Entra authentication for the server

Settings for the database:  
* Allow Azure services and resources to access this server

### Web App
Create a web app with the following settings:
* Publish: Code
* Operating system: Linux
* Basic authentication: Disable
* Continous deployment: Enable
* Enable public access: On
* Enable network injection: Off

Add connection string for the SQL database: 
* Name: Default
* Value: Server=xxxx,xxxx;Initial Catalog=xxxx;Persist Security Info=False;User ID=xxxx;Password=xxxx;

Add application settings:
* Name: RATING_CLOSING_TIME
* Value: when you want the rating to close on the format yyyy-MM-ddTHH:mm:ssZ (UTC time) or yyyy-MM-ddTHH:mm:sszzz (with timezone offset)

In *Configuration* go to *General Settings* and set
* Startup Command: dotnet EurovisionOnMars.Api.dll