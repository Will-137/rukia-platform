using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Rukia.Api.IntegrationTests.Tests;

[Collection("BancoDeTeste")]
[Trait("Modulo", "Produtos")]
public sealed class ProdutosProblemDetailsTests
{
    private readonly HttpClient _client;

    public ProdutosProblemDetailsTests(DatabaseFixture db)
    {
        var factory = new CustomWebApplicationFactory(db.ConnectionString);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_produto_inexistente_deve_retornar_404_em_ptbr_com_errorCode()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/produtos/{id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var pd = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(pd);

        Assert.Equal("Não encontrado", pd!.Title);
        Assert.NotNull(pd.Extensions);
        Assert.True(pd.Extensions.ContainsKey("errorCode"));
    }
}