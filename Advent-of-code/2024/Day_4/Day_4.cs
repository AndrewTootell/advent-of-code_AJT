using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_4;

public class Day_4
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 4;

    public Day_4(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 18)]
    [InlineData(false, 0, 2532)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(new WordSearch(input));
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }q
    [Theory]
    [InlineData(true, 0, 9)]
    [InlineData(false, 0, 1941)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput_2(new WordSearch(input));
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private int ParseInput(WordSearch input)
    {
        var total = 0;

        for (var rowIndex = 0; rowIndex < input.RowCount; rowIndex++)
        {
            for (var colIndex = 0; colIndex < input.ColCount; colIndex++)
            {
                var currentChar = input.GetValue(rowIndex, colIndex);
                if (currentChar != 'X')
                {
                    continue;
                }

                var dirs = input.CheckSurrounding(rowIndex, colIndex, 'M');

                foreach (var dir in dirs)
                {
                    if (input.GetOffsetValue(rowIndex, colIndex, dir, 2) == 'A' && input.GetOffsetValue(rowIndex, colIndex, dir, 3) == 'S')
                    {
                        total += 1;
                    }
                }

            }
        }
        
        return total;
    }

    private int ParseInput_2(WordSearch input)
    {
        var total = 0;

        var aLocations = new List<int>();

        for (var rowIndex = 0; rowIndex < input.RowCount; rowIndex++)
        {
            for (var colIndex = 0; colIndex < input.ColCount; colIndex++)
            {
                var currentChar = input.GetValue(rowIndex, colIndex);
                if (currentChar != 'M')
                {
                    continue;
                }

                var dirs = input.CheckSurrounding(rowIndex, colIndex, 'A');

                foreach (var dir in dirs)
                {
                    if (new List<Direction> { Direction.upper_middle, Direction.bottom_middle, Direction.middle_left, Direction.middle_right }.Contains(dir))
                    {
                        continue;
                    }
                    if (input.GetOffsetValue(rowIndex, colIndex, dir, 2) == 'S')
                    {
                        var aLocation = input.GetOffsetLocation(rowIndex, colIndex, dir);
                        if (aLocations.Contains(aLocation))
                        {
                            total += 1;
                        }
                        else
                        {
                            aLocations.Add(aLocation);
                        }
                    }
                }
            }
        }

        WriteLine(string.Join(',', aLocations));
        
        return total;
    }

    private class WordSearch
    {
        private readonly List<List<char>> _grid;
        public readonly int RowCount;
        public readonly int ColCount;

        public WordSearch(List<List<char>> input)
        {
            _grid = input;
            RowCount = _grid.Count;
            ColCount = _grid[0].Count;
        }

        public char GetValue(int rowIndex, int colIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount)
            {
                return '-';
            }

            var row = _grid[rowIndex];
            if (colIndex < 0 || colIndex >= ColCount)
            {
                return '-';
            }

            return row[colIndex];
        }

        public int GetOffsetLocation(int rowIndex, int colIndex, Direction offsetDirection, int offsetMagnitude = 1)
        {
            return offsetDirection switch
            {
                Direction.upper_left => (rowIndex - offsetMagnitude) * 1_000_000 + colIndex - offsetMagnitude,
                Direction.upper_middle => (rowIndex - offsetMagnitude) * 1_000_000 + colIndex,
                Direction.upper_right => (rowIndex - offsetMagnitude) * 1_000_000 + colIndex + offsetMagnitude,
                Direction.middle_left => (rowIndex) * 1_000_000 + colIndex - offsetMagnitude,
                Direction.middle_right => (rowIndex) * 1_000_000 + colIndex + offsetMagnitude,
                Direction.bottom_left => (rowIndex + offsetMagnitude) * 1_000_000 + colIndex - offsetMagnitude,
                Direction.bottom_middle => (rowIndex + offsetMagnitude) * 1_000_000 + colIndex,
                Direction.bottom_right => (rowIndex + offsetMagnitude) * 1_000_000 + colIndex + offsetMagnitude,
                _ => throw new ArgumentOutOfRangeException(nameof(offsetDirection), offsetDirection, null)
            };
        }

        public char GetOffsetValue(int rowIndex, int colIndex, Direction offsetDirection, int offsetMagnitude = 1)
        {
            return offsetDirection switch
            {
                Direction.upper_left => GetValue(rowIndex - offsetMagnitude, colIndex - offsetMagnitude),
                Direction.upper_middle => GetValue(rowIndex - offsetMagnitude, colIndex),
                Direction.upper_right => GetValue(rowIndex - offsetMagnitude, colIndex + offsetMagnitude),
                Direction.middle_left => GetValue(rowIndex, colIndex - offsetMagnitude),
                Direction.middle_right => GetValue(rowIndex, colIndex + offsetMagnitude),
                Direction.bottom_left => GetValue(rowIndex + offsetMagnitude, colIndex - offsetMagnitude),
                Direction.bottom_middle => GetValue(rowIndex + offsetMagnitude, colIndex),
                Direction.bottom_right => GetValue(rowIndex + offsetMagnitude, colIndex + offsetMagnitude),
                _ => throw new ArgumentOutOfRangeException(nameof(offsetDirection), offsetDirection, null)
            };
        }

        public List<Direction> CheckSurrounding(int rowIndex, int colIndex, char lookingFor)
        {
            var ret = new List<Direction>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                if (GetOffsetValue(rowIndex, colIndex, dir) == lookingFor)
                {
                    ret.Add(dir);
                }
            }
            return ret;
        }
        
    }

    private enum Direction
    {
        upper_left,
        upper_middle,
        upper_right,
        middle_left,
        middle_right,
        bottom_left,
        bottom_middle,
        bottom_right
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