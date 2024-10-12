using System;

namespace GolayCodeSimulator.Utilities;

public static class DoubleExtensions
{
    private const double NineDigitPrecision = 0.000_000_001;
    
    public static bool AlmostEqualTo(this double left, double right)
    {
        return Math.Abs(left - right) < NineDigitPrecision;
    }
}