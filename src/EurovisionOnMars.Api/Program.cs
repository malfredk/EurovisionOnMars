using EurovisionOnMars.Api.Configurations;
using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;
using EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Api.Features.Players.CreatePlayer;
using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Api.Features.Players.GetRatingGameResults;
using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Api.Features.Players.RateCountry;
using EurovisionOnMars.Api.Features.Players.ResolveTieBreak;
using EurovisionOnMars.Api.Middlewares;
using EurovisionOnMars.Domain.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RegisterRatingClosingTime(builder);
AddDbContext(builder);

AddCountriesFeature(builder);
AddGameResultsFeature(builder);
AddPlayerFeature(builder);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Database.EnsureCreated();
    }
}

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .WithHeaders(HeaderNames.ContentType)
);

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

static void RegisterRatingClosingTime(WebApplicationBuilder builder)
{
    var ratingClosingTime = ConfigurationValidator.GetAndValidateRatingClosingTime(builder.Configuration);
    builder.Services.AddSingleton(typeof(DateTimeOffset), ratingClosingTime);
}

static void AddDbContext(WebApplicationBuilder builder)
{
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
    }
    else
    {
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("Default"),
                options => options.EnableRetryOnFailure()
                )
            );
    }
}

static void AddCountriesFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ICountryRepository, CountryRepository>();
    builder.Services.AddTransient<ICountryMapper, CountryMapper>();
    builder.Services.AddScoped<ICountryService, CountryService>();
}

static void AddGameResultsFeature(WebApplicationBuilder builder)
{
    AddCalculateGameResultsFeature(builder);
    AddGetPlayerGameResultsFeature(builder);
}

static void AddCalculateGameResultsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ICalculateGameResultsRepository, CalculateGameResultsRepository>();
    builder.Services.AddScoped<ICalculateGameResultsService, CalculateGameResultsService>();
}

static void AddGetPlayerGameResultsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IGetPlayerGameResultsRepository, GetPlayerGameResultsRepository>();
    builder.Services.AddTransient<IPlayerGameResultMapper, PlayerGameResultMapper>();
    builder.Services.AddScoped<IGetPlayerGameResultsService, GetPlayerGameResultsService>();
}

static void AddPlayerFeature(WebApplicationBuilder builder)
{
    AddRatingTimeValidator(builder);
    AddCreatePlayerFeature(builder);
    AddGetPlayerFeature(builder);
    AddGetRatingGameResultsFeature(builder);
    AddGetRatingsFeature(builder);
    AddRateCountryFeature(builder);
    AddResolveTieBreakFeature(builder);
}

static void AddRatingTimeValidator(WebApplicationBuilder builder)
{
    builder.Services.AddTransient<IDateTimeNow, DateTimeNow>();
    builder.Services.AddScoped<IRatingTimeValidator, RatingTimeValidator>();
}

static void AddCreatePlayerFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ICreatePlayerRepository, CreatePlayerRepository>();
    builder.Services.AddScoped<ICreatePlayerService, CreatePlayerService>();
}

static void AddGetPlayerFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IGetPlayerRepository, GetPlayerRepository>();
    builder.Services.AddTransient<IPlayerMapper, PlayerMapper>();
    builder.Services.AddScoped<IGetPlayerService, GetPlayerService>();
}

static void AddGetRatingGameResultsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IGetRatingGameResultsRepository, GetRatingGameResultsRepository>();
    builder.Services.AddTransient<IRatingGameResultMapper, RatingGameResultMapper>();
    builder.Services.AddScoped<IGetRatingGameResultsService, GetRatingGameResultsService>();
}

static void AddGetRatingsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IGetRatingsRepository, GetRatingsRepository>();
    builder.Services.AddTransient<IPlayerRatingMapper, PlayerRatingMapper>();
    builder.Services.AddScoped<IGetRatingsService, GetRatingsService>();
}

static void AddRateCountryFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IRateCountryRepository, RateCountryRepository>();
    builder.Services.AddScoped<IRateCountryService, RateCountryService>();
}

static void AddResolveTieBreakFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IResolveTieBreakRepository, ResolveTieBreakRepository>();
    builder.Services.AddScoped<IResolveTieBreakService, ResolveTieBreakService>();
}