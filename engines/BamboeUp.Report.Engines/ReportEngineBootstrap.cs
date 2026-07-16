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

    public static ReportTemplateOptions CreateDefaultOptions(string? configuredTemplateRoot = null)
    {
        if (!string.IsNullOrWhiteSpace(configuredTemplateRoot))
        {
            var configured = Path.GetFullPath(configuredTemplateRoot.Trim());
            if (Directory.Exists(configured))
                return new ReportTemplateOptions { TemplateRoot = configured };
        }

        var candidates = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "BamboeUp.Report")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "BamboeUp.Report")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "BamboeUp.Report")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "BamboeUp.Report")),
            Path.Combine(AppContext.BaseDirectory, "Reports"),
        };

        foreach (var path in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (Directory.Exists(path) && HasTemplateContent(path))
                return new ReportTemplateOptions { TemplateRoot = path };
        }

        foreach (var path in candidates)
        {
            if (Directory.Exists(path))
                return new ReportTemplateOptions { TemplateRoot = path };
        }

        return new ReportTemplateOptions();
    }

    private static bool HasTemplateContent(string root)
    {
        foreach (var sub in new[] { "Rpt", "Doc", "Pvt" })
        {
            var dir = Path.Combine(root, sub);
            if (!Directory.Exists(dir))
                continue;

            if (Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                .Any(IsTemplateFile))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsTemplateFile(string path)
    {
        var ext = Path.GetExtension(path);
        return ext.Equals(".frx", StringComparison.OrdinalIgnoreCase)
               || ext.Equals(".fr3", StringComparison.OrdinalIgnoreCase)
               || ext.Equals(".trdx", StringComparison.OrdinalIgnoreCase)
               || ext.Equals(".trdp", StringComparison.OrdinalIgnoreCase)
               || ext.Equals(".repx", StringComparison.OrdinalIgnoreCase);
    }
}
