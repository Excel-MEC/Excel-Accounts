using System;
using API.Data;
using API.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;

namespace Tests.RepositoryTests
{
    public class DbContextFixture : IDisposable
    {
        public DataContext Context;

        public DbContextFixture()
        {
            try
            {
                var connection = new SqliteConnection("Data Source=Default;Mode=Memory;Cache=Shared");
                var options = new DbContextOptionsBuilder<DataContext>()
                    .UseSqlite(connection).Options;
                Context = new DataContext(options);
                Context.Database.OpenConnection();
                Context.Database.EnsureCreated();
                SeedData.Seed(Context);
            }
            catch { }
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}