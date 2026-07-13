using BamboeUp.Report.Abstractions;

namespace BamboeUp.Report.DevExpress;

public sealed class DevExpressReportRenderer : IReportRenderer
{
    public string RendererType => ReportRendererTypes.DevExpress;

    public Task<ReportRenderResult> RenderAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken = default)
    {
#if ENABLE_DEVEXPRESS_REPORT
        return RenderWithDevExpressAsync(request, cancellationToken);
#else
        return Task.FromResult(new ReportRenderResult
        {
            Success = false,
            Message =
                "DevExpress Reporting engine is not enabled. " +
                "Copy ReportEngines.Local.props.example to ReportEngines.Local.props, " +
                "set EnableDevExpressReport=true, and restore DevExpressNETComponentsSetup-21.1.5 packages."
        });
#endif
    }

#if ENABLE_DEVEXPRESS_REPORT
    private static Task<ReportRenderResult> RenderWithDevExpressAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.ResolvedTemplatePath) || !File.Exists(request.ResolvedTemplatePath))
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"DevExpress template not found: {request.ResolvedTemplatePath}"
            });
        }

        try
        {
            using var report = DevExpress.XtraReports.UI.XtraReport.FromFile(request.ResolvedTemplatePath, true);
            if (request.Data.Tables.Count > 0)
            {
                report.DataSource = request.Data.Tables[0];
                report.DataMember = string.Empty;
            }

            foreach (var (key, value) in request.Context.Parameters)
            {
                if (report.Parameters[key] is { } parameter)
                    parameter.Value = value;
            }

            using var stream = new MemoryStream();
            report.ExportToPdf(stream);
            var fileName = Path.GetFileNameWithoutExtension(request.ResolvedTemplatePath) + ".pdf";

            return Task.FromResult(new ReportRenderResult
            {
                Success = true,
                Message = "DevExpress Report PDF generated.",
                OutputBytes = stream.ToArray(),
                OutputFileName = fileName
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"DevExpress render failed: {ex.Message}"
            });
        }
    }
#endif
}
