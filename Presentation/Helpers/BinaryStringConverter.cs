using System;
using System.Collections.Generic;
using System.Linq;

namespace GolayCodeSimulator.Presentation.Helpers;

public static class BinaryStringConverter
{
    public static List<byte> ToBytes(string binaryString)
    {
        List<byte> bytes = [];
        byte @byte = 0;
        int shift = 7;

        foreach (char ch in binaryString)
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

    public static string FromBytes(List<byte> bytes, int multipleOf)
    {
        string str = string.Concat(bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

        int lastNCharsToRemove = str.Length % multipleOf;

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
