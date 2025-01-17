namespace BlueFrames.Domain.Common.Domain;

public abstract class AuditableEntity : Entity
{
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
}
