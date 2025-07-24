using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_7;

public class Day_7
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 7;

    public Day_7(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 3749)]
    [InlineData(false, 0, 663613490587L)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 11387)]
    [InlineData(false, 0, 110365987435001L)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput_2(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private long ParseInput(List<string> input)
    {
        var total = 0l;

        foreach (var calc in input)
        {
            WriteLine(calc);
            var split = calc.Split(": ");
            var solution = long.Parse(split[0]);
            var values = split[1].Split(' ').ToList().ConvertAll(int.Parse);

            var solved = TrySolve(solution, values);
            if (solved)
            {
                total += solution;
            }
        }
        
        return total;
    }

    private bool TrySolve(long ans, List<int> values, int depth = 1)
    {
        bool solved;
        if (values.Count+1 == depth)
        {
            return false;
        }
        var value = values[^depth];
        if (ans == value)
        {
            WriteLine(value);
            return true;
        }
        if (ans % value == 0)
        {
            solved = TrySolve(ans / value, values, depth + 1);
            if (solved)
            {
                WriteLine($"*{value}");
                return solved;
            }
        }
        solved = TrySolve(ans - value, values, depth + 1);
        if (solved)
        {
            WriteLine($"+{value}");
            return solved;
        }
        return false;
    }

    private long ParseInput_2(List<string> input)
    {
        var total = 0L;
        
        foreach (var calc in input)
        {
            WriteLine();
            WriteLine(calc);
            var split = calc.Split(": ");
            var solution = long.Parse(split[0]);
            var values = split[1].Split(' ').ToList().ConvertAll(int.Parse);

            var solved = TrySolve_2(solution, values);
            if (solved)
            {
                total += solution;
            }
        }
        
        return total;
    }
    
    private bool TrySolve_2(long ans, List<int> values, int depth = 1)
    {
        bool solved;
        if (values.Count+1 == depth)
        {
            return false;
        }
        var value = values[^depth];
        if (ans < value)
        {
            return false;
        }
        if (ans == value && values.Count == depth)
        {
            WriteLine(value);
            return true;
        }
        if (ans % value == 0)
        {
            solved = TrySolve_2(ans / value, values, depth + 1);
            if (solved)
            {
                WriteLine($"*{value}");
                return solved;
            }
        }
        solved = TrySolve_2(ans - value, values, depth + 1);
        if (solved)
        {
            WriteLine($"+{value}");
            return solved;
        }

        if (ans.ToString().EndsWith(value.ToString()))
        {
            var shortAns = ((ans - value) / Math.Pow(10, value.ToString().Length)).ToString();
            solved = TrySolve_2(long.Parse(shortAns), values, depth + 1);
            if (solved)
            {
                WriteLine($"||{value}");
                return solved;
            }
        }
        return false;
    }

    
    private List<string> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<string>();
        
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day_{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input.Add(line);
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