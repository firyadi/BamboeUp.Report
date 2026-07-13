using System.Data;

namespace BamboeUp.Report.Abstractions;

public interface IReportDataProvider
{
    Task<DataTable> ExecuteStoredProcedureAsync(
        string storedProcedureName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default);
}
