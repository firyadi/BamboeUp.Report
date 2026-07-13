using System.Data;
using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using BamboeUp.Report.Services;

namespace BamboeUp.Report.Handlers;

/// <summary>Runs catalog reports (RPT) via stored procedure + renderer plug-in.</summary>
public sealed class RptReportHandler : IReportHandler
{
    private readonly IReportDataProvider _dataProvider;
    private readonly ReportRendererRegistry _rendererRegistry;
    private readonly ReportTemplatePathResolver _templateResolver;

    public RptReportHandler(
        IReportDataProvider dataProvider,
        ReportRendererRegistry rendererRegistry,
        ReportTemplateOptions templateOptions)
    {
        _dataProvider = dataProvider;
        _rendererRegistry = rendererRegistry;
        _templateResolver = new ReportTemplatePathResolver(templateOptions);
    }

    public string ReportKind => "RPT";

    public async Task<ReportRunResult> RunAsync(ReportRunContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.StoreProcedureName))
        {
            return Fail("Report definition is missing stored procedure name.");
        }

        if (!ReportRendererTypes.TryResolve(context.RendererType, context.FilePath, context.ReportKind, out var rendererType))
        {
            return Fail("Report definition is missing RendererType (or recognizable template extension).");
        }

        try
        {
            if (rendererType != ReportRendererTypes.QuestPdf)
                _templateResolver.EnsureTemplateExists(context.FilePath, rendererType);

            var parameters = BuildProcedureParameters(context.Parameters);
            var dataTable = await _dataProvider.ExecuteStoredProcedureAsync(
                context.StoreProcedureName,
                parameters,
                cancellationToken);

            if (dataTable.Rows.Count == 0)
                return Fail("No data returned for this report.");

            var dataSet = ReportDataSetHelper.ToReportDataSet(dataTable);

            var renderer = _rendererRegistry.Resolve(rendererType);
            var renderResult = await renderer.RenderAsync(new ReportRenderRequest
            {
                Context = context,
                Data = dataSet,
                ResolvedTemplatePath = _templateResolver.Resolve(context.FilePath)
            }, cancellationToken);

            if (!renderResult.Success)
                return Fail(renderResult.Message);

            return new ReportRunResult
            {
                Success = true,
                Message = $"{context.ProgramName} generated ({rendererType}).",
                ReportPrintId = context.Print.ReportPrintId,
                ReportPrintIdMasked = context.Print.ReportPrintIdMasked,
                OutputContentType = renderResult.OutputContentType,
                OutputBytes = renderResult.OutputBytes,
                OutputFileName = renderResult.OutputFileName ?? BuildFileName(context),
                ReportExecutionGuid = Guid.NewGuid()
            };
        }
        catch (Exception ex)
        {
            return Fail($"Report generation failed: {ex.Message}");
        }
    }

    private static Dictionary<string, object?> BuildProcedureParameters(IReadOnlyDictionary<string, object?> parameters)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in parameters)
            result[key] = value;
        return result;
    }

    private static string BuildFileName(ReportRunContext context)
    {
        var key = string.IsNullOrWhiteSpace(context.DefinitionKey) ? "report" : context.DefinitionKey;
        return $"{key}.pdf";
    }

    private static ReportRunResult Fail(string message) =>
        new() { Success = false, Message = message };
}
