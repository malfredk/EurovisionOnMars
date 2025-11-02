using EurovisionOnMars.Api.Configurations;
using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.PlayerRatings.Mapper;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Api.Features.RatingClosing;
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

// Validate and register CLOSE_RATING_TIME
var closingRatingTime = ConfigurationValidator.GetAndValidateRatingClosingTime(builder.Configuration);
builder.Services.AddSingleton(typeof(DateTimeOffset), closingRatingTime);

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

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRatingService, PlayerRatingService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IRatingGameResultService, RatingGameResultService>();
builder.Services.AddScoped<IPlayerGameResultService, PlayerGameResultService>();
builder.Services.AddScoped<IRatingClosingService, RatingClosingService>();

builder.Services.AddTransient<IDateTimeNow, DateTimeNow>();

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerRatingRepository, PlayerRatingRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IRatingGameResultRepository, RatingGameResultRepository>();
builder.Services.AddScoped<IPlayerGameResultRepository, PlayerGameResultRepository>();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddTransient<IPlayerMapper, PlayerMapper>();
builder.Services.AddTransient<IPlayerRatingMapper, PlayerRatingMapper>();
builder.Services.AddTransient<ICountryMapper, CountryMapper>();
builder.Services.AddTransient<ICountryMapper, CountryMapper>();
builder.Services.AddTransient<IPlayerGameResultMapper, PlayerGameResultMapper>();
builder.Services.AddTransient<IRatingGameResultMapper, RatingGameResultMapper>();
builder.Services.AddTransient<IPredictionMapper, PredictionMapper>();

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