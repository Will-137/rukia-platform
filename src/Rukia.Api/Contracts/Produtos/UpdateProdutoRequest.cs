namespace Rukia.Api.Contracts.Produtos;

public sealed record UpdateProdutoRequest(
    string Nome,
    string? Descricao,
    string? Categoria,
    string UnidadeMedida
);