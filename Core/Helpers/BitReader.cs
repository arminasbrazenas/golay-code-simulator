namespace GolayCodeSimulator.Core.Helpers;

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

    /// <summary>
    /// Reads the next bit block.
    /// </summary>
    /// <returns>The block that was read, if there were enough bits left; otherwise, null.</returns>
    public uint? ReadNextBlock()
    {
        uint block = _buffer & _blockMask;
        if (_unreadBufferSize >= _blockSize)
        {
            _buffer <<= _blockSize;
            _unreadBufferSize -= _blockSize;
        }
        else
        {
            int bitsRead = _unreadBufferSize;
            ReadToBuffer();

            if (bitsRead + _unreadBufferSize < _blockSize)
            {
                return null;
            }

            int bitsToReadFromNewBuffer = _blockSize - bitsRead;
            block |= (_buffer >> bitsRead) & _blockMask;

            _unreadBufferSize -= bitsToReadFromNewBuffer;
            _buffer <<= bitsToReadFromNewBuffer;
        }

        return block;
    }

    /// <summary>
    /// Reads bytes into the buffer.
    /// </summary>
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
