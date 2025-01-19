using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlueFrames.Api.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromMultiSourceAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource { get; } = CompositeBindingSource.Create(
        [
            BindingSource.Path,
            BindingSource.Query
        ],
        nameof(FromMultiSourceAttribute));
}
