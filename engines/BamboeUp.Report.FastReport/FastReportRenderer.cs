using System.Data;
using BamboeUp.Report.Abstractions;
using FastReport.Export.PdfSimple;
using FrReport = FastReport.Report;

namespace BamboeUp.Report.FastReport;

public sealed class FastReportRenderer : IReportRenderer
{
    public string RendererType => ReportRendererTypes.FastReport;

    public Task<ReportRenderResult> RenderAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.ResolvedTemplatePath))
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = "FastReport template path is required."
            });
        }

        if (!File.Exists(request.ResolvedTemplatePath))
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"FastReport template not found: {request.ResolvedTemplatePath}"
            });
        }

        try
        {
            using var report = new FrReport();
            report.Load(request.ResolvedTemplatePath);
            RegisterData(report, request.Data, request.Context.Parameters);
            report.Prepare();

            using var export = new PDFSimpleExport();
            using var stream = new MemoryStream();
            report.Export(export, stream);

            var fileName = Path.GetFileNameWithoutExtension(request.ResolvedTemplatePath) + ".pdf";
            return Task.FromResult(new ReportRenderResult
            {
                Success = true,
                Message = "FastReport PDF generated.",
                OutputBytes = stream.ToArray(),
                OutputFileName = fileName
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"FastReport render failed: {ex.Message}"
            });
        }
    }

    private static void RegisterData(FrReport report, DataSet data, IReadOnlyDictionary<string, object?> parameters)
    {
        foreach (DataTable table in data.Tables)
            report.RegisterData(table, table.TableName);

        foreach (var (key, value) in parameters)
            report.SetParameterValue(key, value ?? DBNull.Value);
    }
}
