namespace obdCodeFinder.Core.Models;

public class LiveParameter
{
    public string Pid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}
