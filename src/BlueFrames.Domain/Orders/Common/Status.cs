namespace BlueFrames.Domain.Orders.Common;

public record Status(int Id, string Name)
{
    public static Status Pending { get; } = new(1, "Pending");
    public static Status Complete { get; } = new(2, "Complete");
    public static Status Cancelled { get; } = new(3, "Cancelled");

    public override string ToString() => Name;

    public static Status From(int id)
    {
        return id switch
        {
            1 => Pending,
            2 => Complete,
            3 => Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(id), $"Invalid status id: {id}")
        };
    }
}