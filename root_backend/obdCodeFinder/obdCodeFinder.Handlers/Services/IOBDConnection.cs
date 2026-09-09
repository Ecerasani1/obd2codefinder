namespace obdCodeFinder.Handlers.Services;

/// <summary>
/// Il confine tra il nostro codice e il "mondo esterno" (il dongle OBD).
/// Qualunque cosa sappia ricevere un comando ELM327 e rispondere con una
/// riga grezza implementa questa interfaccia: il dongle vero via seriale
/// e il simulatore per i test sono intercambiabili.
/// </summary>
public interface IOBDConnection
{
    /// <summary>
    /// Invia un comando ELM327 (es. "010C" = leggi il PID 0C) e restituisce
    /// la risposta grezza del dispositivo (es. "41 0C 25 80").
    /// </summary>
    string SendCommand(string command);
}
