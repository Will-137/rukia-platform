using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Rukia.Api.Errors;

public static class ApiProblemDetails
{
    public static ProblemDetails Create(int status, string title, string detail, string errorCode)
    {
        var pd = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        pd.Extensions["errorCode"] = errorCode;
        return pd;
    }

    public static ValidationProblemDetails CreateValidation(ModelStateDictionary modelState, string errorCode)
    {
        var vpd = new ValidationProblemDetails(modelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro de validação",
            Detail = "A requisição contém dados inválidos."
        };

        vpd.Extensions["errorCode"] = errorCode;
        return vpd;
    }
}