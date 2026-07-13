using System.Data;
using BamboeUp.Report.Abstractions;
using BamboeUp.Report.QuestPdf.Rendering;
using QuestPDF.Infrastructure;

namespace BamboeUp.Report.QuestPdf;

public sealed class QuestPdfReportRenderer : IReportRenderer
{
    static QuestPdfReportRenderer()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public string RendererType => ReportRendererTypes.QuestPdf;

    public Task<ReportRenderResult> RenderAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var table = request.Data.Tables.Count > 0
            ? request.Data.Tables[0]
            : throw new InvalidOperationException("QuestPDF renderer requires at least one data table.");

        if (table.Rows.Count == 0)
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = "No data rows available for QuestPDF rendering."
            });
        }

        try
        {
            var pdfBytes = DocPdfRenderer.Render(request.Context, table);
            var fileName = string.IsNullOrWhiteSpace(request.Context.DefinitionKey)
                ? "report.pdf"
                : $"{request.Context.DefinitionKey}.pdf";

            return Task.FromResult(new ReportRenderResult
            {
                Success = true,
                Message = "QuestPDF document generated.",
                OutputBytes = pdfBytes,
                OutputFileName = fileName
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"QuestPDF render failed: {ex.Message}"
            });
        }
    }
}
