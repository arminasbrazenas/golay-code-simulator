namespace GolayCodeSimulator.Validators;

public static class GolayTextValidator
{
    public static ValidationResult Validate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return ValidationResult.Failure("Text is required.");
        }

        return ValidationResult.Success;
    }
}