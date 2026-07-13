using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using BamboeUp.Report.DevExpress;
using BamboeUp.Report.FastReport;
using BamboeUp.Report.Handlers;
using BamboeUp.Report.QuestPdf;
using BamboeUp.Report.Services;
using BamboeUp.Report.Telerik;

namespace BamboeUp.Report.Engines;

public sealed class ReportHandlerSet
{
    public required IReportHandler Rpt { get; init; }
    public required IReportHandler Doc { get; init; }
    public required IReportHandler Pvt { get; init; }

    public IReportHandler Resolve(string reportKind) =>
        reportKind.ToUpperInvariant() switch
        {
            "PVT" => Pvt,
            "DOC" => Doc,
            _ => Rpt
        };
}

public static class ReportEngineBootstrap
{
    public static ReportHandlerSet CreateHandlers(
        IReportDataProvider dataProvider,
        ReportTemplateOptions? options = null)
    {
        options ??= CreateDefaultOptions();
        var registry = BuildRendererRegistry();

        return new ReportHandlerSet
        {
            Rpt = new RptReportHandler(dataProvider, registry, options),
            Doc = new DocReportHandler(dataProvider, registry, options),
            Pvt = new StubPivotHandler()
        };
    }

    public static ReportRendererRegistry BuildRendererRegistry()
    {
        var registry = new ReportRendererRegistry();
        registry.Register(new QuestPdfReportRenderer());
        registry.Register(new FastReportRenderer());
        registry.Register(new TelerikReportRenderer());
        registry.Register(new DevExpressReportRenderer());
        return registry;
    }

    public static ReportTemplateOptions CreateDefaultOptions()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Reports"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "BamboeUp.Report")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "BamboeUp.Report"))
        };

        foreach (var path in candidates)
        {
            if (Directory.Exists(path))
                return new ReportTemplateOptions { TemplateRoot = path };
        }

        return new ReportTemplateOptions();
    }
}
