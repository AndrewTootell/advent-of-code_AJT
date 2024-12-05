using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using Xunit.Abstractions;

namespace Advent_of_code.Day14;

public class Day14
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Day14(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 136)]
    [InlineData(false, 0, 113424)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(14, isTest, testDataCount);
        var controlPanel = ParseData(input);
        long total = 0;
        for (var i = 0; i < controlPanel.Count; i++)
        {
            var rocks = controlPanel[i];
            _testOutputHelper.WriteLine(RocksToString(rocks));
            var movedRocks = MoveAllUp(rocks);
            _testOutputHelper.WriteLine(RocksToString(movedRocks));
            total += CalcValue(movedRocks);
            _testOutputHelper.WriteLine("");
        }
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine("");
    }
    
    [Theory]
    [InlineData(true, 1, 136)]
    public void Test_1_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(14, isTest, testDataCount);
        
        var controlPanel = ParseData(input);
        long total = 0;
        for (var i = 0; i < controlPanel.Count; i++)
        {
            var rocks = controlPanel[i];
            total += CalcValue(rocks);
        }
        total.Should().Be(expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0, 64)]
    [InlineData(false, 0, 96003)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(14, isTest, testDataCount);
        var controlPanel = ParseData(input);
        long subTotal = 0;
        var k = LoopyLoop(controlPanel);
        var index = _cache.Keys.ToList().IndexOf(k);
        var loopCount = _cache.Count - index;
        var remainder = (1000000000-index) % loopCount;
        var key = _cache.Keys.ToList()[remainder + index -1];
        var total = _cache[key];
        _testOutputHelper.WriteLine($"Cache count: {_cache.Count}");
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine($"total: {total}");
    }

    private string LoopyLoop(List<List<Rock>> controlPanel)
    {
        long subTotal;
        var controlPanelLength = controlPanel.Count;
        for (var cycle = 0; cycle < 1000000000; cycle++)
        {
            for (var loop = 0; loop < 4; loop++)
            {
                for (var i = 0; i < controlPanelLength; i++)
                {
                    controlPanel[i] = MoveAllUp(controlPanel[i]).ToList();
                }
                controlPanel = RotateClockwise(controlPanel).ToList();
            }
            if (_cache.TryGetValue(ControlPanelToString(controlPanel), out _))
            {
                return ControlPanelToString(controlPanel);
            }
            subTotal = 0;
            for (var i = 0; i < controlPanelLength; i++)
            {
                subTotal += CalcValue(controlPanel[i]);
            }
            _cache.Add(ControlPanelToString(controlPanel), subTotal);
        }
        return ControlPanelToString(controlPanel);
    }
    

    private Dictionary<string, long> _cache  = new();

    private List<List<Rock>> RotateClockwise(List<List<Rock>> controlPanel)
    {
        List<List<Rock>> result = new();
        for (var i = 0; i < controlPanel.Count; i++)
        {
            result.Add(new List<Rock>());
        }
        for (var rowIndex = 0; rowIndex < controlPanel.Count; rowIndex++)
        {
            for (var colIndex = 0; colIndex < controlPanel[rowIndex].Count; colIndex++)
            {
                var newRow = controlPanel.Count-1-colIndex;
                var e = controlPanel[rowIndex][colIndex];
                result[newRow].Add(e);
            }
        }
        return result;
    }    

    private List<Rock> MoveAllUp(List<Rock> rocks)
    {
        var splitCount = rocks.Count(r => r == Rock.Square);
        var newRocks = new List<Rock>();
        for (var i = 0; i < splitCount+1; i++)
        {
            var splitIndex = rocks.IndexOf(Rock.Square);
            if (splitIndex == -1)
            {
                 var lastMovedRocks = MoveRocks(rocks);
                 newRocks = newRocks.Concat(lastMovedRocks).ToList();
                 break;
            }
            var rocksToMove = rocks.Take(splitIndex).ToList();
            rocks = rocks.Skip(splitIndex+1).ToList();
            var movedRocks = MoveRocks(rocksToMove);
            newRocks = newRocks.Concat(movedRocks).ToList();
            newRocks.Add(Rock.Square);
        }

        //newRocks = newRocks.Take(newRocks.Count-1).ToList();
        return newRocks;
    }

    private List<Rock> MoveRocks(List<Rock> rocksToMove)
    {
        var rockCount = rocksToMove.Count(r => r == Rock.Round);
        var movedRocks = new List<Rock>(Enumerable.Repeat(Rock.Round, rockCount));
        movedRocks = movedRocks.Concat(Enumerable.Repeat(Rock.Empty, rocksToMove.Count - rockCount)).ToList();
        return movedRocks;
    }

    private int CalcValue(List<Rock> rocks)
    {
        var subTotal = 0;
        var index = 0;
        for (var value = rocks.Count; value > 0; value--)
        {
            if (rocks[index] == Rock.Round)
            {
                subTotal += value;
            }
            index++;
        }
        return subTotal;
    }
    
    private List<List<Rock>> ParseData(List<string> input)
    {
        var id = 0;
        var controlPanel = new List<List<Rock>>();
        for (var rowIndex = 0; rowIndex < input.Count; rowIndex++)
        {
            var line = input[rowIndex];
            for (var colIndex = 0; colIndex < line.Length; colIndex++)
            {
                var c = line[colIndex];
                if (controlPanel.Count-1 < colIndex)
                {
                    controlPanel.Add(new List<Rock>());
                }

                var rockType = c switch
                {
                    'O' => Rock.Round,
                    '#' => Rock.Square,
                    '.' => Rock.Empty,
                    _ => Rock.Null
                };
                controlPanel[colIndex].Add(rockType);
            }
        }
        
        return controlPanel;
    }
    
    private string ControlPanelToString(List<List<Rock>> controlPanel)
    {
        return controlPanel.Aggregate("", (current, rocks) => current + RocksToString(rocks));
    }
    
    private void PrintControlPanel(List<List<Rock>> controlPanel)
    {
        var controlPanelToPrint = TransposeControlPanel(controlPanel);
        controlPanelToPrint.ForEach(rocks =>
        {
            _testOutputHelper.WriteLine(RocksToString(rocks));
        });
    }

    private List<List<Rock>> TransposeControlPanel(List<List<Rock>> controlPanel)
    {
        var result = new List<List<Rock>>();
        for (var rowIndex = 0; rowIndex < controlPanel.Count; rowIndex++)
        {
            var line = controlPanel[rowIndex];
            for (var colIndex = 0; colIndex < line.Count; colIndex++)
            {
                var c = line[colIndex];
                if (result.Count-1 < colIndex)
                {
                    result.Add(new List<Rock>());
                }
                result[colIndex].Add(c);
            }
        }
        return result;
    }

    private string RocksToString(List<Rock> rocks)
    {
        return rocks.Aggregate("", (current, rock) => current + RockToString(rock));
    }

    private char RockToString(Rock r)
    {
        return r switch
        {
            Rock.Round => 'O',
            Rock.Square => '#',
            Rock.Empty => '.',
            _ => '-'
        };
    }
    

    private enum Rock
    {
        Round,
        Square,
        Empty,
        Null
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
}