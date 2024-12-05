namespace Advent_of_code.Day1;

public class Day1
{
    [Fact]
    public void Test_1()
    {
        var sumTotal = 0;
        using (var sr = new StreamReader("/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day1/Data.txt"))
        {
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var firstInt = line!.First(c => int.TryParse(c+"",out int _));
                var lastInt = line!.Last(c => int.TryParse(c+"",out int _));
                var wholeInt = char.GetNumericValue(firstInt) * 10 + char.GetNumericValue(lastInt);
                sumTotal += (int)wholeInt;
            }
        }
        Console.WriteLine(sumTotal);
    }
    
    [Fact]
    public void Test_1_2()
    {
        var sumTotal = 0;
        var wordNumbers = new List<string>
            { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "0", "1","2","3","4","5","6","7","8","9" };
        using (var sr = new StreamReader("/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day1/Data.txt"))
        {
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var indexes = new List<(int, int)>();
                foreach(var w in wordNumbers)
                {
                    if (line!.Contains(w))
                    {
                        indexes.Add(new() { Item1 = line.IndexOf(w, StringComparison.Ordinal), Item2 = wordNumbers.IndexOf(w) % 10 });
                        indexes.Add(new() { Item1 = line.LastIndexOf(w, StringComparison.Ordinal), Item2 = wordNumbers.LastIndexOf(w) % 10 });
                    }
                }

                var firstInt = indexes.MinBy(t => t.Item1).Item2;
                var lastInt = indexes.MaxBy(t => t.Item1).Item2;
                var wholeInt = firstInt * 10 + lastInt;
                sumTotal += wholeInt;
            }
        }
        Console.WriteLine(sumTotal);
    }
}