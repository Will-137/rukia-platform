using Rukia.Domain.Common;

namespace Rukia.Domain.Produtos.ValueObjects;

public readonly record struct CodigoProduto
{
    public const int MinLength = 1;
    public const int MaxLength = 50;

    public string Value { get; }

    private CodigoProduto(string value)
    {
        value = (value ?? string.Empty).Trim().ToUpperInvariant();

        if (value.Length < MinLength || value.Length > MaxLength)
            throw new DomainException($"Código do produto inválido. Informe entre {MinLength} e {MaxLength} caracteres.");

        Value = value;
    }

    public static CodigoProduto Create(string value) => new(value);

    public override string ToString() => Value;

    public static implicit operator string(CodigoProduto codigo) => codigo.Value;
}