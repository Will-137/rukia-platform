using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Rukia.Api.IntegrationTests.Tests;

[Collection("BancoDeTeste")]

[Trait("Modulo", "Produtos")]
public sealed class ProdutosEndpointsTests
{
    private readonly HttpClient _client;

    public ProdutosEndpointsTests(DatabaseFixture db)
    {
        var factory = new CustomWebApplicationFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    private sealed record CreateProdutoRequest(string Codigo, string Nome, string? Descricao, string? Categoria, string UnidadeMedida);
    private sealed record UpdateProdutoRequest(string Nome, string? Descricao, string? Categoria, string UnidadeMedida);

    private sealed record ProdutoResponse(
        Guid Id,
        string Codigo,
        string Nome,
        string? Descricao,
        string? Categoria,
        string UnidadeMedida,
        bool Ativo,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc);

    private sealed record PagedResponse<T>(List<T> Items, int Page, int PageSize, int Total);

    [Fact]
    public async Task Produto_CRUD_minimo_deve_funcionar()
    {
        // CREATE
        var create = new CreateProdutoRequest("13.24.0137", "Avental Hospitalar", "Modelo base", "Hospitalar", "UN");
        var createdResp = await _client.PostAsJsonAsync("/produtos", create);
        Assert.Equal(HttpStatusCode.Created, createdResp.StatusCode);

        var created = await createdResp.Content.ReadFromJsonAsync<ProdutoResponse>();
        Assert.NotNull(created);
        Assert.Equal("13.24.0137", created!.Codigo);
        Assert.True(created.Ativo);

        // GET BY ID
        var getResp = await _client.GetAsync($"/produtos/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // LIST (ativos)
        var listResp = await _client.GetAsync("/produtos?page=1&pageSize=20&includeInativos=false");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);

        var list = await listResp.Content.ReadFromJsonAsync<PagedResponse<ProdutoResponse>>();
        Assert.NotNull(list);
        Assert.True(list!.Total >= 1);

        // UPDATE
        var update = new UpdateProdutoRequest("Avental Hospitalar Atualizado", "Revisado", "Hospitalar", "UN");
        var putResp = await _client.PutAsJsonAsync($"/produtos/{created.Id}", update);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        var updated = await putResp.Content.ReadFromJsonAsync<ProdutoResponse>();
        Assert.NotNull(updated);
        Assert.Equal("Avental Hospitalar Atualizado", updated!.Nome);

        // DELETE lógico
        var delResp = await _client.DeleteAsync($"/produtos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        // LIST sem inativos: não deve aparecer
        var listAtivosResp = await _client.GetAsync("/produtos?page=1&pageSize=50&includeInativos=false");
        var ativos = await listAtivosResp.Content.ReadFromJsonAsync<PagedResponse<ProdutoResponse>>();
        Assert.NotNull(ativos);
        Assert.DoesNotContain(ativos!.Items, x => x.Id == created.Id);

        // LIST com inativos: deve aparecer com ativo=false
        var listAllResp = await _client.GetAsync("/produtos?page=1&pageSize=50&includeInativos=true");
        var all = await listAllResp.Content.ReadFromJsonAsync<PagedResponse<ProdutoResponse>>();
        Assert.NotNull(all);
        var found = all!.Items.Single(x => x.Id == created.Id);
        Assert.False(found.Ativo);
    }

    [Fact]
    public async Task Produto_mesmo_codigo_ativo_deve_retornar_409()
    {
        var req1 = new CreateProdutoRequest("COD-UNICO", "Produto 1", null, null, "UN");
        var req2 = new CreateProdutoRequest("COD-UNICO", "Produto 2", null, null, "UN");

        var r1 = await _client.PostAsJsonAsync("/produtos", req1);
        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);

        var r2 = await _client.PostAsJsonAsync("/produtos", req2);
        Assert.Equal(HttpStatusCode.Conflict, r2.StatusCode);
    }
}