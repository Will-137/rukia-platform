using Rukia.Domain.Common;
using Rukia.Domain.Produtos.ValueObjects;

namespace Rukia.Domain.Produtos;

public class Produto
{
    // EF Core
    private Produto() { }

    public Guid Id { get; private set; }

    public CodigoProduto Codigo { get; private set; }
    public string Nome { get; private set; } = default!;
    public string? Descricao { get; private set; }
    public string? Categoria { get; private set; }
    public UnidadeMedida UnidadeMedida { get; private set; }

    public bool Ativo { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public Produto(
        Guid id,
        CodigoProduto codigo,
        string nome,
        string? descricao,
        string? categoria,
        UnidadeMedida unidadeMedida)
    {
        if (id == Guid.Empty)
            throw new DomainException("Id do produto é obrigatório.");

        Id = id;
        Codigo = codigo;

        Nome = Normalize(nome);
        if (Nome.Length < 3 || Nome.Length > 200)
            throw new DomainException("Nome do produto inválido. Informe entre 3 e 200 caracteres.");

        Descricao = NormalizeNullable(descricao);
        if (Descricao is not null && Descricao.Length > 1000)
            throw new DomainException("Descrição do produto inválida. Máximo de 1000 caracteres.");

        Categoria = NormalizeNullable(categoria);
        if (Categoria is not null && Categoria.Length > 100)
            throw new DomainException("Categoria do produto inválida. Máximo de 100 caracteres.");

        UnidadeMedida = unidadeMedida;

        Ativo = true;

        var now = DateTime.UtcNow;
        CreatedAtUtc = now;
        UpdatedAtUtc = now;
    }

    //  Atualização canônica
    public void AtualizarDados(
        string nome,
        string? descricao,
        string? categoria,
        UnidadeMedida unidadeMedida)
    {
        var novoNome = Normalize(nome);
        if (novoNome.Length < 3 || novoNome.Length > 200)
            throw new DomainException("Nome do produto inválido. Informe entre 3 e 200 caracteres.");

        var novaDescricao = NormalizeNullable(descricao);
        if (novaDescricao is not null && novaDescricao.Length > 1000)
            throw new DomainException("Descrição do produto inválida. Máximo de 1000 caracteres.");

        var novaCategoria = NormalizeNullable(categoria);
        if (novaCategoria is not null && novaCategoria.Length > 100)
            throw new DomainException("Categoria do produto inválida. Máximo de 100 caracteres.");

        Nome = novoNome;
        Descricao = novaDescricao;
        Categoria = novaCategoria;
        UnidadeMedida = unidadeMedida;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Desativar()
    {
        if (!Ativo) return;

        Ativo = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Ativar()
    {
        if (Ativo) return;

        Ativo = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string Normalize(string value)
        => (value ?? string.Empty).Trim();

    private static string? NormalizeNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}