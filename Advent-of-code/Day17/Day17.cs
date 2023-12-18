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
    [InlineData(true, 0, 103)]
    [InlineData(true, 1, 1)]
    [InlineData(false, 0, 670)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        long total = RunLogic(data);
        total.Should().BeLessThan(expectedAnswer);
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

    private int RunLogic(IReadOnlyList<List<Position>> data)
    {
        var startPosition = data.First().First();
        startPosition.CostToReach = 0;
        var endPosition = data.Last().Last();
        var frontier = new PriorityQueue<Position,int>(Comparer<int>.Create((a,b)=>a-b));
        frontier.Enqueue(startPosition, 0);

        var path = new Dictionary<string, Position?> { { startPosition.Key, null } };
        var count = 0;
        Position current = startPosition;
        while (frontier.Count != 0)
        {
            count++;
            current = frontier.Dequeue();
            var neighbors = FindNeighbors(data, current, path[current.Key]);
            foreach (var next in neighbors)
            {
                var isSameRowOrCol = IsSameRowOrCol(current, next, path);
                var costToReach = current.CostToReach + next.CostToMoveTo;
                if (isSameRowOrCol || (path.ContainsKey(next.Key) && next.CostToReach < costToReach))
                {
                    continue;
                }
                next.CostToReach = costToReach;
                if (path.TryGetValue(next.Key, out _))
                {
                    path[next.Key] = current;
                    continue;
                }
                path.Add(next.Key, current);
                frontier.Enqueue(next, costToReach); // + (endPosition.Row - next.Row) + (endPosition.Col - next.Col));
            }
        }
        WriteLine($"Count: {count}");
        
        
        foreach (var positions in data)
        {
            var newRow = "";
            foreach (var position in positions)
            {
                newRow += $" {position.CostToReach:000} ";
            }
            WriteLine(newRow);
        }
        
        var pathPos = endPosition;
        var stringToLog = "";
        var cost = 0;
        while (pathPos != startPosition)
        {
            cost += pathPos.CostToMoveTo;
            stringToLog = $" {pathPos.Key} =>{stringToLog}";
            pathPos = path[pathPos.Key];
        }
        stringToLog = $" {pathPos.Key} =>{stringToLog}";
        WriteLine($"cost: {cost}");
        WriteLine(stringToLog);
        
        foreach (var positions in data)
        {
            var newRow = "";
            foreach (var position in positions)
            {
                newRow += stringToLog.Contains($" {position.Key} ") ? '#' : '.';
            }
            WriteLine(newRow);
        }
        return endPosition.CostToReach;
    }

    private bool IsSameRowOrCol(Position current, Position next, Dictionary<string,Position?> path)
    {
        Position previousPos = current;
        var sameRow = current.Row == next.Row;
        var sameCol = current.Col == next.Col;
        for (var i = 0; i < 3; i++)
        {
            var nextPrevious = path[previousPos.Key];
            if (nextPrevious == null)
            {
                sameRow = false;
                sameCol = false;
                break;
            }
            sameRow = sameRow && nextPrevious.Row == previousPos.Row;
            sameCol = sameCol && nextPrevious.Col == previousPos.Col;
            previousPos = nextPrevious;
        }

        return sameRow || sameCol;
    }
    
    private List<Position> FindNeighbors(IReadOnlyList<List<Position>> data, Position current, Position? previous)
    {
        var stepSize = 1;
        var neighbors = new List<Position>();
        if (current.Row > 1-stepSize && (previous == null || previous.Row != current.Row-1 ))
        {
            neighbors.Add(data[current.Row-stepSize][current.Col]);
        }
        if (current.Col > 1-stepSize && (previous == null || previous.Col != current.Col-1 ))
        {
            neighbors.Add(data[current.Row][current.Col-stepSize]);
        }
        if (current.Row < data.Count-stepSize && (previous == null || previous.Row != current.Row+1 ))
        {
            neighbors.Add(data[current.Row+stepSize][current.Col]);
        }
        if (current.Col < data[0].Count-stepSize && (previous == null || previous.Col != current.Col+1 ))
        {
            neighbors.Add(data[current.Row][current.Col+stepSize]);
        }

        return neighbors;
    }
    
    private class Position
    {
        public readonly int Row;
        public readonly int Col;
        public int CostToReach = int.MaxValue;
        public int CostToMoveTo;
        public string Key => $"{Row:00}_{Col:00}";

        public Position(int row, int col, int costToMoveTo)
        {
            Row = row;
            Col = col;
            CostToMoveTo = costToMoveTo;
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