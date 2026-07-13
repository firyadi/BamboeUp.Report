namespace BamboeUp.Report.Abstractions;

public sealed class ResolvedReportDefinition
{
    public long ReportDefinitionId { get; init; }
    public long ProgramId { get; init; }
    public string ReportScope { get; init; } = "Standard";
    public long? CompanyId { get; init; }
    public string ReportKind { get; init; } = "RPT";
    public string DefinitionKey { get; init; } = string.Empty;
    public string? FilePath { get; init; }
    public string? StoreProcedureName { get; init; }
    public bool RequiresPrintId { get; init; }
    public string? PrintIdPolicy { get; init; }
    public string? PrintIdPrefix { get; init; }
}
