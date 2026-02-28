using Microsoft.EntityFrameworkCore;

namespace Rukia.Infrastructure.Persistence;

public class RukiaDbContext : DbContext
{
	public RukiaDbContext(DbContextOptions<RukiaDbContext> options) : base(options) { }

	// TODO: adicionar DbSets quando existirem entidades reais no Domain
	// public DbSet<Cliente> Clientes => Set<Cliente>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// TODO: aplicar configurações Fluent quando existirem
		// modelBuilder.ApplyConfigurationsFromAssembly(typeof(RukiaDbContext).Assembly);
	}
}