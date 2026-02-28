using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rukia.Domain.Common;

namespace Rukia.Api.Errors;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            // Regra de domínio (nome inválido, código inválido, etc.)
            await WriteProblemDetailsAsync(
                context,
                status: StatusCodes.Status400BadRequest,
                title: "Erro de validação",
                detail: ex.Message,
                errorCode: ResolveDadosInvalidosCode(context)
            );
        }
        catch (ArgumentException ex)
        {
            // Argumentos inválidos (ex: ValueObjects Create())
            await WriteProblemDetailsAsync(
                context,
                status: StatusCodes.Status400BadRequest,
                title: "Erro de validação",
                detail: ex.Message,
                errorCode: ResolveDadosInvalidosCode(context)
            );
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // Violação de unicidade (ex: documento duplicado, código duplicado)
            await WriteProblemDetailsAsync(
                context,
                status: StatusCodes.Status409Conflict,
                title: "Conflito de dados",
                detail: ResolveUniqueViolationDetail(context),
                errorCode: ResolveUniqueViolationCode(context)
            );
        }
        catch (Exception)
        {
            // Falha inesperada
            await WriteProblemDetailsAsync(
                context,
                status: StatusCodes.Status500InternalServerError,
                title: "Erro interno",
                detail: "Ocorreu um erro inesperado. Tente novamente.",
                errorCode: ErrorCodes.InfraErroInterno
            );
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation;

    private static Task WriteProblemDetailsAsync(
        HttpContext context,
        int status,
        string title,
        string detail,
        string errorCode)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var pd = ApiProblemDetails.Create(
            status: status,
            title: title,
            detail: detail,
            errorCode: errorCode
        );

        return context.Response.WriteAsJsonAsync(pd);
    }

    private static string ResolveDadosInvalidosCode(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value?.ToLowerInvariant() ?? "";

        if (path.StartsWith("/clientes"))
            return ErrorCodes.ClienteDadosInvalidos;

        if (path.StartsWith("/produtos"))
            return ErrorCodes.ProdutoDadosInvalidos;

        // fallback seguro
        return ErrorCodes.ValidacaoRequisicaoInvalida;
    }

    private static string ResolveUniqueViolationCode(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value?.ToLowerInvariant() ?? "";

        if (path.StartsWith("/clientes"))
            return ErrorCodes.ClienteDocumentoDuplicado;

        if (path.StartsWith("/produtos"))
            return ErrorCodes.ProdutoCodigoDuplicado;

        return ErrorCodes.ConflitoDados;
    }

    private static string ResolveUniqueViolationDetail(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value?.ToLowerInvariant() ?? "";

        if (path.StartsWith("/clientes"))
            return "Já existe um cliente ativo com o mesmo documento.";

        if (path.StartsWith("/produtos"))
            return "Já existe um produto ativo com o mesmo código.";

        return "Conflito de dados. Já existe um registro com os mesmos dados únicos.";
    }
}