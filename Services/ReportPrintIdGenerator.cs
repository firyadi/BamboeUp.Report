using System.Security.Cryptography;
using System.Text;

namespace BamboeUp.Report.Services;

public static class ReportPrintIdGenerator
{
    public static (string Full, string Masked) Generate(string prefix, string? period = null)
    {
        prefix = string.IsNullOrWhiteSpace(prefix) ? "RP" : prefix.Trim().ToUpperInvariant();
        period ??= DateTime.UtcNow.ToString("yyyyMM");
        var seq = Convert.ToHexString(RandomNumberGenerator.GetBytes(3));
        var checksum = ComputeChecksum($"{prefix}{period}{seq}");
        var full = $"{prefix}-{period}-{seq}-{checksum}";
        var masked = $"{prefix}-{period}-****-{checksum}";
        return (full, masked);
    }

    private static string ComputeChecksum(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash)[..4];
    }
}
