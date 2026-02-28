using Rukia.Domain.Common;

namespace Rukia.Domain.Produtos.ValueObjects;

public readonly record struct UnidadeMedida
{
    public const int MinLength = 1;
    public const int MaxLength = 10;

    public string Value { get; }

    private UnidadeMedida(string value)
    {
        value = (value ?? string.Empty).Trim().ToUpperInvariant();

        if (value.Length < MinLength || value.Length > MaxLength)
            throw new DomainException($"Unidade de medida inválida. Informe entre {MinLength} e {MaxLength} caracteres.");

        Value = value;
    }

    public static UnidadeMedida Create(string value) => new(value);

    public override string ToString() => Value;

    public static implicit operator string(UnidadeMedida unidade) => unidade.Value;
}