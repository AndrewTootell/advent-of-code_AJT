using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_3;

public class Day_3
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 3;

    public Day_3(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 161)]
    [InlineData(false, 0, 187825547)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 1, 48)]
    [InlineData(false, 0, 23741109)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput_2(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private int ParseInput(string input)
    {
        var total = 0;

        var pattern = @"mul\([0-9]{1,3},[0-9]{1,3}\)";
        var matches = new Regex(pattern).Matches(input).Select(m => m);

        foreach (var match in matches)
        {
            var values = match.Value.Split('(')[1].TrimEnd(')').Split(',');
            total += int.Parse(values[0]) * int.Parse(values[1]);
        }
        
        return total;
    }

    enum cmdType
    {
        Start,
        Stop,
        Mul,
    }

    private int ParseInput_2(string input)
    {
        var total = 0;
        
        var commands = new SortedDictionary<int, (cmdType,string)>();

        var doMatch = new Regex(@"do[(][)]").Matches(input).Select(m => m);
        foreach (var match in doMatch)
        {
            commands.Add(match.Index, (cmdType.Start,""));
            WriteLine("has do");
        }
        var dontMatch = new Regex(@"don't[(][)]").Matches(input).Select(m => m);
        foreach (var match in dontMatch)
        {
            commands.Add(match.Index, (cmdType.Stop,""));
            WriteLine("has don't");
        }
        
        var matches = new Regex(@"mul\([0-9]{1,3},[0-9]{1,3}\)").Matches(input).Select(m => m);
        foreach (var match in matches)
        {
            commands.Add(match.Index, (cmdType.Mul,match.Value));
        }

        var doMultiply = true;
        WriteLine(commands.Count);

        foreach (var command in commands)
        {
            switch (command.Value.Item1)
            {
                case cmdType.Start:
                    doMultiply = true;
                    WriteLine("Start!");
                    continue;
                case cmdType.Stop:
                    doMultiply = false;
                    WriteLine("Stop!");
                    continue;
                case cmdType.Mul:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!doMultiply) continue;
            WriteLine(command.Value.Item2);
            var values = command.Value.Item2.Split('(')[1].TrimEnd(')').Split(',');
            total += int.Parse(values[0]) * int.Parse(values[1]);
        }

        
        
        return total;
    }
    
    private string ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = "";
        
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day_{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input += line;
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