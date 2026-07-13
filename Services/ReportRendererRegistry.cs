using BamboeUp.Report.Abstractions;

namespace BamboeUp.Report.Services;

public sealed class ReportRendererRegistry
{
    private readonly Dictionary<string, IReportRenderer> _renderers =
        new(StringComparer.OrdinalIgnoreCase);

    public void Register(IReportRenderer renderer)
    {
        var key = ReportRendererTypes.Normalize(renderer.RendererType);
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Renderer type is required.", nameof(renderer));

        _renderers[key] = renderer;
    }

    public IReportRenderer Resolve(string rendererType)
    {
        var key = ReportRendererTypes.Normalize(rendererType);
        if (!_renderers.TryGetValue(key, out var renderer))
            throw new InvalidOperationException(
                $"Report renderer '{rendererType}' is not registered. " +
                $"Available: {string.Join(", ", _renderers.Keys)}");

        return renderer;
    }

    public IReadOnlyCollection<string> RegisteredRendererTypes => _renderers.Keys.ToList();
}
