namespace BlueFrames.Application.Interfaces.Common;

public interface IDateTimeService
{
    DateTime UtcNow { get; }
    DateTime StartOfToday { get; }
    DateTime EndOfToday { get; }
    DateTime Yesterday { get; }
}