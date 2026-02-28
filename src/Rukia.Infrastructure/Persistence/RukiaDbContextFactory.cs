using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Rukia.Infrastructure.Persistence;

public class RukiaDbContextFactory : IDesignTimeDbContextFactory<RukiaDbContext>
{
    public RukiaDbContext CreateDbContext(string[] args)
    {
        // Design-time: usa variável de ambiente se existir,
        // senão cai num default local (DEV).
        var cs =
            Environment.GetEnvironmentVariable("RUKIA_CONNECTIONSTRING")
            ?? "Host=localhost;Port=5432;Database=rukia_dev;Username=rukia;Password=rukia_dev_pwd;Include Error Detail=true";

        var options = new DbContextOptionsBuilder<RukiaDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new RukiaDbContext(options);
    }
}