namespace BlueFrames.Domain.Common.CombGuid;

public static class GuidProvider
{
    private const int EmbedAtIndex = 10;
    private static readonly UnixDateTimeStrategy DateTimeStrategy = new();
    
    public static Guid Create()
    {
        var value = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;

        Span<byte> guidBytes = stackalloc byte[16];
        value.TryWriteBytes(guidBytes);
        DateTimeStrategy.WriteDateTime(guidBytes[EmbedAtIndex..], timestamp);
        return new Guid(guidBytes);
    }

    public static DateTime GetTimestamp(Guid comb)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        comb.TryWriteBytes(guidBytes);
        return DateTimeStrategy.ReadDateTime(guidBytes[EmbedAtIndex..]);
    }
}