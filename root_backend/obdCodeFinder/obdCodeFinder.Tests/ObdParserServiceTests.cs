using obdCodeFinder.Handlers.Services;

namespace obdCodeFinder.Tests;

/// <summary>
/// Blinda la matematica del parser: per ogni ricetta di conversione
/// (direct, percent, minus40, rpm, maf) verifica che una risposta grezza
/// nota produca esattamente il valore atteso. Se un giorno una formula
/// verrà toccata e rotta, questi test lo urlano.
/// </summary>
public class ObdParserServiceTests
{
    private readonly ObdParserService _parser = new();

    [Fact]
    public void Direct_velocity_returns_byte_value()
    {
        // PID 0D (Velocità): il byte è già il valore. 0x37 = 55 km/h
        var result = _parser.Parse("41 0D 37");

        Assert.NotNull(result);
        Assert.Equal(55, result!.Value, 2);
    }

    [Fact]
    public void Percent_engine_load_returns_percentage()
    {
        // PID 04 (Carico motore): A × 100 ÷ 255. 0x66 = 102 → 40%
        var result = _parser.Parse("41 04 66");

        Assert.NotNull(result);
        Assert.Equal(40.0, result!.Value, 2);
    }

    [Fact]
    public void Minus40_coolant_temperature_offsets_by_minus_40()
    {
        // PID 05 (Temperatura liquido): A − 40. 0x82 = 130 → 90 °C
        var result = _parser.Parse("41 05 82");

        Assert.NotNull(result);
        Assert.Equal(90.0, result!.Value, 2);
    }

    [Fact]
    public void Rpm_engine_speed_combines_two_bytes_and_divides_by_four()
    {
        // PID 0C (Giri motore): (A×256+B)÷4. 0x2580 = 9600 → 2400 RPM
        var result = _parser.Parse("41 0C 25 80");

        Assert.NotNull(result);
        Assert.Equal(2400.0, result!.Value, 2);
    }

    [Fact]
    public void Maf_air_flow_combines_two_bytes_and_divides_by_hundred()
    {
        // PID 10 (Flusso aria MAF): (A×256+B)÷100. 0x05DC = 1500 → 15 g/s
        var result = _parser.Parse("41 10 05 DC");

        Assert.NotNull(result);
        Assert.Equal(15.0, result!.Value, 2);
    }

    [Fact]
    public void Parse_carries_metadata_from_the_catalog()
    {
        // La voce del catalogo (nome, unità, categoria) deve arrivare intatta.
        var result = _parser.Parse("41 46 32"); // 0x32 = 50 → 10 °C ambiente

        Assert.NotNull(result);
        Assert.Equal("Temperatura ambiente", result!.Name);
        Assert.Equal("°C", result.Unit);
        Assert.Equal("Temperature", result.Category);
    }

    [Fact]
    public void Parse_returns_null_for_unknown_pid()
    {
        // Un PID che non è nel catalogo non va inventato: meglio null.
        var result = _parser.Parse("41 FF 00");

        Assert.Null(result);
    }

    [Fact]
    public void Parse_returns_null_when_response_is_too_short()
    {
        // Senza almeno un byte di valore non c'è nulla da convertire.
        var result = _parser.Parse("41 0C");

        Assert.Null(result);
    }
}

