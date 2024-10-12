using System;
using System.Collections.Generic;
using System.Linq;
using GolayCodeSimulator.Core;

namespace GolayCodeSimulator.Utilities;

public class BinaryStringConverter
{
    public static IList<byte> ToBytes(string binaryString)
    {
        List<byte> bytes = [];
        byte @byte = 0;
        int shift = 7;

        foreach (var ch in binaryString)
        {
            @byte |= (byte)(CharToBit(ch) << shift);
            
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

    public static string FromBytes(IList<byte> bytes, int multipleOf = GolayConstants.CodewordLength)
    {
        var str = string.Concat(
            bytes.Select(b => 
                Convert.ToString(b, 2).PadLeft(8, '0')
            )
        );
        
        var charsToRemove = str.Length % multipleOf;
        
        return str[..^charsToRemove];
    }

    private static byte CharToBit(char ch) => ch switch
    {
        '0' => 0,
        '1' => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, null)
    };
}