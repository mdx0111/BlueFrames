using System.Reflection;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Products;
using BlueFrames.Persistence.Common.Extensions;

namespace BlueFrames.Persistence.DataContext;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; private set; }
    public DbSet<Product> Products { get; private set; }
    public DbSet<Order> Orders { get; private set; }
    
    public AppDbContext(
        DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.RemovePluralizingTableNameConvention();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<Customer>().HasQueryFilter(customer => customer.IsDeleted == false);
        builder.Entity<Product>().HasQueryFilter(product => product.IsDeleted == false);

        base.OnModelCreating(builder);
    }
}