using System.Text.Json;

try
{
    if (args.Length != 1) throw new ArgumentException("Pass one JSON input file");
    using var data = JsonDocument.Parse(File.ReadAllText(args[0]));
    Console.WriteLine(JsonSerializer.Serialize(Analyze(data.RootElement)));
    return 0;
}
catch (Exception error) when (error is IOException or JsonException or FormatException
                               or ArgumentException or InvalidOperationException
                               or KeyNotFoundException or OverflowException)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}

static object Analyze(JsonElement data)
{
    var open=data.GetProperty("openHz").GetDouble();
    if(!double.IsFinite(open)||open<=0)throw new ArgumentException("Invalid open frequency");
    var rows=new List<(int fret,double target,double measured,double cents)>();
    foreach(var row in data.GetProperty("frets").EnumerateArray())
    {
        if(row.GetArrayLength()!=2)throw new ArgumentException("Expected fret and frequency");
        var fret=row[0].GetInt32();
        var measured=row[1].GetDouble();
        if(fret<0||fret>48||!double.IsFinite(measured)||measured<=0)
            throw new ArgumentException("Invalid fret sample");
        var target=open*Math.Pow(2,fret/12.0);
        if(!double.IsFinite(target))throw new ArgumentException("Target frequency too large");
        var cents=1200*Math.Log2(measured/target);
        if(!double.IsFinite(cents))throw new ArgumentException("Frequencies exceed supported ratio");
        rows.Add((fret,target,measured,cents));
    }
    return new {withinTolerance=rows.Count(row=>Math.Abs(row.cents)<=5),
        meanAbsoluteCents=rows.Count==0 ? (double?)null : rows.Average(row=>Math.Abs(row.cents)),
        frets=rows.Select(row=>new{row.fret,row.target,row.measured,row.cents})};
}
