using EurovisionOnMars.Api.Configurations;
using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.GameResults;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Api.Middlewares;
using EurovisionOnMars.Infrastructure;
using EurovisionOnMars.Infrastructure.DataAccess;
using Microsoft.Net.Http.Headers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RegisterRatingClosingTime(builder);
builder.Services.AddInfrastructure(
    builder.Configuration,
    builder.Environment);

builder.Services
    .AddCountriesFeatures()
    .AddGameResultsFeatures()
    .AddPlayersFeatures();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    await DatabaseInitializer.EnsureCreatedAsync(app.Services);
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