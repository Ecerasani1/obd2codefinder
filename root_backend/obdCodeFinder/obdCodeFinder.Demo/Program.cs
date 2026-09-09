using obdCodeFinder.Demo;
using obdCodeFinder.Handlers.Services;

// Il "dongle finto" e il parser VERO: la stessa coppia che useremo
// con l'adattatore ELM327 quando arriverà. Il simulatore risponde
// con righe grezze; il parser vero le converte in valori puliti.
IOBDConnection connection = new SimulatedObdConnection();
var parser = new ObdParserService();

// I PID che vogliamo vedere sul "cruscotto".
string[] pids = ["04", "05", "0B", "0C", "0D", "0F", "10", "11", "2F", "33", "46", "5C"];

// Se passi un numero come argomento si ferma da solo (utile per provare);
// senza argomenti va all'infinito (fermalo con Ctrl+C).
var maxTicks = args.Length > 0 && int.TryParse(args[0], out var n) ? n : -1;
var tick = 0;

while (maxTicks < 0 || tick < maxTicks)
{
    var readings = new List<string>();

    foreach (var pid in pids)
    {
        // 1. chiedo al "dongle" il PID (modalità 01 = dati correnti)
        var response = connection.SendCommand("01" + pid);

        // 2. il parser VERO converte la riga grezza in un valore pulito
        var parameter = parser.Parse(response);

        if (parameter is not null)
            readings.Add($"{parameter.Name}: {parameter.Value:0.#} {parameter.Unit}");
    }

    Console.WriteLine($"[{tick + 1}]  " + string.Join("   |   ", readings));
    await Task.Delay(500);
    tick++;
}
