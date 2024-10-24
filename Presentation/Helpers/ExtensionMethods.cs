using System;
using System.Globalization;

namespace GolayCodeSimulator.Presentation.Helpers;

public static class ExtensionMethods
{
    public static bool TryParseDoubleCultureInvariant(this string probability, out double parsedProbability)
    {
        probability = probability.Replace(',', '.');
        return double.TryParse(probability, CultureInfo.InvariantCulture, out parsedProbability);
    }

    public static double ParseDoubleCultureInvariant(this string probability)
    {
        var isSuccess = TryParseDoubleCultureInvariant(probability, out var parsedProbability);
        return isSuccess ? parsedProbability : throw new ArgumentException($"Failed to parse decimal: {probability}");
    }
}
