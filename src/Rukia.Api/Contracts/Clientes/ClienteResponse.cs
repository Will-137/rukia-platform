using System;

namespace Rukia.Api.Contracts.Clientes;

public sealed record ClienteResponse(
    Guid Id,
    string? Nome,
    string? Documento,
    string? Email,
    string? Telefone,
    bool Ativo,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);