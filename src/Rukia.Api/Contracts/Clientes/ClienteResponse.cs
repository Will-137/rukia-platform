using System;

namespace Rukia.Api.Contracts.Clientes
{
    public class ClienteResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = default!;
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}