namespace obdCodeFinder.Handlers.Services;

/// <summary>
/// Voce del catalogo pids.json: descrive un PID (come si chiama, in che
/// unità è, in quale sezione mostrarlo) e dice al parser QUALE ricetta
/// di conversione usare. La formula vera e propria vive nel codice,
/// non qui: il JSON non può sbagliare la matematica.
/// </summary>
public class PidDefinition
{
    public string Pid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Conversion { get; set; } = string.Empty;
}
