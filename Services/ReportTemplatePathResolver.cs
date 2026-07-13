using BamboeUp.Report.Configuration;

namespace BamboeUp.Report.Services;

public sealed class ReportTemplatePathResolver
{
    private readonly ReportTemplateOptions _options;

    public ReportTemplatePathResolver(ReportTemplateOptions options)
    {
        _options = options;
    }

    public string? Resolve(string? relativeFilePath)
    {
        if (string.IsNullOrWhiteSpace(relativeFilePath))
            return null;

        var normalized = relativeFilePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);

        var fullPath = Path.GetFullPath(Path.Combine(_options.TemplateRoot, normalized));
        var rootFull = Path.GetFullPath(_options.TemplateRoot);

        if (!fullPath.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Report template path escapes the configured template root.");

        return fullPath;
    }

    public void EnsureTemplateExists(string? relativeFilePath, string rendererType)
    {
        if (string.IsNullOrWhiteSpace(relativeFilePath))
            return;

        var fullPath = Resolve(relativeFilePath);
        if (fullPath != null && File.Exists(fullPath))
            return;

        throw new FileNotFoundException(
            $"Report template not found for renderer '{rendererType}': {relativeFilePath} " +
            $"(resolved: {fullPath ?? "(null)"}, root: {_options.TemplateRoot})");
    }
}
