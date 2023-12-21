using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day24;

public class Day24
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 24;

    public Day24(ITestOutputHelper testOutputHelper)
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
        public readonly int Row;
        public readonly int Col;
        public int CostToReach;
        public int CostToMoveTo;
        public List<Position> Path = new();
        public string Key => $"{Row:00}_{Col:00}";

        public Position(int row, int col, int costToMoveTo, int costToReach = int.MaxValue)
        {
            Row = row;
            Col = col;
            CostToMoveTo = costToMoveTo;
            CostToReach = costToReach;
        }

        public Position Clone()
        {
            return new Position(Row, Col, CostToMoveTo, CostToReach);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(Position))
            {
                return false;
            }

            var pos = (Position)obj;
            return Row == pos.Row && Col == pos.Col;
        }
    }

    private List<List<Position>> ParseInput(List<List<int>> input)
    {
        var data = new List<List<Position>>();
        for (var rowIndex = 0; rowIndex < input.Count; rowIndex++)
        {
            var newRow = new List<Position>();
            for (var colIndex = 0; colIndex < input[rowIndex].Count; colIndex++)
            {
                var costToMoveTo = input[rowIndex][colIndex];
                var pos = new Position(rowIndex, colIndex, costToMoveTo);
                newRow.Add(pos);
            }
            data.Add(newRow);
        }
        return data;
    }
    
    private List<List<int>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<int>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
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