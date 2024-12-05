using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Template;

public class Day_
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 25;

    public Day_(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 660)]
    [InlineData(false, 0, 660)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        int total = 0;
        
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 145)]
    [InlineData(false, 0, 284132)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var data = ReadInput(Day, isTest, testDataCount);
        long total = 0;
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine($"total: {total}");
    }

    private class Position
    {
        
    }

    private List<List<int>> ParseInput(List<List<int>> input)
    {
        var data = new List<List<int>>();
        for (var rowIndex = 0; rowIndex < input.Count; rowIndex++)
        {
            var newRow = new List<int>();
            for (var colIndex = 0; colIndex < input[rowIndex].Count; colIndex++)
            {
                var i = input[rowIndex][colIndex];
                newRow.Add(i);
            }
            data.Add(newRow);
        }
        return data;
    }
    
    private List<List<int>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<int>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input.Add(line.ToList().ConvertAll(c=>int.Parse($"{c}")));
        }
        return input;
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}