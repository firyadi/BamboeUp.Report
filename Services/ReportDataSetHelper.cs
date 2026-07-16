using System.Data;

namespace BamboeUp.Report.Services;

public static class ReportDataSetHelper
{
    public static string ResolveDataSourceName(string? definitionKey, string fallback = "ReportData")
    {
        if (!string.IsNullOrWhiteSpace(definitionKey))
            return $"{definitionKey.Trim()}Data";

        return fallback;
    }

    public static DataSet ToReportDataSet(DataTable source, string tableName = "BankData")
    {
        var copy = source.Copy();
        copy.TableName = string.IsNullOrWhiteSpace(tableName) ? "BankData" : tableName;

        var dataSet = new DataSet("ReportData");
        dataSet.Tables.Add(copy);
        return dataSet;
    }
}
