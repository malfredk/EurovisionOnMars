using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Api.Middlewares;
using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
}
else
{
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
}

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IRatingResultService, RatingResultService>();
builder.Services.AddScoped<IPlayerResultService, PlayerResultService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<IRateClosingService, RateClosingService>();
builder.Services.AddTransient<IDateTimeNow, DateTimeNow>();

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>(); // TODO: reconsider type of service
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IRatingResultRepository, RatingResultRepository>();
builder.Services.AddScoped<IPlayerResultRepository, PlayerResultRepository>();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddTransient<IPlayerMapper, PlayerMapper>();
builder.Services.AddTransient<IRatingMapper, RatingMapper>();
builder.Services.AddTransient<ICountryMapper, CountryMapper>();
builder.Services.AddTransient<IPlayerResultMapper, PlayerResultMapper>();
builder.Services.AddTransient<IRatingResultMapper, RatingResultMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.EnsureCreated();
    }
}

app.UseCors(policy => policy // TODO: define for production and development
    .AllowAnyOrigin() // .WithOrigins("https://localhost:7052") 
    .AllowAnyMethod()
    .WithHeaders(HeaderNames.ContentType)
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();