using System.Data;

namespace BlueFrames.Persistence.Common.Extensions;

public static class DataTableExtensions
{
    public static DataTable ToDataTable<T>(this List<T> valuesList, string columnName)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add(columnName);

        foreach (var value in valuesList)
        {
            dataTable.Rows.Add(value);
        }

        return dataTable;
    }
}