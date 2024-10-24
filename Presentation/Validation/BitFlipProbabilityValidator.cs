using GolayCodeSimulator.Presentation.Helpers;

namespace GolayCodeSimulator.Presentation.Validation;

public static class BitFlipProbabilityValidator
{
    public static ValidationResult Validate(string? probability)
    {
        if (string.IsNullOrWhiteSpace(probability))
        {
            return ValidationResult.Failure("Bit flip probability is required.");
        }

        if (!probability.TryParseDoubleCultureInvariant(out var parsedProbability))
        {
            return ValidationResult.Failure("Bit flip probability must be a number.");
        }

        if (parsedProbability < 0 || parsedProbability > 1)
        {
            return ValidationResult.Failure("Bit flip probability must be between 0 and 1.");
        }

        return ValidationResult.Success;
    }
}
