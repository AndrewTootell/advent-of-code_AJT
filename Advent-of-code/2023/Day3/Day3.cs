namespace Advent_of_code.Day3;

public class Day3
{
    private class Jerry
    {
        public bool IsSymbol;
        public bool IsNumber;
        public int? Index;
        public int? IndexStart;
        public int? IndexEnd;
        public int? Value;
    }
    
    [Fact]
    public void Task_1()
    {
        var lines = new List<string>();
        using (var sr = new StreamReader(
                   "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day3/Data.txt"))
        {
            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }
        }

        var data = new List<List<Jerry>>();
        foreach (var line in lines)
        {
            var lineData = new List<Jerry>();
            string numberVal = "";
            int? indexStart = null;
            
            for (var i=0;i<line.Length;i++)
            {
                var s = line[i].ToString();
                // If number
                if (int.TryParse(s, out _))
                {
                    numberVal += s;
                    indexStart ??= i;
                    continue;
                }

                // if end of number
                if (numberVal != "")
                {
                    lineData.Add( new Jerry
                        {
                            IsNumber = true,
                            IndexStart = indexStart,
                            IndexEnd = i-1,
                            Value = int.Parse(numberVal)
                        }
                    );
                    indexStart = null;
                    numberVal = "";
                }
                    
                if (s == ".")
                {
                    continue;
                }
                
                // Handle symbol
                lineData.Add( new Jerry
                    {
                        IsSymbol = true,
                        Index = i
                    }
                );
            }
            // if end of number
            if (numberVal != "")
            {
                lineData.Add( new Jerry
                    {
                        IsNumber = true,
                        IndexStart = indexStart,
                        IndexEnd = line.Length,
                        Value = int.Parse(numberVal)
                    }
                );
            }
            data.Add(lineData);
        }

        var sumTotal = 0;
        
        for (var i = 0;i<data.Count;i++)
        {
            var lineData = data[i];
            var lineBefore = i == 0? new List<Jerry>():data[i-1];
            var lineAfter = i == data.Count-1? new List<Jerry>():data[i+1];
            var numbers = lineData.Where(ld => ld.IsNumber);
            foreach (var n in numbers)
            {
                // line before
                var before = lineBefore.Any(s => s.IsSymbol && (n.IndexStart - 1 <= s.Index && s.Index <= n.IndexEnd + 1));
                
                // same line before or after
                var same = lineData.Any(s => s.IsSymbol && (s.Index == n.IndexStart - 1 || s.Index == n.IndexEnd + 1));

                // line after
                var after = lineAfter.Any(s => s.IsSymbol && (n.IndexStart - 1 <= s.Index && s.Index <= n.IndexEnd + 1));

                if (before || same || after)
                {
                    sumTotal += n.Value!.Value;
                }
            }
        }
        Console.WriteLine(sumTotal);
    }
    
    [Fact]
    public void Task_2()
    {
        var lines = new List<string>();
        using (var sr = new StreamReader(
                   "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day3/Data.txt"))
        {
            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }
        }

        var data = new List<List<Jerry>>();
        foreach (var line in lines)
        {
            var lineData = new List<Jerry>();
            string numberVal = "";
            int? indexStart = null;
            
            for (var i=0;i<line.Length;i++)
            {
                var s = line[i].ToString();
                // If number
                if (int.TryParse(s, out _))
                {
                    numberVal += s;
                    indexStart ??= i;
                    continue;
                }

                // if end of number
                if (numberVal != "")
                {
                    lineData.Add( new Jerry
                        {
                            IsNumber = true,
                            IndexStart = indexStart,
                            IndexEnd = i-1,
                            Value = int.Parse(numberVal)
                        }
                    );
                    indexStart = null;
                    numberVal = "";
                }
                    
                if (s == "*")
                {
                    // Handle symbol
                    lineData.Add( new Jerry
                        {
                            IsSymbol = true,
                            Index = i
                        }
                    );
                }
            }
            
            // if end of number
            if (numberVal != "")
            {
                lineData.Add( new Jerry
                    {
                        IsNumber = true,
                        IndexStart = indexStart,
                        IndexEnd = line.Length,
                        Value = int.Parse(numberVal)
                    }
                );
            }
            data.Add(lineData);
        }

        var sumTotal = 0;
        
        // for each line
        for (var i = 0;i<data.Count;i++)
        {
            var lineData = data[i];
            var lineBefore = i == 0? new List<Jerry>():data[i-1];
            var lineAfter = i == data.Count-1? new List<Jerry>():data[i+1];
            
            var symbols = lineData.Where(ld => ld.IsSymbol);
            foreach (var star in symbols)
            {
                var gearNumbers = lineBefore.Where(n => n.IsNumber && n.IndexStart <= star.Index + 1 && n.IndexEnd >= star.Index - 1 ).ToList();
                gearNumbers = gearNumbers.Concat(lineData.Where(n => n.IsNumber && (n.IndexStart == star.Index + 1 || n.IndexEnd == star.Index - 1))).ToList();
                gearNumbers = gearNumbers.Concat(lineAfter.Where(n => n.IsNumber && n.IndexStart <= star.Index + 1 && n.IndexEnd >= star.Index - 1)).ToList();
                if (gearNumbers.Count == 2)
                {
                    sumTotal += gearNumbers[0].Value!.Value * gearNumbers[1].Value!.Value;
                }
            }
        }
        Console.WriteLine(sumTotal);
    }
}