using System.Linq;

namespace GolayCodeSimulator.Presentation.Validation;

public static class GolayEncodedMessageValidator
{
    public static ValidationResult Validate(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return ValidationResult.Failure("Encoded message is required.");
        }
            
        if (message.Any(x => x != '0' && x != '1'))
        {
            return ValidationResult.Failure("Encoded message must be binary.");
        }
            
        if (message.Length % 23 != 0)
        {
            return ValidationResult.Failure($"Encoded message length must be a multiple of 23. Current length is {message.Length}.");
        }

        return ValidationResult.Success;
    }
}