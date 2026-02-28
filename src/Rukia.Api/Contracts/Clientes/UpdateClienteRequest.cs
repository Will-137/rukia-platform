using System.ComponentModel.DataAnnotations;

namespace Rukia.Api.Contracts.Clientes
{
    public class UpdateClienteRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Nome { get; set; } = default!;

        [MaxLength(32)]
        public string? Documento { get; set; }

        [MaxLength(254)]
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? Telefone { get; set; }
    }
}