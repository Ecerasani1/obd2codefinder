using obdCodeFinder.Core.Models;

namespace obdCodeFinder.Handlers.Services;

public interface IDtcCodeService
{
    DtcResult FindCode(string code, string? brand = null);
}
