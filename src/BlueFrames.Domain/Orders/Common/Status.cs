namespace BlueFrames.Domain.Orders;

public class Status(int id, string name)
{
    public static Status Pending { get; } = new Status(1, "Pending");
    public static Status Complete { get; } = new Status(2, "Complete");
    public static Status Cancelled { get; } = new Status(3, "Cancelled");

    public override string ToString() => name;
}