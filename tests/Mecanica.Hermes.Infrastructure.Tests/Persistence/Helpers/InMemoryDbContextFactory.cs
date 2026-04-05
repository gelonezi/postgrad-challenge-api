using Mecanica.Hermes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mecanica.Hermes.Infrastructure.Tests.Persistence.Helpers;

internal static class InMemoryDbContextFactory
{
    internal static AppDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    internal static DbContextOptions<AppDbContext> CreateOptions(string databaseName)
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }
}
