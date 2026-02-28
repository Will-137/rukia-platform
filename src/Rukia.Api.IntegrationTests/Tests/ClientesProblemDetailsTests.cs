using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Rukia.Api.IntegrationTests.Asserts;
using Xunit;

namespace Rukia.Api.IntegrationTests.Tests;

[Collection("BancoDeTeste")]
[Trait("Modulo", "Clientes")]
public sealed class ClientesProblemDetailsTests
{
    private readonly HttpClient _client;

    public ClientesProblemDetailsTests(DatabaseFixture db)
    {
        var factory = new CustomWebApplicationFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    private sealed record CreateClienteRequest(
        string Nome,
        string? Documento,
        string? Email,
        string? Telefone
    );

    private static string NovoDocumento14()
    {
        // 14 dígitos, apenas números. Prefixo fixo + sufixo de 6 dígitos.
        // Ex: 12345678000100 .. 12345678000199 etc
        var suffix = (int)(DateTime.UtcNow.Ticks % 1_000_000); // 0..999999
        return $"12345678{suffix:000000}"; // 8 + 6 = 14
    }

    [Fact]
    public async Task Get_cliente_inexistente_deve_retornar_404_em_ptbr_com_errorCode()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/clientes/{id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var pd = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);

        ProblemDetailsAsserts.DeveEstarEmPortuguesEComErrorCode(pd!);
        ProblemDetailsAsserts.DeveTerErrorCode(pd!, "CLIENTE_NAO_ENCONTRADO");
    }

    [Fact]
    public async Task Cliente_com_nome_invalido_deve_retornar_400_com_errorCode()
    {
        var req = new CreateClienteRequest(
            Nome: "A", // inválido (menos que 3)
            Documento: NovoDocumento14(),
            Email: null,
            Telefone: null
        );

        var response = await _client.PostAsJsonAsync("/clientes", req);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var pd = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);

        ProblemDetailsAsserts.DeveEstarEmPortuguesEComErrorCode(pd!);

        // Como é validação automática (ModelState), o esperado é:
        ProblemDetailsAsserts.DeveTerErrorCode(pd!, "VALIDACAO_REQUISICAO_INVALIDA");
    }

    [Fact]
    public async Task Cliente_documento_duplicado_ativo_deve_retornar_409_com_errorCode()
    {
        var doc = NovoDocumento14();

        var req1 = new CreateClienteRequest("Cliente 1", doc, null, null);
        var req2 = new CreateClienteRequest("Cliente 2", doc, null, null);

        var r1 = await _client.PostAsJsonAsync("/clientes", req1);
        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);

        var r2 = await _client.PostAsJsonAsync("/clientes", req2);
        Assert.Equal(HttpStatusCode.Conflict, r2.StatusCode);

        var pd = await r2.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);

        ProblemDetailsAsserts.DeveEstarEmPortuguesEComErrorCode(pd!);
        ProblemDetailsAsserts.DeveTerErrorCode(pd!, "CLIENTE_DOCUMENTO_DUPLICADO");
    }
}