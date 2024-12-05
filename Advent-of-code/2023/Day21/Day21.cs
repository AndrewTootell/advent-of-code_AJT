using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day21;

public class Day21
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 21;
    private Position _startPos;

    public Day21(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 1, 50, 5)]
    //[InlineData(true, 1, 16, 6)]
    //[InlineData(true, 1, 50, 10)]
    //[InlineData(true, 1, 50, 20)]
    //[InlineData(true, 1, 1594, 50)]
    //[InlineData(false, 1, 3651, 64)]
    // [InlineData(false, 0, 6536, 100)]
    // [InlineData(false, 0, 167004, 500)]
    // [InlineData(false, 0, 668697, 1000)]
    // [InlineData(false, 0, 16733044, 5000)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer, int steps)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        int total = RunLogic2(data, steps);
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

    private void PrintMap(IReadOnlyList<List<Position>> data, int maxDelta = int.MaxValue)
    {
        for (var rowIndex = 0; rowIndex < data.Count(); rowIndex++)
        {
            var rowDelta = Math.Abs(_startPos.Row-rowIndex);
            if (rowDelta > maxDelta)
            {
                continue;
            }
            var printRow = "";
            for (var colIndex = 0; colIndex < data[rowIndex].Count; colIndex++)
            {
                var colDelta = Math.Abs(_startPos.Col-colIndex);
                if (colDelta > maxDelta)
                {
                    continue;
                }
                var pos = data[rowIndex][colIndex];
                if (!pos.CanMoveTo)
                {
                    printRow += "#";
                    continue;
                }

                if (pos.Row == _startPos.Row && pos.Col == _startPos.Col)
                {
                    printRow += "S";
                    continue;
                }

                var costToReach = pos.CostToReach;
                if (costToReach < int.MaxValue && costToReach % 2 == 0)
                {
                    printRow += "O";
                    continue;
                }
                printRow += ".";
            }
            WriteLine(printRow);
        }
    }
    
    private int RunLogic2(IReadOnlyList<List<Position>> data, int steps)
    {
        var current = _startPos;
        current.CostToReach = 0;
        var queue = new Queue<Position>();
        queue.Enqueue(current);
        var evenPositions = new List<Position>();
        var history = new Dictionary<string, Position>();
        var count = 0;
        var highestDelta = 1;
        var print = false;
        while (queue.Count != 0)// && count < 10000)
        {
            current = queue.Dequeue();
            if (current.Col - _startPos.Col > highestDelta)
            {
                highestDelta = current.Col - _startPos.Col;
                if (highestDelta % 2 == 0)
                {
                    print = true;
                }
            }
            var neighbors = FindNeighbors(data, current, history);
            foreach (var neighbor in neighbors)
            {
                var costToReach = current.CostToReach + 1;
                if (costToReach > steps)
                {
                    continue;
                }
                if (!neighbor.CanMoveTo)
                {
                    continue;
                }
                
                var newNext = neighbor;
                newNext.CostToReach = costToReach;
                queue.Enqueue(newNext);
                var newValue = history.TryAdd(newNext.Key, newNext);
                if (!newValue)
                {
                    history[newNext.Key] = newNext;
                }
                if (newValue && costToReach % 2 == 0)
                {
                    evenPositions.Add(newNext);
                }
            }

            count++;
            if (print)
            {
                WriteLine($"{count}  {highestDelta}  {evenPositions.Count}");
                PrintMap(data, highestDelta);
                print = false;
            }
        }
        WriteLine($"{count}");
        PrintMap(data);
        return evenPositions.Count;
    }

    private List<Position> FindNeighbors(IReadOnlyList<List<Position>> data, Position current, Dictionary<string, Position> history)
    {
        var stepSize = 1;
        var neighbors = new List<Position>();
        if (current.Row > 1-stepSize && !history.ContainsKey($"{current.Row-stepSize}_{current.Col}"))
        {
            // Up
            neighbors.Add(data[current.Row-stepSize][current.Col]);
        }
        if (current.Col > 1-stepSize && !history.ContainsKey($"{current.Row}_{current.Col-stepSize}"))
        {
            // Left
            neighbors.Add(data[current.Row][current.Col-stepSize]);
        }
        if (current.Row < data.Count-stepSize && !history.ContainsKey($"{current.Row+stepSize}_{current.Col}"))
        {
            // Down
            neighbors.Add(data[current.Row+stepSize][current.Col]);
        }
        if (current.Col < data[0].Count-stepSize && !history.ContainsKey($"{current.Row}_{current.Col+stepSize}"))
        {
            // Right
            neighbors.Add(data[current.Row][current.Col+stepSize]);
        }

        return neighbors;
    }

    private class Position
    {
        public readonly int Row;
        public readonly int Col;
        public bool CanMoveTo;
        public int CostToReach;
        public List<Position> Path = new();
        public string Key => $"{Row:00}_{Col:00}";

        public Position(int row, int col, bool canMoveTo, int costToReach = int.MaxValue)
        {
            Row = row;
            Col = col;
            CanMoveTo = canMoveTo;
            CostToReach = costToReach;
        }

        public Position Clone()
        {
            return new Position(Row, Col, CanMoveTo, CostToReach);
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

    private List<List<Position>> ParseInput(List<List<char>> input)
    {
        var data = new List<List<Position>>();
        for (var rowIndex = 0; rowIndex < input.Count; rowIndex++)
        {
            var newRow = new List<Position>();
            for (var colIndex = 0; colIndex < input[rowIndex].Count; colIndex++)
            {
                var symbol = input[rowIndex][colIndex];
                var pos = new Position(rowIndex, colIndex, symbol!='#');
                newRow.Add(pos);
                if (symbol == 'S')
                {
                    _startPos = pos;
                }
            }
            data.Add(newRow);
        }
        return data;
    }
    
    private List<List<char>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<char>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input.Add(line.ToList());
        }
        return input;
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}