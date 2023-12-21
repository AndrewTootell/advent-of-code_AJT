using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day17;

public class Day17
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 17;

    public Day17(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    //[InlineData(true, 0, 102, 10)]
    //[InlineData(false, 0, 660, 10)]
    //[InlineData(false, 0, 660, 9)]
    //[InlineData(false, 0, 660, 8)]
    //[InlineData(false, 0, 660, 7)]
    //[InlineData(false, 0, 660, 6, true)]
    //[InlineData(false, 0, 660, 6, false)]
    [InlineData(false, 0, 660, 5, false)]
    [InlineData(false, 0, 660, 4, false)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer, int weight, bool useNewRule)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        int total = 0;
        WriteLine();
        var subTotal = RunLogic2(data, weight, useNewRule);
        WriteLine($"subTotal: {subTotal} with weight: {weight}");

        total = subTotal;
        
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

    private int RunLogic2(IReadOnlyList<List<Position>> data, int weight, bool useNewRule)
    {
        var current = data.First().First().Clone();
        current.CostToReach = 0;
        var endPosition = data.Last().Last();
        var queue = new PriorityQueue<Position,int>(Comparer<int>.Create((a,b)=>a-b));
        queue.EnsureCapacity(500000);
        queue.Enqueue(current, 0);
        var count = 0;
        var highest = 0;
        while (queue.Count != 0 && (current.Row != endPosition.Row || current.Col != endPosition.Col))
        {
            if (queue.Count > 500000)
            {
                queue.TrimExcess();
            }
            current = queue.Dequeue();
            var skip = false;
            if (useNewRule)
            {
                
            }
            if (skip)
            {
                WriteLine("Rule works!");
                continue;
            }
            // WriteLine(current.Key);
            if (current.Row + current.Col > highest)
            {
                WriteLine($"Cur key: {current.Key} CostToReach: {current.CostToReach}  Queue size: {queue.Count}");
                highest = current.Row + current.Col;
            }
            var neighbors = FindNeighbors(data, current);
            foreach (var neighbor in neighbors)
            {
                if (current.Path.Where(past => past.Row == neighbor.Row && past.Col == neighbor.Col).ToList().Count > 0)
                {
                    continue;
                }
                var costToReach = current.CostToReach + neighbor.CostToMoveTo;
                
                var newNext = neighbor.Clone();
                newNext.CostToReach = costToReach;
                newNext.Path = current.Path.ToList();
                newNext.Path.Add(current.Clone());
                queue.Enqueue(newNext, costToReach + (endPosition.Row - newNext.Row)*weight + (endPosition.Col - newNext.Col)*weight);
            }

            count++;
        }
        WriteLine($"Count: {count}");
        //PrintMap(current, data);
        return current.CostToReach;
    }

    private void PrintMap(Position current, IReadOnlyList<List<Position>> data)
    {
        WriteLine();
        foreach (var row in data)
        {
            var rowToPrint = "";
            foreach (var pos in row)
            {
                if (current.Path.Where(past => past.Row == pos.Row && past.Col == pos.Col).ToList().Count > 0)
                {
                    rowToPrint += "# ";
                    continue;
                }
                rowToPrint += ". ";
            }
            WriteLine(rowToPrint);
        }
        WriteLine();
        WriteLine();
    }

    private List<Position> FindNeighbors(IReadOnlyList<List<Position>> data, Position current)
    {
        var previous = current.Path.Count >= 1 ? current.Path.Last() : null;;
        var threeAgo = current.Path.Count >= 3 ? current.Path[^3] : null;
        if (threeAgo != null)
        {
            var b = "P";
        }
        var stepSize = 1;
        var neighbors = new List<Position>();
        if (current.Row > 1-stepSize && (previous == null || previous.Row != current.Row-1 ) && (threeAgo == null || threeAgo.Row != current.Row+3))
        {
            // Up
            neighbors.Add(data[current.Row-stepSize][current.Col]);
        }
        if (current.Col > 1-stepSize && (previous == null || previous.Col != current.Col-1 ) && (threeAgo == null || threeAgo.Col != current.Col+3))
        {
            // Left
            neighbors.Add(data[current.Row][current.Col-stepSize]);
        }
        if (current.Row < data.Count-stepSize && (previous == null || previous.Row != current.Row+1 ) && (threeAgo == null || threeAgo.Row != current.Row-3))
        {
            // Down
            neighbors.Add(data[current.Row+stepSize][current.Col]);
        }
        if (current.Col < data[0].Count-stepSize && (previous == null || previous.Col != current.Col+1 ) && (threeAgo == null || threeAgo.Col != current.Col-3))
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