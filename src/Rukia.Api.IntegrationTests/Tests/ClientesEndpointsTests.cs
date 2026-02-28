using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Rukia.Api.IntegrationTests.Tests;


[Collection("BancoDeTeste")]

[Trait("Modulo", "Clientes")]
public sealed class ClientesEndpointsTests
{
    private readonly HttpClient _client;

    public ClientesEndpointsTests(DatabaseFixture db)
    {
        var factory = new CustomWebApplicationFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    private sealed record CreateClienteRequest(string Nome, string? Documento, string? Email, string? Telefone);
    private sealed record UpdateClienteRequest(string Nome, string? Documento, string? Email, string? Telefone);

    private sealed record ClienteResponse(
        Guid Id,
        string? Nome,
        string? Documento,
        string? Email,
        string? Telefone,
        bool Ativo,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc);

    private sealed record PagedResponse<T>(List<T> Items, int Page, int PageSize, int Total);

    [Fact]
    public async Task Cliente_CRUD_minimo_deve_funcionar()
    {
        var create = new CreateClienteRequest("Cliente Teste", "12345678000191", "teste@empresa.com", "+55 11 99999-0000");
        var createdResp = await _client.PostAsJsonAsync("/clientes", create);
        Assert.Equal(HttpStatusCode.Created, createdResp.StatusCode);

        var created = await createdResp.Content.ReadFromJsonAsync<ClienteResponse>();
        Assert.NotNull(created);
        Assert.True(created!.Ativo);

        var getResp = await _client.GetAsync($"/clientes/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        var update = new UpdateClienteRequest("Cliente Teste Atualizado", "12345678000191", "teste@empresa.com", "+55 11 99999-0000");
        var putResp = await _client.PutAsJsonAsync($"/clientes/{created.Id}", update);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        var delResp = await _client.DeleteAsync($"/clientes/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var listAtivosResp = await _client.GetAsync("/clientes?page=1&pageSize=50&includeInativos=false");
        var ativos = await listAtivosResp.Content.ReadFromJsonAsync<PagedResponse<ClienteResponse>>();
        Assert.NotNull(ativos);
        Assert.DoesNotContain(ativos!.Items, x => x.Id == created.Id);

        var listAllResp = await _client.GetAsync("/clientes?page=1&pageSize=50&includeInativos=true");
        var all = await listAllResp.Content.ReadFromJsonAsync<PagedResponse<ClienteResponse>>();
        Assert.NotNull(all);
        var found = all!.Items.Single(x => x.Id == created.Id);
        Assert.False(found.Ativo);
    }

    [Fact]
    public async Task Cliente_documento_duplicado_deve_retornar_409_quando_ativo()
    {
        var req1 = new CreateClienteRequest("Cliente 1", "12345678000190", null, null);
        var req2 = new CreateClienteRequest("Cliente 2", "12345678000190", null, null);

        var r1 = await _client.PostAsJsonAsync("/clientes", req1);
        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);

        var r2 = await _client.PostAsJsonAsync("/clientes", req2);
        Assert.Equal(HttpStatusCode.Conflict, r2.StatusCode);
    }
}