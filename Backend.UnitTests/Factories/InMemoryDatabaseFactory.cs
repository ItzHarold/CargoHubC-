using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using Backend.Infrastructure.Database;

namespace Backend.UnitTests.Factories;

static class InMemoryDatabaseFactory
{
    public static CargoHubDbContext CreateMockContext(string databaseName = "")
    {
        if (databaseName == "")
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<CargoHubDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;
        var context = new CargoHubDbContext(options);
        return context;
    }

    public static async Task<bool> SeedData<T>(CargoHubDbContext context, T[] values) where T : class
    {
        await context.Set<T>().AddRangeAsync(values);
        return await context.SaveChangesAsync() > 0;
    }

    public static async Task<bool> SeedData<T>(CargoHubDbContext context, T value) where T : class
    {
        await context.Set<T>().AddAsync(value);
        return await context.SaveChangesAsync() > 0;
    }
}