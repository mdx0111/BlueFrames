namespace BlueFrames.Domain.Orders.Common;

public class Status(int id, string name)
{
    public static Status Pending { get; } = new(1, "Pending");
    public static Status Complete { get; } = new(2, "Complete");
    public static Status Cancelled { get; } = new(3, "Cancelled");

    public override string ToString() => name;
}