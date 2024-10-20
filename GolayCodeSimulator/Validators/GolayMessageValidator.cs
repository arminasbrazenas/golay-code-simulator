using System.Linq;

namespace GolayCodeSimulator.Validators;

public static class GolayMessageValidator
{
    public static ValidationResult Validate(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return ValidationResult.Failure("Message is required.");
        }
            
        if (message.Any(x => x != '0' && x != '1'))
        {
            return ValidationResult.Failure("Message must be binary.");
        }
            
        if (message.Length % 12 != 0)
        {
            return ValidationResult.Failure($"Message length must be a multiple of 12. Current length is {message.Length}.");
        }

        return ValidationResult.Success;
    }
}