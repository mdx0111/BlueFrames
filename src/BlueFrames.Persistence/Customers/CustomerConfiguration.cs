using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Persistence.Customers;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .HasKey(customer => customer.Id);
        
        builder
            .Property(customer => customer.Id)
            .HasConversion(id => id.Value, value => CustomerId.From(value));

        builder
            .Property(customer => customer.FirstName)
            .IsRequired()
            .HasConversion(firstName => firstName.Value, value => FirstName.From(value));

        builder
            .Property(customer => customer.LastName)
            .IsRequired()
            .HasConversion(lastName => lastName.Value, value => LastName.From(value));

        builder
            .Property(customer => customer.Phone)
            .IsRequired()
            .HasConversion(phoneNumber => phoneNumber.Value, value => PhoneNumber.From(value));

        builder
            .Property(customer => customer.Email)
            .IsRequired()
            .HasConversion(email => email.Value, value => Email.From(value));

        builder
            .HasMany(customer => customer.Orders)
            .WithOne()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(customer => customer.DomainEvents);
    }
}