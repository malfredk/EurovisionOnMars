using EurovisionOnMars.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EurovisionOnMars.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("Default")));
        }
        else
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Default"),
                    options => options.EnableRetryOnFailure()));
        }

        return services;
    }
}