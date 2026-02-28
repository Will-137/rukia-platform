using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rukia.Domain.Clientes;

namespace Rukia.Infrastructure.Persistence.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("clientes");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");

            builder.Property(x => x.Nome)
                .HasColumnName("nome")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Documento)
                .HasColumnName("documento")
                .HasMaxLength(14)
                .HasConversion(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : DocumentoFiscal.Create(v)
                )
                .IsRequired(false);

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(254)
                .IsRequired(false);

            builder.Property(x => x.Telefone)
                .HasColumnName("telefone")
                .HasMaxLength(30)
                .IsRequired(false);

            builder.Property(x => x.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasDefaultValueSql("timezone('utc', now())")
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasDefaultValueSql("timezone('utc', now())")
                .IsRequired();

            // Índices: agora Documento é VO -> index por coluna "documento"
            builder.HasIndex("documento")
                .HasDatabaseName("ix_clientes_documento")
                .IsUnique()
                .HasFilter("documento IS NOT NULL");

            builder.HasIndex(x => x.Email)
                .HasDatabaseName("ix_clientes_email")
                .IsUnique()
                .HasFilter("email IS NOT NULL");
        }
    }
}