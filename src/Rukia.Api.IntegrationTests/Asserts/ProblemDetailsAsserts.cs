using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Rukia.Api.IntegrationTests.Asserts;

public static class ProblemDetailsAsserts
{
    public static void DeveEstarEmPortuguesEComErrorCode(ProblemDetails pd)
    {
        Assert.False(string.IsNullOrWhiteSpace(pd.Title));
        Assert.False(string.IsNullOrWhiteSpace(pd.Detail));

        // Verificação simples para evitar inglês
        Assert.DoesNotContain("not found", pd.Title, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("error", pd.Title, StringComparison.OrdinalIgnoreCase);

        Assert.True(pd.Extensions.ContainsKey("errorCode"));
        Assert.False(string.IsNullOrWhiteSpace(pd.Extensions["errorCode"]?.ToString()));
    }

    public static void DeveTerErrorCode(ProblemDetails pd, string expected)
    {
        var value = pd.Extensions["errorCode"]?.ToString();
        Assert.Equal(expected, value);
    }
}