namespace BlueFrames.Persistence.Common.Extensions;

public static class ModelBuilderExtension
{
    /// <summary>
    /// Remove pluralizing table name convention to create table name in singular form.
    /// </summary>
    public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.DisplayName());
        }
    }
}

