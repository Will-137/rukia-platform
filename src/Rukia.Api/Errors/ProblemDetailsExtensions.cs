using Microsoft.AspNetCore.Mvc;

namespace Rukia.Api.Errors;

public static class ProblemDetailsExtensions
{
    public static string? GetErrorCode(this ProblemDetails pd)
    {
        if (pd.Extensions.TryGetValue("errorCode", out var value))
            return value?.ToString();

        return null;
    }
}