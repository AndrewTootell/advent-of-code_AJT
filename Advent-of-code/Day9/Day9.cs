
namespace Advent_of_code.Day9;

public class Day9
{
    [Test]
    public void Test_1()
    {
        var data = ReadInput(9, false, 0);
        var sum = 0;
        data.ForEach(d =>
        {
            sum += PredictNext(d);
        });
        
        Console.WriteLine($"Total: {sum}");
        // TestData0: 114
        // Data: 1479011877
    }
    
    [Test]
    public void Test_2()
    {
        var data = ReadInput(9, false, 0);
        var sum = 0;
        data.ForEach(d =>
        {
            sum += PredictPrevious(d);
        });
        
        Console.WriteLine($"Total: {sum}");
        // TestData0: 114
        // Data: 1479011877
    }

    private static int PredictNext(List<int> input)
    {
        var lines = FindDiffs(input);
        
        for (var i = lines.Count-1; i > 0; i--)
        {
            var currentLine = lines[i];
            lines[i-1].Add(currentLine.Last() + lines[i-1].Last());
        }
        
        return lines.First().Last();
    }

    private static int PredictPrevious(List<int> input)
    {
        var lines = FindDiffs(input);
        
        for (var i = lines.Count-1; i > 0; i--)
        {
            var currentLine = lines[i];
            lines[i-1] = lines[i-1].Prepend(lines[i-1].First() - currentLine.First()).ToList();
        }
        // foreach (var line in lines)
        // {
        //     line.ForEach(i =>
        //     {
        //         Console.Write($" {i},");
        //     });
        //     Console.WriteLine();
        // }
        // Console.WriteLine();
        
        
        return lines.First().First();
    }

    private static List<List<int>> FindDiffs(List<int> input)
    {
        var lines = new List<List<int>> { input };
        while (lines.Last().Any(i => i != 0))
        {
            var currentLine = lines.Last();
            var nextLine = new List<int>();
            for (var i = 0; i < currentLine.Count-1; i++)
            {
                nextLine.Add(currentLine[i+1]-currentLine[i]);
            }
            lines.Add(nextLine);
        }

        return lines;
    }
    
    private static List<List<int>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var data = new List<List<int>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            data.Add(line.Split(' ').Where(s=> !string.IsNullOrEmpty(s)).ToList().ConvertAll(int.Parse));
        }
        return data;
    }
}