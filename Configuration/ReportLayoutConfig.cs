using System.Text.Json;
using System.Text.Json.Serialization;

namespace BamboeUp.Report.Configuration;

public sealed class ReportLayoutConfig
{
    public string Paper { get; set; } = "A4";
    public string Orientation { get; set; } = "Portrait";
    public string MarginPreset { get; set; } = "Normal";
    public ReportHeaderConfig Header { get; set; } = new();

    public bool UsesHeaderData =>
        !string.Equals(Header.Profile, ReportHeaderProfiles.Minimal, StringComparison.OrdinalIgnoreCase);

    public static ReportLayoutConfig Parse(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new ReportLayoutConfig();

        try
        {
            return JsonSerializer.Deserialize<ReportLayoutConfig>(json, JsonOptions)
                   ?? new ReportLayoutConfig();
        }
        catch (JsonException)
        {
            return new ReportLayoutConfig();
        }
    }

    public string ToJson() =>
        JsonSerializer.Serialize(this, JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public sealed class ReportHeaderConfig
{
    public string Profile { get; set; } = ReportHeaderProfiles.Minimal;
    public bool ShowCompanyLogo { get; set; }
    public bool ShowCompanyName { get; set; }
    public bool ShowOfficeName { get; set; }
    public bool ShowPrintedAt { get; set; }
}

public static class ReportHeaderProfiles
{
    public const string Minimal = "Minimal";
    public const string Standard = "Standard";
    public const string Letterhead = "Letterhead";
}
