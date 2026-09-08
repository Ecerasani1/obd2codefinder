namespace obdCodeFinder.Core.Models;

public class DtcResult
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsFound { get; set; }
}
