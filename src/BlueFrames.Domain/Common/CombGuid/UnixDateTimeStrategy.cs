using System.Buffers.Binary;

namespace BlueFrames.Domain.Common.CombGuid;

internal class UnixDateTimeStrategy
{
    private const int FixedNumDateBytes = 6;
    private const int RemainingBytesFromInt64 = 8 - FixedNumDateBytes;

    private static int NumDateBytes => FixedNumDateBytes;

    private DateTime MinDateTimeValue { get; } = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private DateTime MaxDateTimeValue => MinDateTimeValue.AddMilliseconds(2 ^ (8 * NumDateBytes));

    private long ToUnixTimeMilliseconds(DateTime timestamp) => (long)(timestamp.ToUniversalTime() - MinDateTimeValue).TotalMilliseconds;
    private DateTime FromUnixTimeMilliseconds(long ms) => MinDateTimeValue.AddMilliseconds(ms);

    public void WriteDateTime(Span<byte> destination, DateTime timestamp)
    {
        var ms = ToUnixTimeMilliseconds(timestamp);
        Span<byte> msBytes = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(msBytes, ms);
        msBytes[RemainingBytesFromInt64..].CopyTo(destination);
    }

    public DateTime ReadDateTime(ReadOnlySpan<byte> source)
    {
        // Attempt to convert the first 6 bytes.
        Span<byte> msBytes = stackalloc byte[8];
        source[..FixedNumDateBytes].CopyTo(msBytes[RemainingBytesFromInt64..]);
        msBytes[..RemainingBytesFromInt64].Clear();
        var ms = BinaryPrimitives.ReadInt64BigEndian(msBytes);
        return FromUnixTimeMilliseconds(ms);
    }
}