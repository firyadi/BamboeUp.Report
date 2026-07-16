using System.Data;
using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Configuration;

namespace BamboeUp.Report.Services;

public static class ReportHeaderDataBuilder
{
    public static DataTable? TryCreate(ReportRunContext context)
    {
        var layout = ReportLayoutConfig.Parse(context.LayoutJson);
        var needsHeader = layout.UsesHeaderData;
        var needsPrintId = context.Print.RequiresPrintId;

        if (!needsHeader && !needsPrintId)
            return null;

        var table = new DataTable(ReportSystemFields.HeaderDataTable);
        table.Columns.Add("ShowCompanyLogo", typeof(bool));
        table.Columns.Add("ShowCompanyName", typeof(bool));
        table.Columns.Add("ShowOfficeName", typeof(bool));
        table.Columns.Add("ShowPrintedAt", typeof(bool));
        table.Columns.Add(ReportSystemFields.ShowPrintIdColumn, typeof(bool));
        table.Columns.Add("CompanyName", typeof(string));
        table.Columns.Add("OfficeName", typeof(string));
        table.Columns.Add("PrintedAt", typeof(string));
        table.Columns.Add(ReportSystemFields.PrintIdColumn, typeof(string));
        table.Columns.Add(ReportSystemFields.PrintIdMaskedColumn, typeof(string));
        table.Columns.Add("CompanyLogo", typeof(byte[]));

        var hasLogo = context.CompanyLogo is { Length: > 0 };
        var showLogo = needsHeader && layout.Header.ShowCompanyLogo && hasLogo;
        var showName = needsHeader && layout.Header.ShowCompanyName && !string.IsNullOrWhiteSpace(context.CompanyName);
        var showOffice = needsHeader && layout.Header.ShowOfficeName && !string.IsNullOrWhiteSpace(context.CompanyOfficeName);
        var printedAt = needsHeader && layout.Header.ShowPrintedAt
            ? DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            : string.Empty;
        var showPrintedAt = needsHeader && layout.Header.ShowPrintedAt;

        var printId = context.Print.ReportPrintId ?? string.Empty;
        var printIdMasked = context.Print.ReportPrintIdMasked ?? printId;

        table.Rows.Add(
            showLogo,
            showName,
            showOffice,
            showPrintedAt,
            needsPrintId,
            context.CompanyName ?? string.Empty,
            context.CompanyOfficeName ?? string.Empty,
            printedAt,
            printId,
            printIdMasked,
            showLogo ? context.CompanyLogo : null);

        return table;
    }
}
