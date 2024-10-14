using System.Collections.Generic;

namespace GolayCodeSimulator.Helpers;

public class BitReader
{
    private readonly List<byte> _bytes;
    private readonly int _blockSize;
    private readonly uint _blockMask;

    private int _bytesRead;
    private uint _buffer;
    private int _unreadBufferSize;

    public BitReader(List<byte> bytes, int blockSize)
    {
        _bytes = bytes;
        _blockSize = blockSize;
        _blockMask = Utilities.CalculateBlockMask(blockSize);
    }

    public uint? NextBlock()
    {
        uint block = _buffer << (32 - _unreadBufferSize);
        if (_unreadBufferSize >= _blockSize)
        {
            _unreadBufferSize -= _blockSize;
        }
        else
        {
            var bitsRead = _unreadBufferSize;
            ReadToBuffer();
            
            if (bitsRead + _unreadBufferSize < _blockSize)
            {
                return null;
            }
            
            block |= (_buffer >> bitsRead) & _blockMask;
            _unreadBufferSize -= _blockSize - bitsRead;
        }

        return block;
    }
    
    private void ReadToBuffer()
    {
        _buffer = 0;
        _unreadBufferSize = 0;
        
        while (_bytesRead < _bytes.Count && _unreadBufferSize < 32)
        {
            _buffer |= (uint)_bytes[_bytesRead++] << (24 - _unreadBufferSize);
            _unreadBufferSize += 8;
        }
    }
}