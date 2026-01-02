using EurovisionOnMars.Api.Configurations;
using EurovisionOnMars.Api.Features;
using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.GameResults;
using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Api.Features.Predictions;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Api.Middlewares;
using EurovisionOnMars.Entity.DataAccess;
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
AddPlayerGameResultsFeature(builder);
AddRatingTimeValidator(builder);
AddPlayerRatingsFeature(builder);
AddPredictionsFeature(builder);
AddPlayersFeature(builder);
AddRatingGameResultsFeature(builder);

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
    builder.Services.AddScoped<IGameResultService, GameResultService>();
}

static void AddPlayerGameResultsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IPlayerGameResultRepository, PlayerGameResultRepository>();
    builder.Services.AddTransient<IPlayerGameResultMapper, PlayerGameResultMapper>();
    builder.Services.AddScoped<IPlayerGameResultService, PlayerGameResultService>();
}

static void AddPlayerRatingsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IPlayerRatingRepository, PlayerRatingRepository>();
    builder.Services.AddTransient<IPlayerRatingMapper, PlayerRatingMapper>();
    builder.Services.AddScoped<IPlayerRatingService, PlayerRatingService>();

    builder.Services.AddScoped<IRankHandler, RankHandler>();
    builder.Services.AddScoped<ISpecialPointsValidator, SpecialPointsValidator>();
}

static void AddRatingTimeValidator(WebApplicationBuilder builder)
{
    builder.Services.AddTransient<IDateTimeNow, DateTimeNow>();
    builder.Services.AddScoped<IRatingTimeValidator, RatingTimeValidator>();
}

static void AddPredictionsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IPredictionRepository, PredictionRepository>();
    builder.Services.AddScoped<IPredictionService, PredictionService>();
}

static void AddPlayersFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
    builder.Services.AddTransient<IPlayerMapper, PlayerMapper>();
    builder.Services.AddScoped<IPlayerService, PlayerService>();
}

static void AddRatingGameResultsFeature(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IRatingGameResultRepository, RatingGameResultRepository>();
    builder.Services.AddTransient<IRatingGameResultMapper, RatingGameResultMapper>();
    builder.Services.AddScoped<IRatingGameResultService, RatingGameResultService>();
}