using Microsoft.EntityFrameworkCore;
using Rukia.Domain.Clientes;
using Rukia.Domain.Produtos;

namespace Rukia.Infrastructure.Persistence
{
	public class RukiaDbContext : DbContext
	{
		public RukiaDbContext(DbContextOptions<RukiaDbContext> options) : base(options) { }

        
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Produto> Produtos => Set<Produto>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Se já existir ApplyConfigurationsFromAssembly, mantenha.
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(RukiaDbContext).Assembly);

			// Se você já estiver usando UseSnakeCaseNamingConvention no DI, não precisa aqui.
			// (De qualquer forma, nossa config define nomes explicitamente.)
		}
	}
}

