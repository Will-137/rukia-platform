namespace Rukia.Api.Contracts.Produtos;

public sealed record CreateProdutoRequest(
    string Codigo,
    string Nome,
    string? Descricao,
    string? Categoria,
    string UnidadeMedida
);