using System.Globalization;
using System.Text.Json;
using obdCodeFinder.Core.Models;

namespace obdCodeFinder.Handlers.Services;

public class ObdParserService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Dictionary<string, PidDefinition> _catalog;

    public ObdParserService()
    {
        _catalog = LoadCatalog();
    }

    public LiveParameter? Parse(string response)
    {
        var tokens = response.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length < 3)
            return null;


        var pid = tokens[1];
        if (!_catalog.TryGetValue(pid, out var definition))
            return null;

        var payload = tokens[2..];
        if (!TryApplyConversion(definition.Conversion, payload, out var value))
            return null;

        return new LiveParameter
        {
            Pid = pid,
            Name = definition.Name,
            Unit = definition.Unit,
            Category = definition.Category,
            Value = value,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    private static bool TryApplyConversion(string conversion, string[] payload, out double value)
    {
        value = 0;

        switch (conversion)
        {
            case "direct":
                {
                    if (!TryParseByte(payload, 0, out var a))
                        return false;
                    value = a;
                    return true;
                }
            case "percent":
                {
                    if (!TryParseByte(payload, 0, out var a))
                        return false;
                    value = a * 100.0 / 255.0;
                    return true;
                }
            case "minus40":
                {
                    if (!TryParseByte(payload, 0, out var a))
                        return false;
                    value = a - 40;
                    return true;
                }
            case "rpm":
                {
                    if (!TryParseByte(payload, 0, out var a))
                        return false;
                    if (!TryParseByte(payload, 1, out var b))
                        return false;
                    value = (a * 256 + b) / 4.0;
                    return true;
                }
            case "maf":
                {
                    if (!TryParseByte(payload, 0, out var a))
                        return false;
                    if (!TryParseByte(payload, 1, out var b))
                        return false;
                    value = (a * 256 + b) / 100.0;
                    return true;
                }
            default:

                return false;
        }
    }

    private static bool TryParseByte(string[] payload, int index, out int b)
    {
        b = 0;
        if (index >= payload.Length)
            return false;
        return int.TryParse(payload[index], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b);
    }

    private static Dictionary<string, PidDefinition> LoadCatalog()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "pids.json");
        if (!File.Exists(jsonPath))
            return new Dictionary<string, PidDefinition>();

        var json = File.ReadAllText(jsonPath);
        var list = JsonSerializer.Deserialize<List<PidDefinition>>(json, JsonOptions);
        return list?.ToDictionary(p => p.Pid, StringComparer.OrdinalIgnoreCase)
               ?? new Dictionary<string, PidDefinition>();
    }
}
