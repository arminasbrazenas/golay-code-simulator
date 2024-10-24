using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;

namespace GolayCodeSimulator.Presentation.Validation;

public record ValidationResult
{
    public bool IsValid { get; private init; }

    [MemberNotNullWhen(false, nameof(IsValid))]
    public string? Error { get; private init; }

    public static ValidationResult Success => new() { IsValid = true };

    public static ValidationResult Failure(string error) => new() { IsValid = false, Error = error };

    public void ThrowOnFailure()
    {
        if (!IsValid)
        {
            throw new DataValidationException(Error);
        }
    }
}
