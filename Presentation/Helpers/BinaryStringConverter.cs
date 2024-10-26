using System;
using System.Collections.Generic;
using System.Linq;

namespace GolayCodeSimulator.Presentation.Helpers;

public static class BinaryStringConverter
{
    /// <summary>
    /// Converts a string, which contains only 0s and 1s, to its byte representation.
    /// </summary>
    /// <param name="binaryString">Binary string.</param>
    /// <returns>Byte representation of the binary string.</returns>
    public static List<byte> ToBytes(string binaryString)
    {
        List<byte> bytes = [];
        byte @byte = 0;
        var shift = 7;

        foreach (var ch in binaryString)
        {
            @byte |= (byte)(ch.ToBit() << shift);

            if (shift-- == 0)
            {
                bytes.Add(@byte);
                @byte = 0;
                shift = 7;
            }
        }

        if (shift != 7)
        {
            bytes.Add(@byte);
        }

        return bytes;
    }

    /// <summary>
    /// Converts a bytes to a binary string, ensuring the result is a multiple of the specified length.
    /// </summary>
    /// <param name="bytes">Bytes to convert to a binary string.</param>
    /// <param name="multipleOf">Length multiple to which the binary string should conform.</param>
    /// <returns>A binary string representation of the input bytes.</returns>
    public static string FromBytes(List<byte> bytes, int multipleOf)
    {
        var str = string.Concat(bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

        var lastNCharsToRemove = str.Length % multipleOf;

        return str[..^lastNCharsToRemove];
    }

    private static byte ToBit(this char ch) =>
        ch switch
        {
            '0' => 0,
            '1' => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, null),
        };
}
