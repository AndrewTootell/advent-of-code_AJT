using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_6;

public class Day_6
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 6;

    public Day_6(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 41)]
    [InlineData(false, 0, 4967)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 6)]
    [InlineData(false, 0, 1789)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput_2(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private int ParseInput(List<List<char>> input)
    {
        var total = 0;

        var coords = (0, 0);

        for (var rowIndex = 0; rowIndex < input.Count; rowIndex++)
        {
            for (var colIndex = 0; colIndex < input[0].Count; colIndex++)
            {
                var currentChar = input[rowIndex][colIndex];
                if (currentChar == '^')
                {
                    coords = (colIndex, rowIndex);
                    input[coords.Item2][coords.Item1] = 'X';
                    total += 1;
                }
            }
        }

        var direction = Direction.North;
        var walk = true;
        var nextCoords = coords;

        while (walk)
        {
            var step = MapDirection(direction);
            if (input.Count == coords.Item2 + step.Item2 || 0 > coords.Item2 + step.Item2 || 
                input[0].Count == coords.Item1 + step.Item1 || 0 > coords.Item1 + step.Item1)
            {
                break;
            }

            nextCoords.Item2 += step.Item2;
            nextCoords.Item1 += step.Item1;
            var nextSymbol = input[nextCoords.Item2][nextCoords.Item1];
            
            // Take the step
            if (nextSymbol == '^' || nextSymbol == '.' ||
                nextSymbol == 'X')
            {
                coords = nextCoords;
                if (nextSymbol != 'X')
                {
                    input[nextCoords.Item2][nextCoords.Item1] = 'X';
                    total += 1;
                }
            }
            
            // Change direction
            if (nextSymbol == '#')
            {
                nextCoords = coords;
                direction = MapTurnDirection(direction);
            }
        }

        foreach (var row in input)
        {
            WriteLine(string.Join(' ',row));
        }
        
        
        return total;
    }

    private int ParseInput_2(List<List<char>> input)
    {
        var total = 0;
        
        var startCoords = (0, 0);

        for (var rowIndex = 0; rowIndex < input.Count; rowIndex++)
        {
            for (var colIndex = 0; colIndex < input[0].Count; colIndex++)
            {
                var currentChar = input[rowIndex][colIndex];
                if (currentChar == '^')
                {
                    startCoords = (colIndex, rowIndex);
                }
            }
        }

        var count = 0;
        var inputClone = new List<List<char>>();
        foreach (var row in input)
        {
            var l = new char[row.Count];
            row.CopyTo(l);
            inputClone.Add(l.ToList());
        }

        var (path, _) = Walked(startCoords, inputClone, count);
        var c = 0;
        foreach (var step in path)
        {
            var obstacleCoord = step.Item1;
            var index = path.FindIndex(s => s.Item1.Item1 == obstacleCoord.Item1 && s.Item1.Item2 == obstacleCoord.Item2);
            if (index != c)
            {
                c += 1;
                continue;
            }
            c += 1;
            count += 1;
            inputClone.Clear();
            foreach (var row in input)
            {
                var l = new char[row.Count];
                row.CopyTo(l);
                inputClone.Add(l.ToList());
            }
            inputClone[obstacleCoord.Item2][obstacleCoord.Item1] = 'O';
            var (_, looped) = Walked(startCoords, inputClone, count);
            if (looped)
            {
                total += 1;
            }
            
        }
        
        return total;
    }

    private (List<((int, int), Direction)>, bool) Walked((int, int) startCoords, List<List<char>> input, int count)
    {
        var coords = (startCoords.Item1, startCoords.Item2);
        var startDirection = Direction.North; 
        var direction = startDirection;
        var walk = true;
        var nextCoords = coords;
        var looped = false;
        var path = new List<((int, int), Direction)>();
        
        while (walk)
        {
            var stepDiff = MapDirection(direction);
            if (input.Count == coords.Item2 + stepDiff.Item2 || 0 > coords.Item2 + stepDiff.Item2 || 
                input[0].Count == coords.Item1 + stepDiff.Item1 || 0 > coords.Item1 + stepDiff.Item1)
            {
                break;
            }

            nextCoords.Item2 += stepDiff.Item2;
            nextCoords.Item1 += stepDiff.Item1;
            var step = (nextCoords, direction);

            if (path.Contains(step))
            {
                looped = true;
                break;
            
            }
            path.Add(step);
            var nextSymbol = input[nextCoords.Item2][nextCoords.Item1];
            
            // Take the step
            if (nextSymbol == '^' || nextSymbol == '.' ||
                nextSymbol == 'X')
            {
                coords = nextCoords;
                if (nextSymbol != 'X')
                {
                    input[nextCoords.Item2][nextCoords.Item1] = 'X';
                }
            }
            
            // Change direction
            if (nextSymbol == '#' || nextSymbol == 'O')
            {
                nextCoords = coords;
                direction = MapTurnDirection(direction);
            }
        }
        
        return (path,looped);
    }

    private Direction MapTurnDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return Direction.East;
            case Direction.South:
                return Direction.West;
            case Direction.West:
                return Direction.North;
            case Direction.East:
                return Direction.South;
            default:
                return dir;
        }
    }

    private (int, int) MapDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return (0, -1);
            case Direction.South:
                return (0, 1);
            case Direction.West:
                return (-1, 0);
            case Direction.East:
                return (1, 0);
            default:
                return (0, 0);
        }
    }

    private enum Direction
    {
        North,
        West,
        East,
        South
    }
    
    private List<List<char>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<char>>();
        
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day_{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input.Add(line.ToList());
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