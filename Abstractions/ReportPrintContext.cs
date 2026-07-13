namespace BamboeUp.Report.Abstractions;

public sealed class ReportPrintContext
{
    public bool RequiresPrintId { get; init; }
    public string? ReportPrintId { get; init; }
    public string? ReportPrintIdMasked { get; init; }
    public bool IsReprint { get; init; }
    public string? ReprintOfPrintId { get; init; }
    public string? ReprintReason { get; init; }
}
