using System.Text.Json;
using obdCodeFinder.Core.Models;

namespace obdCodeFinder.Handlers.Services;

public class DtcCodeService : IDtcCodeService
{
    private readonly Dictionary<string, string> _genericCodes;
    private readonly Dictionary<string, Dictionary<string, string>> _brandCodes;

    public DtcCodeService()
    {
        _genericCodes = LoadDictionary("dtcCode.json");
        _brandCodes = LoadBrandDictionaries();
    }

    public DtcResult FindCode(string code, string? brand = null)
    {
        // 1. Prima cerchiamo nella marca scelta (descrizione più precisa)
        if (brand is not null
            && _brandCodes.TryGetValue(brand, out var brandDict)
            && brandDict.TryGetValue(code, out var brandDescription))
        {
            return new DtcResult { Code = code, Description = brandDescription, IsFound = true };
        }

        // 2. Ripiego: cerchiamo nei codici generici
        if (_genericCodes.TryGetValue(code, out var genericDescription))
        {
            return new DtcResult { Code = code, Description = genericDescription, IsFound = true };
        }

        // 3. Non trovato da nessuna parte
        return new DtcResult { Code = code, Description = null, IsFound = false };
    }

    private static Dictionary<string, string> LoadDictionary(string fileName)
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        if (!File.Exists(jsonPath))
        {
            return new Dictionary<string, string>();
        }

        var json = File.ReadAllText(jsonPath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
               ?? new Dictionary<string, string>();
    }

    private static Dictionary<string, Dictionary<string, string>> LoadBrandDictionaries()
    {
        var brands = new Dictionary<string, Dictionary<string, string>>();
        var brandsDir = Path.Combine(AppContext.BaseDirectory, "Data", "Brands");

        if (!Directory.Exists(brandsDir))
        {
            return brands;
        }

        foreach (var file in Directory.EnumerateFiles(brandsDir, "*.json"))
        {
            var brandName = Path.GetFileNameWithoutExtension(file);
            brands[brandName] = LoadDictionary(Path.Combine("Brands", Path.GetFileName(file)));
        }

        return brands;
    }
}
