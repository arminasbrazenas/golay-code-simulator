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

    public void WriteBlock(uint value)
    {
        value &= _blockMask;
        _buffer |= value >> _bufferSize;

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
            newBuffer = value << bitsWritten;
            newBufferSize = _blockSize - bitsWritten;
        }

        if (_bufferSize == 32)
        {
            Flush();
            _buffer = newBuffer;
            _bufferSize = newBufferSize;
        }
    }

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
