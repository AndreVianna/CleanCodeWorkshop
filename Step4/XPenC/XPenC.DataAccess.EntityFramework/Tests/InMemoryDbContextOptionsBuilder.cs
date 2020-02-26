using System;
using Microsoft.EntityFrameworkCore;

namespace XPenC.DataAccess.EntityFramework.Tests
{
    public static class InMemoryDbContextOptionsBuilder<TContext>
        where TContext : DbContext
    {
        public static DbContextOptions<TContext> Build()
        {
            return new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(RandomDbName)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .Options;
        }

        private static string RandomDbName => $"InMemory-{Guid.NewGuid().ToString()}";
    }
}