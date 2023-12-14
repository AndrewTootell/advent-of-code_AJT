using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using Xunit.Abstractions;

namespace Advent_of_code.Day13;

public class Day13
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Day13(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 405)]
    [InlineData(false, 0, 30802)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(13, isTest, testDataCount);
        var rockGardens = ParseData(input);
        long total = 0;
        
        rockGardens.ForEach(rockGarden =>
        {
            total += FindTopOrLeftDistance(rockGarden);
        });
        
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine("");
    }

    private int FindTopOrLeftDistance(RockGarden rockGarden)
    {
        for (var rowIndex = 1; rowIndex < rockGarden.HorizontalMap.Count; rowIndex++)
        {
            var previousLine = rockGarden.HorizontalMap[rowIndex-1];
            var thisLine = rockGarden.HorizontalMap[rowIndex];
            if (previousLine.Equals(thisLine) && CheckAllLinesMatch(rockGarden.HorizontalMap, rowIndex))
            {
                return rowIndex * 100;
            }
        }
        for (var colIndex = 1; colIndex < rockGarden.VerticalMap!.Count; colIndex++)
        {
            var previousLine = rockGarden.VerticalMap[colIndex-1];
            var thisLine = rockGarden.VerticalMap[colIndex];
            if (previousLine.Equals(thisLine) && CheckAllLinesMatch(rockGarden.VerticalMap, colIndex))
            {
                return colIndex;
            }
        }
        
        _testOutputHelper.WriteLine($"ERROR!  id: {rockGarden.Id}");
        return 0;
    }

    private bool CheckAllLinesMatch(List<string> rockGardenMap, int index)
    {
        var deltaIndex = index-1;
        
        while (index < rockGardenMap.Count && deltaIndex >= 0)
        {
            if (!rockGardenMap[index].Equals(rockGardenMap[deltaIndex]))
            {
                return false;
            }
            index++;
            deltaIndex--;
        }

        return true;
    }
    
    [Theory]
    [InlineData(true, 0, 400)]
    [InlineData(false, 0, 37876)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(13, isTest, testDataCount);
        var rockGardens = ParseData(input);
        long total = 0;
        
        rockGardens.ForEach(rockGarden =>
        {
            total += FindTopOrLeftDistanceOffByOne(rockGarden);
        });
        
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine("");
    }
    
    private int FindTopOrLeftDistanceOffByOne(RockGarden rockGarden)
    {
        for (var rowIndex = 1; rowIndex < rockGarden.HorizontalMap.Count; rowIndex++)
        {
            var previousLine = rockGarden.HorizontalMap[rowIndex-1];
            var thisLine = rockGarden.HorizontalMap[rowIndex];
            (var isEqual, _) = EqualButOne(previousLine, thisLine, false);
            if (isEqual && CheckAllLinesMatchOffByOne(rockGarden.HorizontalMap, rowIndex, false))
            {
                return rowIndex * 100;
            }
        }
        for (var colIndex = 1; colIndex < rockGarden.VerticalMap!.Count; colIndex++)
        {
            var previousLine = rockGarden.VerticalMap[colIndex-1];
            var thisLine = rockGarden.VerticalMap[colIndex];
            (var isEqual, _) = EqualButOne(previousLine, thisLine, false);
            // first line is checked again so if set offByOneUsed will fail when that line is rechecked
            if (isEqual && CheckAllLinesMatchOffByOne(rockGarden.VerticalMap, colIndex, false))
            {
                return colIndex;
            }
        }
        
        _testOutputHelper.WriteLine($"ERROR!  id: {rockGarden.Id}");
        return 0;
    }

    private bool CheckAllLinesMatchOffByOne(List<string> rockGardenMap, int index, bool offByOneUsed)
    {
        var deltaIndex = index-1;
        
        while (index < rockGardenMap.Count && deltaIndex >= 0)
        {
            (var isEqual, offByOneUsed) = EqualButOne(rockGardenMap[index], rockGardenMap[deltaIndex], offByOneUsed);
            if (!isEqual)
            {
                return false;
            }
            index++;
            deltaIndex--;
        }

        return offByOneUsed;
    }

    private (bool,bool) EqualButOne(string line, string otherLine, bool offByOneUsed)
    {
        if (line.Equals(otherLine))
        {
            return (true, offByOneUsed);
        }
        var diffCount = line.Where((t, i) => t != otherLine[i]).Count();
        if (diffCount == 1 && !offByOneUsed)
        {
            return (true, true);
        }
        return (false,false);
    }
    
    
    private class RockGarden
    {
        public int Id;
        public int Mirror;
        public List<string> HorizontalMap = new();
        public List<string>? VerticalMap = new();
    }
    
    private List<RockGarden> ParseData(List<string> input)
    {
        var id = 0;
        var rockGardens = new List<RockGarden>();
        var currentRockGarden = new RockGarden
        {
            Id = id,
            HorizontalMap = new List<string>(),
            VerticalMap = new List<string>( new string[input.First().Length] )
        };
        rockGardens.Add(currentRockGarden);
        input.ForEach(line =>
        {
            if (string.IsNullOrEmpty(line))
            {
                id += 1;
                currentRockGarden = new RockGarden
                {
                    Id = id,
                    HorizontalMap = new List<string>(),
                    VerticalMap = null
                };
                rockGardens.Add(currentRockGarden);
                return;
            }
            currentRockGarden.HorizontalMap.Add(line);
            if (currentRockGarden.VerticalMap is null)
            {
                currentRockGarden.VerticalMap = new List<string>( new string[line.Length] );
            }
            
            for (var i = 0; i < line.Length; i++)
            {
                currentRockGarden.VerticalMap[i] += line[i];
            }
        });
        
        return rockGardens;
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