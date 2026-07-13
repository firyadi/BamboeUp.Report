using BamboeUp.Report.Abstractions;

namespace BamboeUp.Report.Handlers;

public sealed class StubReportHandler : IReportHandler
{
    public string ReportKind => "RPT";

    public Task<ReportRunResult> RunAsync(ReportRunContext context, CancellationToken cancellationToken = default)
    {
        var printSuffix = context.Print.RequiresPrintId
            ? $" Print ID: {context.Print.ReportPrintId}"
            : string.Empty;

        return Task.FromResult(new ReportRunResult
        {
            Success = true,
            Message = $"Report '{context.ProgramName}' queued (engine stub).{printSuffix}",
            ReportPrintId = context.Print.ReportPrintId,
            ReportPrintIdMasked = context.Print.ReportPrintIdMasked,
            ReportExecutionGuid = Guid.NewGuid()
        });
    }
}
