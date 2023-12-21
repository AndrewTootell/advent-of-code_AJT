using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day18;

public class Day18
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 18;

    public Day18(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 62)]
    [InlineData(false, 0, 660)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        int total = RunLogic(data);
        
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

    private int RunLogic(List<Command> data)
    {
        var board = new List<List<Position>>();
        for (var rowIndex = 0; rowIndex <= data.Sum(cmd => cmd.Dir == Direction.Down ? cmd.Count+1 : 0)*2; rowIndex++)
        {
            var newRow = new List<Position>();
            for (var colIndex = 0; colIndex <= data.Sum(cmd => cmd.Dir == Direction.Right ? cmd.Count+1 : 0)*2; colIndex++)
            {
                newRow.Add(new Position(rowIndex, colIndex));
            }
            board.Add(newRow);
        }

        var currentPos = board[board.Count/2][board.First().Count/2];
        var wallDirection = Direction.Up;

        foreach (var cmd in data)
        {
            for (var i = 0; i < cmd.Count; i++)
            {
                //WriteLine(currentPos.Key);
                currentPos.Colors.Add((wallDirection,cmd.ColorCode));
                currentPos.DirectionsDugIn.Add(cmd.Dir);
                currentPos = cmd.Dir switch
                {
                    Direction.Up => board[currentPos.Row - 1][currentPos.Col],
                    Direction.Down => board[currentPos.Row + 1][currentPos.Col],
                    Direction.Left => board[currentPos.Row][currentPos.Col - 1],
                    Direction.Right => board[currentPos.Row][currentPos.Col + 1]
                };

                if (i != cmd.Count-1)
                {
                    continue;
                }
                currentPos.Colors.Add((wallDirection,cmd.ColorCode));
                currentPos.DirectionsDugIn.Add(cmd.Dir);
                wallDirection = cmd.Dir;
            }
        }

        var ans = 0;
        var toggle = false;
        for (var rowIndex = 0; rowIndex < board.Count; rowIndex++)
        {
            var rowToPrint = "";
            for (var colIndex = 0; colIndex < board.First().Count; colIndex++)
            {
                var pos = board[rowIndex][colIndex];
                if (pos.DirectionsDugIn.Contains(Direction.Down))
                {
                    ans += 1;
                    toggle = false;
                    rowToPrint += "| ";
                    continue;
                }
                if (pos.DirectionsDugIn.Contains(Direction.Up))
                {
                    ans += 1;
                    toggle = true;
                    rowToPrint += "| ";
                    continue;
                }
                if (pos.DirectionsDugIn.Count > 0)
                {
                    ans += 1;
                    rowToPrint += "# ";
                    continue;
                }
                ans += toggle ? 1 : 0;
                rowToPrint += toggle ?"# ":". ";
            }
            //WriteLine(rowToPrint);
        }

        return ans;
    }

    private class Position
    {
        public List<(Direction, string)> Colors;
        public List<Direction> DirectionsDugIn;
        public int Row;
        public int Col;
        public string Key => $"{Row}_{Col}";

        public Position(int rowIndex, int colIndex)
        {
            Row = rowIndex;
            Col = colIndex;
            Colors = new List<(Direction, string)>();
            DirectionsDugIn = new List<Direction>();
        }
        
    }

    private class Command
    {
        public int Count;
        public Direction Dir;
        public string ColorCode;

        public Command(string direction, string count, string colorCode)
        {
            Count = int.Parse(count);
            ColorCode = colorCode;
            Dir = direction switch
            {
                "U" => Direction.Up,
                "D" => Direction.Down,
                "L" => Direction.Left,
                "R" => Direction.Right
            };
        }
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Null
    }

    private List<Command> ParseInput2(List<string> input)
    {
        var data = new List<Command>();
        foreach (var str in input)
        {
            var parts = str.Split(' ');
            var hex = parts[2].TrimEnd(')').TrimStart('(');
            data.Add(new Command( "R", "0" , ""));
        }
        return data;
    }

    private List<Command> ParseInput(List<string> input)
    {
        var data = new List<Command>();
        foreach (var str in input)
        {
            var parts = str.Split(' ');
            data.Add(new Command(parts[0], parts[1], parts[2]));
        }
        return data;
    }
    
    private List<string> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<string>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input.Add(line);
        }
        return input;
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}