using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Persistence.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(product => product.Id);
        
        builder
            .Property(product => product.Id)
            .HasConversion(id => id.Value, value => ProductId.From(value));

        builder
            .Property(product => product.Name)
            .IsRequired()
            .HasConversion(productName => productName.Value, value => ProductName.From(value));

        builder
            .Property(product => product.Description)
            .IsRequired()
            .HasConversion(description => description.Value, value => ProductDescription.From(value));

        builder
            .Property(product => product.SKU)
            .IsRequired()
            .HasConversion(productSku => productSku.Value, value => ProductSku.From(value));

        builder.Ignore(product => product.DomainEvents);
    }
}