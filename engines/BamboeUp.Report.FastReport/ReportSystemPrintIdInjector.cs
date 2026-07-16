using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;
using FastReport;
using FrReport = FastReport.Report;

namespace BamboeUp.Report.FastReport;

/// <summary>Injects or overwrites Print ID on every page footer — platform controlled, not from SP.</summary>
internal static class ReportSystemPrintIdInjector
{
    public static void Apply(FrReport report, ReportRunContext context)
    {
        if (!context.Print.RequiresPrintId)
            return;

        var display = context.Print.ReportPrintIdMasked
                      ?? context.Print.ReportPrintId
                      ?? string.Empty;

        if (string.IsNullOrWhiteSpace(display))
            return;

        var text = $"Print ID: {display}";
        var existing = report.FindObject(ReportSystemFields.PrintIdObjectName) as TextObject;
        if (existing != null)
        {
            existing.Text = text;
            existing.Visible = true;
            return;
        }

        foreach (PageBase page in report.Pages)
        {
            if (page is not ReportPage reportPage)
                continue;

            var footer = reportPage.PageFooter;
            if (footer == null)
                continue;

            var width = Math.Max(200f, footer.Width - 18.9f);
            footer.Objects.Add(new TextObject
            {
                Name = ReportSystemFields.PrintIdObjectName,
                Left = 9.45f,
                Top = 4.45f,
                Width = width,
                Height = 18.9f,
                HorzAlign = HorzAlign.Right,
                Text = text,
                Font = new System.Drawing.Font("Arial", 9f),
                TextFill = new SolidFill(System.Drawing.Color.Gray)
            });
            break;
        }
    }
}
