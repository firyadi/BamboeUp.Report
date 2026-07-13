using BamboeUp.Report.Abstractions;

namespace BamboeUp.Report.Handlers;

public sealed class StubPivotHandler : IPivotHandler
{
    public string ReportKind => "PVT";

    public Task<ReportRunResult> RunAsync(ReportRunContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ReportRunResult
        {
            Success = true,
            Message = $"Pivot '{context.ProgramName}' queued (engine stub).",
            ReportExecutionGuid = Guid.NewGuid()
        });
    }
}
