using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_2;

public class Day_2
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 2;

    public Day_2(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 2)]
    [InlineData(false, 0, 591)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 4)]
    [InlineData(false, 0, 621)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput_2(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private int ParseInput(List<List<int>> input)
    {
        var total = 0;

        foreach (var level in input)
        {
            if ((IsAscending(level).Count==0 || IsDecending(level).Count==0) && IsClose(level).Count==0)
            {
                total++;
            }
        }
        
        return total;
    }

    private int ParseInput_2(List<List<int>> input)
    {
        var total = 0;

        foreach (var level in input)
        {
            WriteLine(string.Join(", ",level.ConvertAll(i=>i.ToString()).ToArray()));

            if (IsPassed(level, IsClose(level), IsAscending(level), IsDecending(level)))
            {
                WriteLine("True!");
                total++;
            }
            WriteLine();
        }
        
        return total;
    }

    private bool IsPassed(List<int> level, List<int> close, List<int> asc, List<int> dsc)
    {
        if ((asc.Count == 0 || dsc.Count == 0) && close.Count == 0)
        {
            return true;
        }
        
        if (close.Count > 1 || (asc.Count > 1 && dsc.Count > 1))
        {
            WriteLine("Too many Errors!");
            return false;
        }

        for (var index = 0; index < level.Count; index++)
        {
            var clone = new List<int>(level);
            clone.RemoveAt(index);
            if ((IsAscending(clone).Count == 0 || IsDecending(clone).Count == 0) && IsClose(clone).Count == 0)
            {
                return true;
            }
        }

        return false;
    }

    private List<int> IsDecending(List<int> row)
    {
        var count = new List<int>();
        for (var index = 1; index < row.Count; index++)
        {
            if (row[index - 1] <= row[index])
            {
                count.Add(index);
            }
        }

        return count;
    }
    
    private List<int> IsAscending(List<int> row)
    {
        var count = new List<int>();
        for (var index = 1; index < row.Count; index++)
        {
            if (row[index - 1] >= row[index])
            {
                count.Add(index);
            }
        }

        return count;
    }
    
    private List<int> IsClose(List<int> row)
    {
        var count = new List<int>();
        for (var index = 1; index < row.Count; index++)
        {
            if (Math.Abs(row[index - 1] - row[index]) > 3)
            {
                count.Add(index);
            }
        }

        return count;
    }
    
    
    private List<List<int>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<int>>();
        
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day_{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            var values = line.Split(" ");
            input.Add(values.ToList().ConvertAll(c=>int.Parse($"{c}")));
        }
        return input;
    }
    
    private void WriteLine(int message)
    {
        _testOutputHelper.WriteLine(message.ToString());
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}