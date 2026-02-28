namespace Rukia.Api.Contracts.Produtos;

public sealed record ProdutoResponse(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao,
    string? Categoria,
    string UnidadeMedida,
    bool Ativo,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);