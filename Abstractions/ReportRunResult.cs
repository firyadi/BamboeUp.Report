namespace BamboeUp.Report.Abstractions;

public sealed class ReportRunResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? ReportPrintId { get; init; }
    public string? ReportPrintIdMasked { get; init; }
    public Guid ReportExecutionGuid { get; init; }
    public string? OutputContentType { get; init; }
    public byte[]? OutputBytes { get; init; }
    public string? OutputFileName { get; init; }
}
