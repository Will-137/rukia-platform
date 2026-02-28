using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rukia.Infrastructure.Persistence;
using Xunit;

namespace Rukia.Api.IntegrationTests;

public sealed class DatabaseFixture : IAsyncLifetime
{
    public string ConnectionString
    {
        get
        {
            var password = Environment.GetEnvironmentVariable("RUKIA_TEST_DB_PASSWORD");
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Defina RUKIA_TEST_DB_PASSWORD com a senha do Postgres local.");

            return $"Host=127.0.0.1;Port=5432;Database=rukia_test;Username=rukia;Password={password}";
        }
    }

    public async Task InitializeAsync()
    {
        // limpa o banco de testes para começar do zero sempre
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        // drop schema public (cascata) e recria
        var sql = """
                  DROP SCHEMA IF EXISTS public CASCADE;
                  CREATE SCHEMA public;
                  """;

        await using (var cmd = new NpgsqlCommand(sql, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // aplica migrations no banco de testes
        var options = new DbContextOptionsBuilder<RukiaDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var db = new RukiaDbContext(options);
        await db.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}