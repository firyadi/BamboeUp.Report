namespace BamboeUp.Report.Abstractions;

public interface IReportDefinitionResolver
{
    Task<ResolvedReportDefinition?> ResolveAsync(long programId, long? companyId, string reportKind);
}
