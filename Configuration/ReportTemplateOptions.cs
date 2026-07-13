namespace BamboeUp.Report.Configuration;

public sealed class ReportTemplateOptions
{
    /// <summary>Root folder for template files (Rpt/, Doc/, Pvt/ relative paths from DB).</summary>
    public string TemplateRoot { get; set; } = Path.Combine(AppContext.BaseDirectory, "Reports");
}
