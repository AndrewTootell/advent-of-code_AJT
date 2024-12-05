namespace Advent_of_code.Day5;

public class Day5
{
	[Fact]
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
		//test: 35
		//actual: 486613012
	}
	
	[Fact]
    public void Test_2()
    {
	    var data = ReadInput("Day5/Data.txt");
        var dataSetsStr = data.Split("\n\n").ToList();
        List<Map> maps = new();
        List<(long,long)> seedRanges = new();
        dataSetsStr.ForEach(d =>
        {
            if (d.Contains("seeds: "))
            {
                var seedData = d.Split(": ")[1].Split(' ').ToList().ConvertAll(long.Parse);
                for (var i = 0; i < seedData.Count; i += 2)
                {
                    seedRanges.Add((seedData[i],seedData[i]+seedData[i+1]-1));
                }
                return;
            }
            maps.Add(new Map(d));
        });
		var possibleRanges = seedRanges.OrderBy(s=>s.Item1).ToList();
		// soil, fertilizer, ...
        maps.ForEach(m =>
        {
	        possibleRanges = RangeCrossOver(possibleRanges, m.Ranges.OrderBy(n=> n.Start).ToList());
        });
        Console.WriteLine(possibleRanges.MinBy(i=>i.Item1).Item1.ToString());
        // Test: 46
        // Actual: 56931769
    }

    private List<(long, long)> RangeCrossOver(List<(long,long)> input, List<Range> mapped)
    {
	    List<(long, long)> result = new();
	    input.ForEach(i =>
	    {
		    // map ranges that affect this input
		    var relevantMapped = mapped.Where(m=> m.End > i.Item1 && m.Start < i.Item2).ToList();

		    if (relevantMapped.Count == 0)
		    {
			    result.Add((i.Item1,i.Item2));
			    return;
		    }

		    // [1,2,3,{4,...
		    if (relevantMapped.First().Start <= i.Item1)
		    {
			    var start = i.Item1 + relevantMapped.First().Delta;
			    if (relevantMapped.First().End >= i.Item2)
			    {
				    // all of input is within the range
				    result.Add((start, i.Item2 + relevantMapped.First().Delta));
				    return;
			    }
			    // mapped
			    result.Add((start, relevantMapped.First().End+relevantMapped.First().Delta));
		    }
		    // {1,2,3,4,[5,...
		    else
		    {
			    // start of range must be lower than end of range else relevant mapped would be empty
			    // not mapped
			    result.Add((i.Item1, relevantMapped.First().Start-1));
		    }

		    // ...1,}2,3,4,]
		    if (relevantMapped.Last().End >= i.Item2)
		    {
			    //mapped
			    result.Add((relevantMapped.Last().Start+relevantMapped.Last().Delta, i.Item2 + relevantMapped.Last().Delta));
		    }
		    // {1,2,3,4,[5,...
		    else
		    {
			    // not mapped
			    result.Add((relevantMapped.Last().End+1, i.Item2));
		    }
		    // internal not mapped
		    for (var i1 = 0; i1 < relevantMapped.Count-1; i1++)
		    {
			    // not mapped
			    result.Add((relevantMapped[i1].End+1,relevantMapped[i1+1].Start-1));
		    }
		    var internalMapped = mapped.Where(m=> m.End < i.Item2 && m.Start > i.Item1).ToList();
		    internalMapped.ForEach(m =>
			    {
				    // mapped
				    result.Add((m.Start+m.Delta,m.End+m.Delta));
			    });
	    });
	    return result;
    }

    private class Map
    {
        public readonly string From;
        public readonly string To;

        public readonly List<Range> Ranges;

        public Map(string set)
        {
            var lines = set.Split('\n').ToList();
            var splitName = lines.First().Split(' ')[0].Split('-');
            From = splitName[0];
            To = splitName[2];
            Ranges = new();
            for (var i = 1; i < lines.Count; i++ )
            {
                var lineStrings = lines[i].Split(' ').ToList();
                var lineValues = lineStrings.ConvertAll(long.Parse);
                var sourceStart = lineValues[1];
                var sinkStart = lineValues[0];
                var range = lineValues[2];
                var diff = sinkStart - sourceStart;
                Ranges.Add(new Range
                {
                    Start = sourceStart,
                    End = sourceStart + range,
                    Delta = diff
                });
            }
        }

        public long MapValue(long from)
        {
            var map = Ranges.SingleOrDefault(r => r.Start <= from && from < r.End);
            if (map == null)
            {
                return from;
            }

            return from + map.Delta;
        }
    }


    private class Range
    {
        public long Start;
        public long End;
        public long Delta;
    }

    private static string ReadInput(string filePathEnd)
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
