using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using BamboeUp.Report.Services;

namespace BamboeUp.Report.Handlers;

public sealed class DocReportHandler : IReportHandler
{
    private readonly IReportDataProvider _dataProvider;
    private readonly ReportRendererRegistry _rendererRegistry;
    private readonly ReportTemplatePathResolver _templateResolver;

    public DocReportHandler(
        IReportDataProvider dataProvider,
        ReportRendererRegistry rendererRegistry,
        ReportTemplateOptions templateOptions)
    {
        _dataProvider = dataProvider;
        _rendererRegistry = rendererRegistry;
        _templateResolver = new ReportTemplatePathResolver(templateOptions);
    }

    public string ReportKind => "DOC";

    public async Task<ReportRunResult> RunAsync(ReportRunContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.StoreProcedureName))
        {
            return new ReportRunResult
            {
                Success = false,
                Message = "Report definition is missing stored procedure name."
            };
        }

        if (!ReportRendererTypes.TryResolve(context.RendererType, context.FilePath, context.ReportKind, out var rendererType))
            rendererType = ReportRendererTypes.QuestPdf;

        try
        {
            if (rendererType != ReportRendererTypes.QuestPdf)
                _templateResolver.EnsureTemplateExists(context.FilePath, rendererType);

            var parameters = BuildProcedureParameters(context);
            var data = await _dataProvider.ExecuteStoredProcedureAsync(
                context.StoreProcedureName,
                parameters,
                cancellationToken);

            if (data.Rows.Count == 0)
            {
                return new ReportRunResult
                {
                    Success = false,
                    Message = "No data found for this print slip."
                };
            }

            var dataSet = ReportDataSetHelper.ToReportDataSet(data);

            var renderer = _rendererRegistry.Resolve(rendererType);
            var renderResult = await renderer.RenderAsync(new ReportRenderRequest
            {
                Context = context,
                Data = dataSet,
                ResolvedTemplatePath = _templateResolver.Resolve(context.FilePath)
            }, cancellationToken);

            if (!renderResult.Success)
            {
                return new ReportRunResult
                {
                    Success = false,
                    Message = renderResult.Message
                };
            }

            return new ReportRunResult
            {
                Success = true,
                Message = $"{context.ProgramName} generated.",
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
            return new ReportRunResult
            {
                Success = false,
                Message = $"Document print failed: {ex.Message}"
            };
        }
    }

    private static Dictionary<string, object?> BuildProcedureParameters(ReportRunContext context)
    {
        var entityGuid = ResolveEntityGuid(context.Parameters);
        if (entityGuid.HasValue)
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["EntityGuid"] = entityGuid.Value
            };

        var entityId = ResolveEntityId(context.Parameters);
        if (entityId.HasValue)
            return new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["EntityId"] = entityId.Value
            };

        throw new InvalidOperationException("Entity key is required for document print (EntityGuid or EntityId).");
    }

    private static Guid? ResolveEntityGuid(IReadOnlyDictionary<string, object?> parameters)
    {
        if (TryGetGuid(parameters, "EntityGuid", out var entityGuid))
            return entityGuid;

        foreach (var key in parameters.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsEntityGuidKey(key))
                continue;
            if (TryGetGuid(parameters, key, out var guid))
                return guid;
        }

        return null;
    }

    private static long? ResolveEntityId(IReadOnlyDictionary<string, object?> parameters)
    {
        if (TryGetLong(parameters, "EntityId", out var entityId))
            return entityId;

        foreach (var key in parameters.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsEntityIdKey(key))
                continue;
            if (TryGetLong(parameters, key, out var id))
                return id;
        }

        return null;
    }

    private static bool IsEntityGuidKey(string key)
    {
        if (!key.EndsWith("Guid", StringComparison.OrdinalIgnoreCase))
            return false;

        return !key.Equals("CompanyGuid", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("CompanyOfficeGuid", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("ProgramGuid", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("UserGuid", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsEntityIdKey(string key)
    {
        if (!key.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            return false;

        return !key.Equals("CompanyId", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("CompanyOfficeId", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("ProgramId", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("UserId", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("CreatedById", StringComparison.OrdinalIgnoreCase)
               && !key.Equals("UpdatedById", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetGuid(IReadOnlyDictionary<string, object?> parameters, string key, out Guid guid)
    {
        guid = Guid.Empty;
        if (!parameters.TryGetValue(key, out var raw) || raw is null)
            return false;

        return raw switch
        {
            Guid g => (guid = g) != Guid.Empty,
            string text when Guid.TryParse(text, out guid) => guid != Guid.Empty,
            _ => false
        };
    }

    private static bool TryGetLong(IReadOnlyDictionary<string, object?> parameters, string key, out long id)
    {
        id = 0;
        if (!parameters.TryGetValue(key, out var raw) || raw is null)
            return false;

        return raw switch
        {
            long l => (id = l) != 0,
            int i => (id = i) != 0,
            string text when long.TryParse(text, out id) => id != 0,
            _ => false
        };
    }

    private static string BuildFileName(ReportRunContext context)
    {
        var key = string.IsNullOrWhiteSpace(context.DefinitionKey)
            ? "document"
            : context.DefinitionKey;
        return $"{key}.pdf";
    }
}
