using MediatR;

namespace BlueFrames.Domain.Common.Domain;

public abstract class DomainEvent : INotification
{
    public Guid EventId { get; init; }
    public DateTime CreatedOn { get; init; }

    private DomainEvent()
    {
        EventId = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }
}
