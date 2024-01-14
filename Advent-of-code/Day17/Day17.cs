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
    [InlineData(true, 0, 102, 3, false)]
    [InlineData(false, 0, 660, 3, false)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer, int weight, bool useNewRule)
    {
        var start = DateTime.Now;
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        int total = 0;
        WriteLine();
        var subTotal = RunLogic3(data, weight, useNewRule);
        WriteLine($"subTotal: {subTotal} with weight: {weight}");

        total = subTotal;
        
        WriteLine();
        var end = DateTime.Now;
        WriteLine($"Total: {total} < 998  {end-start} < 00.4125400");
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
        var current = data[0][0].Clone();
        current.CostToReach = 0;
        var endPosition = data[^1][^1];
        var queue = new PriorityQueue<Position,int>(Comparer<int>.Create((a,b)=>a-b));
        queue.Enqueue(current, 0);
        var count = 0;
        var history = new Dictionary<string, int>();
        while (current.Row != endPosition.Row || current.Col != endPosition.Col)
        {
            current = queue.Dequeue();
            ProcessQueueItem(current, data, history, endPosition, queue);

            count++;
        }
        WriteLine($"Count: {count} < 34366");
        //PrintMap(current, data);
        return current.CostToReach;
    }
    
    private void ProcessQueueItem(Position current1, IReadOnlyList<List<Position>> data, Dictionary<string, int> history1, Position goal, PriorityQueue<Position,int> queue1)
    {
        var neighbors = FindNeighbors(data, current1);
        foreach (var neighbor in neighbors)
        {
            var costToReach = current1.CostToReach + neighbor.CostToMoveTo;
            var (isFaster, dirCount) = IsFaster(neighbor, history1, costToReach);
            if (!isFaster)
            {
                continue;
            }
                
            var newNext = neighbor.Clone();
            newNext.CostToReach = costToReach;
            newNext.Path = current1.Path.TakeLast(4).ToList();
            newNext.Path.Add(current1.Clone());
            newNext.DirCount = dirCount;

            var distance = ((goal.Row - newNext.Row) + (goal.Col - newNext.Col));
            var weight = 3;
            queue1.Enqueue(newNext, costToReach + (distance * weight));
                
            history1.TryAdd($"{newNext.Key}_{newNext.Dir}_{dirCount}", newNext.CostToReach);
            history1[$"{newNext.Key}_{newNext.Dir}_{dirCount}"] = newNext.CostToReach;
        }
    }
    
    private int RunLogic3(IReadOnlyList<List<Position>> data, int weight, bool useNewRule)
    {
        var startPosition = data[0][0].Clone();
        var endPosition = data[^1][^1].Clone();
        
        var current1 = startPosition.Clone();
        current1.CostToReach = 0;
        var queue1 = new PriorityQueue<Position,int>(Comparer<int>.Create((a,b)=>a-b));
        queue1.Enqueue(current1, 0);
        var history1 = new Dictionary<string, int>();
        var high = new Dictionary<string, Position> { {startPosition.Key, startPosition.Clone()} };
        
        var current2 = endPosition.Clone();
        current2.CostToReach = current2.CostToMoveTo;
        var queue2 = new PriorityQueue<Position,int>(Comparer<int>.Create((a,b)=>a-b));
        queue2.Enqueue(current2, 0);
        var history2 = new Dictionary<string, int>();
        var low = new Dictionary<string, Position> { {endPosition.Key, endPosition.Clone()} };
        
        var count = 0;
        while (!low.ContainsKey(current1.Key) || !high.ContainsKey(current2.Key))//(!current1.Is(current2))
        {
            current1 = queue1.Dequeue();
            if (current1.Row + current1.Col >= high.Last().Value.Row + high.Last().Value.Col)
            {
                if (current1.Key == "64_45")
                {
                    PrintMap(current1,data);
                }
                if (!high.TryAdd(current1.Key, current1))
                {
                    if (high[current1.Key].CostToReach < current1.CostToReach)
                    {
                        high[current1.Key] = current1;
                    }
                }
                else
                {
                    WriteLine($"high: {high.Last().Value.Key}");   
                }
            }
            ProcessQueueItem2(current1, data, history1, endPosition, queue1, false);
            if (low.ContainsKey(current1.Key) || high.ContainsKey(current2.Key))
            {
                break;
            }
            current2 = queue2.Dequeue();
            if (current2.Row + current2.Col <= low.Last().Value.Row + low.Last().Value.Col)
            {
                if (current2.Key == "64_45")
                {
                    PrintMap(current2,data);
                }
                if (!low.TryAdd(current2.Key, current2))
                {
                    if (low[current2.Key].CostToReach < current2.CostToReach)
                    {
                        low[current2.Key] = current2;
                    }
                }
                else
                {
                    WriteLine($"low: {low.Last().Value.Key}");
                }
            }
            ProcessQueueItem2(current2, data, history2, startPosition, queue2, true);

            count+=2;
        }

        var cur = endPosition.Clone();
        WriteLine($"Count: {count} < 34366");
        if (low.ContainsKey(current1.Key))
        {
            cur.CostToReach = current1.CostToReach + low[current1.Key].CostToReach;
            cur.Path = current1.Path.Concat(low[current1.Key].Path).ToList();
            PrintMap(cur, data);
        }
        if (high.ContainsKey(current2.Key))
        {
            cur.CostToReach = current2.CostToReach + high[current2.Key].CostToReach;
            cur.Path = current2.Path.Concat(high[current2.Key].Path).ToList();
            PrintMap(cur, data);
        }

        return cur.CostToReach;
    }

    private void ProcessQueueItem2(Position current1, IReadOnlyList<List<Position>> data, Dictionary<string, int> history1, Position goal, PriorityQueue<Position,int> queue1, bool isReversed)
    {
        var neighbors = FindNeighbors(data, current1);
        foreach (var neighbor in neighbors)
        {
            var costToReach = current1.CostToReach + neighbor.CostToMoveTo;
            var (isFaster, dirCount) = IsFaster(neighbor, history1, costToReach);
            if (!isFaster)
            {
                continue;
            }
                
            var newNext = neighbor.Clone();
            newNext.CostToReach = costToReach;
            newNext.Path = current1.Path.ToList();
            newNext.Path.Add(current1.Clone());
            newNext.DirCount = dirCount;

            var distance = ((goal.Row - newNext.Row) + (goal.Col - newNext.Col));
            if (isReversed)
            {
                distance = ((newNext.Row) + (newNext.Col));
            }
            var weight = 3;
            queue1.Enqueue(newNext, costToReach + (distance * weight));
                
            history1.TryAdd($"{newNext.Key}_{newNext.Dir}_{dirCount}", newNext.CostToReach);
            history1[$"{newNext.Key}_{newNext.Dir}_{dirCount}"] = newNext.CostToReach;
        }
    }

    private (bool,int) IsFaster(Position current, Dictionary<string, int> history, int costToReach)
    {
        var dirCount = 1;
        for (var index = 1; index < current.Path.Count; index++)
        {
            if (current.Path[^index].Dir != current.Dir)
            {
                break;
            }
            dirCount += 1;
        }

        if (history.TryGetValue($"{current.Key}_{current.Dir}_{dirCount}", out var otherCostToReach))
        {
            if (otherCostToReach + 1 < costToReach)
            {
                return (false,dirCount);
            }
        }

        return (true, dirCount);
    }

    private List<Position> FindNeighbors(IReadOnlyList<List<Position>> data, Position current)
    {
        var previous = current.Path.Count >= 1 ? current.Path[^1] : null;
        var threeAgo = current.Path.Count >= 3 ? current.Path[^3] : null;
        var neighbors = new List<Position>();
        // not off the grid, not backwards and not same direction for 3 in a row
        if (current.Row > 0 && (previous == null || previous.Row != current.Row-1 ) && (threeAgo == null || threeAgo.Row != current.Row+3))
        {
            // Up
            neighbors.Add(data[current.Row-1][current.Col].Clone());
            neighbors[^1].Dir = Direction.Up;
        }
        if (current.Col > 0 && (previous == null || previous.Col != current.Col-1 ) && (threeAgo == null || threeAgo.Col != current.Col+3))
        {
            // Left
            neighbors.Add(data[current.Row][current.Col-1].Clone());
            neighbors[^1].Dir = Direction.Left;
        }
        if (current.Row < data.Count-1 && (previous == null || previous.Row != current.Row+1 ) && (threeAgo == null || threeAgo.Row != current.Row-3))
        {
            // Down
            neighbors.Add(data[current.Row+1][current.Col].Clone());
            neighbors[^1].Dir = Direction.Down;
        }
        if (current.Col < data[0].Count-1 && (previous == null || previous.Col != current.Col+1 ) && (threeAgo == null || threeAgo.Col != current.Col-3))
        {
            // Right
            neighbors.Add(data[current.Row][current.Col+1].Clone());
            neighbors[^1].Dir = Direction.Right;
        }

        return neighbors;
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Null
    }
    
    private class Position
    {
        public readonly int Row;
        public readonly int Col;
        public int CostToReach;
        public int CostToMoveTo;
        public Direction Dir;
        public List<Position> Path = new();
        public int DirCount;
        public string Key => $"{Row:00}_{Col:00}";

        public Position(int row, int col, int costToMoveTo, int costToReach = int.MaxValue, Direction dir = Direction.Null)
        {
            Row = row;
            Col = col;
            CostToMoveTo = costToMoveTo;
            CostToReach = costToReach;
            Dir = dir;
        }

        public Position Clone()
        {
            return new Position(Row, Col, CostToMoveTo, CostToReach, Dir);
        }

        public bool Is(object? obj)
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

        public string ToPrint()
        {
            return $"R/C: {Key} Cost: {CostToReach} {Dir} for {DirCount}";
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
                    rowToPrint += "#";
                    continue;
                }
                rowToPrint += ".";
            }
            WriteLine(rowToPrint);
        }
        WriteLine();
        WriteLine();
    }
}