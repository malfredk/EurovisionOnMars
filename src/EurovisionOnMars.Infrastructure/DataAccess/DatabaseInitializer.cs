using Microsoft.Extensions.DependencyInjection;

namespace EurovisionOnMars.Infrastructure.DataAccess;

public static class DatabaseInitializer
{
    public static async Task EnsureCreatedAsync(
        IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var context = scope.ServiceProvider
            .GetRequiredService<DataContext>();

        await context.Database.EnsureCreatedAsync();
    }
}