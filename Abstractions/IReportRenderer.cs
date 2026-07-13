using System.Data;

namespace BamboeUp.Report.Abstractions;

public interface IReportRenderer
{
    /// <summary>Matches <see cref="ReportRendererTypes"/> constant.</summary>
    string RendererType { get; }

    Task<ReportRenderResult> RenderAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class ReportRenderRequest
{
    public required ReportRunContext Context { get; init; }
    public required DataSet Data { get; init; }
    public string? ResolvedTemplatePath { get; init; }
}

public sealed class ReportRenderResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string OutputContentType { get; init; } = "application/pdf";
    public byte[]? OutputBytes { get; init; }
    public string? OutputFileName { get; init; }
}
