using System.Globalization;
using obdCodeFinder.Handlers.Services;

namespace obdCodeFinder.Demo;

/// <summary>
/// Finge di essere il dongle ELM327: riceve un comando ("010C")
/// e risponde con una riga grezza ("41 0C 25 80"), esattamente
/// come farebbe il chip vero. I valori "si muovono" leggermente
/// a ogni chiamata, per simulare un'auto accesa al minimo.
/// </summary>
public class SimulatedObdConnection : IOBDConnection
{
    // I PID che rispondono con 2 byte di valore (gli altri con 1).
    private static readonly HashSet<string> WidePids =
        new(StringComparer.OrdinalIgnoreCase) { "0C", "10" };

    // Stato "vivo" di ogni PID: il valore grezzo corrente.
    // (I commenti mostrano il valore "vero" dopo la conversione del parser.)
    private readonly Dictionary<string, int> _current = new()
    {
        ["04"] = 0x66,   // carico motore         -> ~40 %
        ["05"] = 0x82,   // temperatura liquido   -> 90 °C
        ["0B"] = 0x23,   // pressione collettore  -> 35 kPa
        ["0C"] = 0x2580, // giri motore           -> 2400 RPM  (0x2580 = 9600 ÷ 4)
        ["0D"] = 0x37,   // velocità              -> 55 km/h
        ["0F"] = 0x4B,   // temp. aria aspirazione -> 35 °C
        ["10"] = 0x05DC, // flusso aria MAF       -> 15 g/s    (0x05DC = 1500 ÷ 100)
        ["11"] = 0x26,   // posizione farfalla    -> ~15 %
        ["2F"] = 0xA6,   // livello carburante    -> ~65 %
        ["33"] = 0x65,   // pressione barometrica -> 101 kPa
        ["46"] = 0x3E,   // temperatura ambiente  -> 22 °C
        ["5C"] = 0x87,   // temperatura olio      -> 95 °C
    };

    private readonly Random _random = new();

    public string SendCommand(string command)
    {
        // Un comando ELM327 ha la forma "modalità" + "PID": "010C".
        // Noi conosciamo solo la modalità 01 (dati correnti).
        var pid = command.Length >= 4 ? command[2..4] : string.Empty;

        if (!_current.ContainsKey(pid))
            return "?"; // come il chip vero: comando sconosciuto

        Perturb(pid); // il valore "vive": oscilla a ogni lettura

        var word = _current[pid];
        var valueBytes = WidePids.Contains(pid)
            ? $"{word >> 8:X2} {word & 0xFF:X2}" // 2 byte: parte alta, parte bassa
            : $"{word & 0xFF:X2}";               // 1 byte

        // "41" = conferma della modalità 01: la risposta pulita del chip.
        return $"41 {pid} {valueBytes}";
    }

    /// <summary>Fa oscillare il valore grezzo di qualche unità, come un'auto al minimo.</summary>
    private void Perturb(string pid)
    {
        var isWide = WidePids.Contains(pid);
        var max = isWide ? 0xFFFF : 0xFF;
        var delta = isWide ? _random.Next(-16, 17) : _random.Next(-2, 3);
        _current[pid] = Math.Clamp(_current[pid] + delta, 0, max);
    }
}
