using System;
using System.Linq;

namespace Rukia.Domain.Clientes
{
    public sealed class DocumentoFiscal : IEquatable<DocumentoFiscal>
    {
        public string Value { get; }

        private DocumentoFiscal(string value)
        {
            Value = value;
        }

        public static DocumentoFiscal Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Documento não pode ser vazio.", nameof(input));

            var digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.Length != 11 && digits.Length != 14)
                throw new ArgumentException("Documento deve conter 11 (CPF) ou 14 (CNPJ) dígitos (após normalização).", nameof(input));

            return new DocumentoFiscal(digits);
        }

        public static DocumentoFiscal? FromNullable(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return Create(input);
        }

        public override string ToString() => Value;

        public bool Equals(DocumentoFiscal? other)
            => other is not null && Value == other.Value;

        public override bool Equals(object? obj)
            => obj is DocumentoFiscal other && Equals(other);

        public override int GetHashCode()
            => Value.GetHashCode(StringComparison.Ordinal);
    }
}