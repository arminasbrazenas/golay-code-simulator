namespace GolayCodeSimulator.Core.Helpers;

public class BitWriter
{
    private readonly int _blockSize;
    private readonly uint _blockMask;

    private uint _buffer;
    private int _bufferSize;

    public BitWriter(int blockSize)
    {
        _blockSize = blockSize;
        _blockMask = Utilities.CalculateBlockMask(blockSize);
        Bytes = [];
    }

    public List<byte> Bytes { get; }

    /// <summary>
    /// Writes the given bit block.
    /// </summary>
    /// <param name="block">Bit block to write.</param>
    public void WriteBlock(uint block)
    {
        block &= _blockMask;
        _buffer |= block >> _bufferSize;

        uint newBuffer = 0;
        var newBufferSize = 0;

        if (_bufferSize + _blockSize <= 32)
        {
            _bufferSize += _blockSize;
        }
        else
        {
            var bitsWritten = 32 - _bufferSize;
            _bufferSize = 32;
            newBuffer = block << bitsWritten;
            newBufferSize = _blockSize - bitsWritten;
        }

        if (_bufferSize == 32)
        {
            Flush();
            _buffer = newBuffer;
            _bufferSize = newBufferSize;
        }
    }

    /// <summary>
    /// Flushes bytes from the buffer into the result bytes.
    /// </summary>
    public void Flush()
    {
        var shift = 24;
        while (_bufferSize > 0 && shift >= 0)
        {
            Bytes.Add((byte)(_buffer >> shift));
            _bufferSize -= 8;
            shift -= 8;
        }

        _buffer = 0;
        _bufferSize = 0;
    }
}
