namespace BamboeUp.Report.Abstractions;

/// <summary>Renderer engine identifiers stored in [core].[ReportDefinition].RendererType.</summary>
public static class ReportRendererTypes
{
    public const string FastReport = "FastReport";
    public const string Telerik = "Telerik";
    public const string QuestPdf = "QuestPDF";
    public const string DevExpress = "DevExpress";
    public const string TelerikPivot = "TelerikPivot";
    public const string DevExpressPivot = "DevExpressPivot";

    private static readonly HashSet<string> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        FastReport, Telerik, QuestPdf, DevExpress, TelerikPivot, DevExpressPivot
    };

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var trimmed = value.Trim();
        foreach (var known in Known)
        {
            if (known.Equals(trimmed, StringComparison.OrdinalIgnoreCase))
                return known;
        }

        return trimmed;
    }

    public static bool TryResolve(
        string? rendererType,
        string? filePath,
        string reportKind,
        out string resolved)
    {
        if (!string.IsNullOrWhiteSpace(rendererType))
        {
            resolved = Normalize(rendererType);
            return true;
        }

        var ext = Path.GetExtension(filePath ?? string.Empty).ToLowerInvariant();
        resolved = ext switch
        {
            ".frx" or ".fr3" => FastReport,
            ".trdp" or ".trdx" => Telerik,
            ".repx" => DevExpress,
            _ when string.Equals(reportKind, "DOC", StringComparison.OrdinalIgnoreCase) => QuestPdf,
            _ => string.Empty
        };

        return !string.IsNullOrWhiteSpace(resolved);
    }

    public static string? GetExpectedExtension(string rendererType)
    {
        return Normalize(rendererType) switch
        {
            FastReport => ".frx",
            Telerik or TelerikPivot => ".trdp",
            DevExpress or DevExpressPivot => ".repx",
            QuestPdf => null,
            _ => null
        };
    }
}
