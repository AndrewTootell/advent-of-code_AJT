namespace Advent_of_code.Day5;

public class Day5
{
    [Test]
    public void Test_1()
    {
        var data = ReadInput("Day5/Data.txt");
        var dataSetsStr = data.Split("\n\n").ToList();
        List<Map> maps = new();
        dataSetsStr.ForEach(d =>
        {
            if (d.Contains("seeds: "))
            {
                return;
            }
            maps.Add(new Map(d));
        });
        var seeds = dataSetsStr.First().Split(':')[1].Split(' ').Where(s=>s!="").ToList().ConvertAll(long.Parse);
        List<long> locations = new();
        seeds.ForEach(seed =>
        {
            var from = "seed";
            long value = seed;
            while (from != "location")
            {
                var map = maps.Single(m => m.From.Equals(from));
                //Console.WriteLine($"{from} {value} => {map.To} {map.MapValue(value)}");
                value = map.MapValue(value);
                from = map.To;
            }
            locations.Add(value);
        });
        Console.WriteLine(locations.Min());
    }

    private class Map
    {
        public readonly string From;
        public readonly string To;

        private readonly Dictionary<(long, long), long> _mapping;

        public Map(string set)
        {
            var lines = set.Split('\n').ToList();
            var splitName = lines.First().Split(' ')[0].Split('-');
            From = splitName[0];
            To = splitName[2];
            _mapping = new();
            Console.WriteLine($"From:{From} To:{To}");
            List<long> lows = new();
            for (var i = 1; i < lines.Count; i++ )
            {
                var lineStrs = lines[i].Split(' ').ToList();
                var lineValues = lineStrs.ConvertAll(long.Parse);
                var sourceStart = lineValues[1];
                var sinkStart = lineValues[0];
                var range = lineValues[2];
                var diff = sinkStart - sourceStart;
                _mapping.Add((sourceStart, sourceStart + range), diff);
                Console.WriteLine($"if between {sourceStart}-{sourceStart + range} => {diff}");
                lows.Add(sourceStart+range);
            }
            Console.WriteLine();
            lows.ForEach(l=>Console.Write($"{l}, "));
            Console.WriteLine(lows.Min());
            Console.WriteLine();
        }

        public long MapValue(long from)
        {
            var map = _mapping.SingleOrDefault(k => k.Key.Item1 <= from && from < k.Key.Item2);
            return from + map.Value;
        }
    }

    private string ReadInput(string filePathEnd)
    {
        var line = "";

        using var sr = new StreamReader(
            "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/" + filePathEnd);
        while (!sr.EndOfStream)
        {
            line = sr.ReadToEnd();
        }

        return line;
    }
    
}