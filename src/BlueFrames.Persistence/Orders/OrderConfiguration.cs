using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Persistence.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasKey(order => order.Id);
        
        builder
            .Property(order => order.Id)
            .HasConversion(id => id.Value, value => OrderId.From(value));
        
        builder
            .Property(order => order.ProductId)
            .IsRequired()
            .HasConversion(productId => productId.Value, value => ProductId.From(value));

        builder
            .Property(order => order.CustomerId)
            .IsRequired()
            .HasConversion(customerId => customerId.Value, value => CustomerId.From(value));
        
        builder
            .Property(order => order.Status)
            .IsRequired()
            .HasConversion(status => status.Id, value => Status.From(value));

        builder
            .Property(order => order.CreatedDate)
            .IsRequired()
            .HasConversion(createdDate => createdDate.Value, value => OrderDate.From(value));
        
        builder
            .Property(order => order.UpdatedDate)
            .IsRequired(false)
            .HasConversion(updatedDate => updatedDate.Value, value => OrderDate.From(value));

        builder.Ignore(order => order.DomainEvents);
    }
}