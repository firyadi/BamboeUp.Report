namespace BamboeUp.Report.Abstractions;

public sealed class ReportRunContext
{
    public long ProgramId { get; init; }
    public string ProgramCode { get; init; } = string.Empty;
    public string ProgramName { get; init; } = string.Empty;
    public string ReportKind { get; init; } = "RPT";
    public long? ReportDefinitionId { get; init; }
    public string? DefinitionKey { get; init; }
    public string? FilePath { get; init; }
    public string? RendererType { get; init; }
    public string? StoreProcedureName { get; init; }
    public long UserId { get; init; }
    public long? CompanyId { get; init; }
    public long? CompanyOfficeId { get; init; }
    public IReadOnlyDictionary<string, object?> Parameters { get; init; } = new Dictionary<string, object?>();
    public ReportPrintContext Print { get; init; } = new();
}
