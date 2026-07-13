namespace BamboeUp.Report.Abstractions;

public interface IReportHandler
{
    string ReportKind { get; }
    Task<ReportRunResult> RunAsync(ReportRunContext context, CancellationToken cancellationToken = default);
}
