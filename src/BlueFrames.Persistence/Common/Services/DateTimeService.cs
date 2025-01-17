using BlueFrames.Application.Interfaces.Common;

namespace BlueFrames.Persistence.Common.Services;

internal class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime StartOfToday => new (UtcNow.Year, UtcNow.Month, UtcNow.Day);
    public DateTime EndOfToday => StartOfToday.AddDays(1).AddSeconds(-1);
    public DateTime Yesterday => StartOfToday.AddDays(-1);
}