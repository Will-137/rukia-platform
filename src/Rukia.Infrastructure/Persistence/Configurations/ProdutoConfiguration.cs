using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rukia.Domain.Produtos;
using Rukia.Domain.Produtos.ValueObjects;

namespace Rukia.Infrastructure.Persistence.Configurations;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        //  ValueConverters canônicos para VOs
        var codigoConverter = new ValueConverter<CodigoProduto, string>(
            v => v.Value,
            v => CodigoProduto.Create(v)
        );

        var unidadeConverter = new ValueConverter<UnidadeMedida, string>(
            v => v.Value,
            v => UnidadeMedida.Create(v)
        );

        builder.Property(x => x.Codigo)
            .HasColumnName("codigo")
            .HasMaxLength(50)
            .HasConversion(codigoConverter)
            .IsRequired();

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.Categoria)
            .HasColumnName("categoria")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.UnidadeMedida)
            .HasColumnName("unidade_medida")
            .HasMaxLength(10)
            .HasConversion(unidadeConverter)
            .IsRequired();

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

        // Índice único canônico (somente ativo)
        builder.HasIndex(x => x.Codigo)
            .HasDatabaseName("ix_produtos_codigo")
            .IsUnique()
            .HasFilter("ativo = true");
    }
}