using System.Collections.Generic;

namespace GolayCodeSimulator.Core;

public class GolayDecoder
{
    private const int CodewordLength = 23;

    private IList<byte> _encodedMessage = [];
    private int _messageBytesRead;
    private uint _buffer;
    private int _bufferSize;
    private int _bufferAvailableBits;

    private void Reset(IList<byte> encodedMessage)
    {
        _encodedMessage = encodedMessage;
        _messageBytesRead = 0;
        _buffer = 0;
        _bufferSize = 0;
        _bufferAvailableBits = 0;
    }

    public IList<byte> Decode(IList<byte> encodedMessage) // dekodavimas nuo 22:45
    {
        Reset(encodedMessage);

        List<byte> decodedMessage = [];
        
        while (true)
        {
            var codeword = GetNextCodeword();
            if (!codeword.HasValue)
            {
                break;
            }

            var oddWeightWord = FormOddWeightWord(codeword.Value);
            var a = 5;
        }

        return decodedMessage;
    }

    private static uint FormOddWeightWord(uint codeword)
    {
        var setBitsCount = Utilities.CountSetBits(codeword);
        return setBitsCount % 2 == 0 ? codeword | (1 << 8) : codeword;
    }

    private uint? GetNextCodeword()
    {
        uint block = _buffer << (32 - _bufferAvailableBits);
        if (_bufferAvailableBits >= CodewordLength)
        {
            _bufferAvailableBits -= CodewordLength;
        }
        else
        {
            var bitsRead = _bufferAvailableBits;
            ReadToBuffer();
            
            if (bitsRead + _bufferAvailableBits < CodewordLength)
            {
                return null;
            }
            
            block |= (_buffer >> bitsRead) & 0xFF_FF_FE_00;
            _bufferAvailableBits -= CodewordLength - bitsRead;
        }

        return block;
    }

    private void ReadToBuffer()
    {
        _buffer = 0;
        _bufferSize = 0;
        
        while (_messageBytesRead < _encodedMessage.Count && _bufferSize < 32)
        {
            _buffer |= (uint)_encodedMessage[_messageBytesRead++] << (24 - _bufferSize);
            _bufferSize += 8;
        }

        _bufferAvailableBits = _bufferSize;
    }
}