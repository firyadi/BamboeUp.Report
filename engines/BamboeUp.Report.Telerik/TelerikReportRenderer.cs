using System.Data;
using System.Xml;
using BamboeUp.Report.Abstractions;
using TelerikReport = global::Telerik.Reporting.Report;

namespace BamboeUp.Report.Telerik;

public sealed class TelerikReportRenderer : IReportRenderer
{
    public string RendererType => ReportRendererTypes.Telerik;

    public Task<ReportRenderResult> RenderAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken = default)
    {
#if ENABLE_TELERIK_REPORT
        return RenderWithTelerikAsync(request, cancellationToken);
#else
        return Task.FromResult(new ReportRenderResult
        {
            Success = false,
            Message =
                "Telerik Reporting engine is not enabled. " +
                "Copy ReportEngines.Local.props.example to ReportEngines.Local.props, " +
                "set EnableTelerikReport=true, and point TelerikReportingPath to your local installer Bin folder " +
                "(or add the Telerik NuGet feed)."
        });
#endif
    }

#if ENABLE_TELERIK_REPORT
    private static Task<ReportRenderResult> RenderWithTelerikAsync(
        ReportRenderRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.ResolvedTemplatePath) || !File.Exists(request.ResolvedTemplatePath))
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"Telerik template not found: {request.ResolvedTemplatePath}"
            });
        }

        try
        {
            using var report = LoadReport(request.ResolvedTemplatePath);
            BindReportData(report, request.Data);

            foreach (var (key, value) in request.Context.Parameters)
            {
                if (report.ReportParameters.Contains(key))
                    report.ReportParameters[key].Value = value;
            }

            var instanceSource = new global::Telerik.Reporting.InstanceReportSource
            {
                ReportDocument = report
            };

            var reportProcessor = new global::Telerik.Reporting.Processing.ReportProcessor();
            var result = reportProcessor.RenderReport("PDF", instanceSource, null);
            var fileName = Path.GetFileNameWithoutExtension(request.ResolvedTemplatePath) + ".pdf";

            return Task.FromResult(new ReportRenderResult
            {
                Success = true,
                Message = "Telerik Report PDF generated.",
                OutputBytes = result.DocumentBytes,
                OutputFileName = fileName
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ReportRenderResult
            {
                Success = false,
                Message = $"Telerik render failed: {ex}"
            });
        }
    }

    private static void BindReportData(TelerikReport report, DataSet data)
    {
        if (data.Tables.Count == 0)
            throw new InvalidOperationException("Telerik renderer requires at least one data table.");

        report.DataSource = data.Tables[0];
    }

    private static TelerikReport LoadReport(string templatePath)
    {
        var ext = Path.GetExtension(templatePath).ToLowerInvariant();

        return ext switch
        {
            ".trdp" => LoadTrdp(templatePath),
            ".trdx" => LoadTrdx(templatePath),
            _ => throw new NotSupportedException(
                $"Telerik template extension '{ext}' is not supported. Use .trdp or .trdx.")
        };
    }

    private static TelerikReport LoadTrdp(string templatePath)
    {
        var packager = new global::Telerik.Reporting.ReportPackager();
        using var source = File.OpenRead(templatePath);
        return (TelerikReport)packager.UnpackageDocument(source);
    }

    private static TelerikReport LoadTrdx(string templatePath)
    {
        var serializer = new global::Telerik.Reporting.XmlSerialization.ReportXmlSerializer();
        using var reader = XmlReader.Create(templatePath, new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Prohibit
        });

        return (TelerikReport)serializer.Deserialize(reader);
    }
#endif
}
