using FluentAssertions;

namespace Advent_of_code.Day11;

public class Day11
{
    [Theory]
    [InlineData(true, 0, 374)]
    [InlineData(false, 0, 9686930)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var data = ReadInput(11, isTest, testDataCount);
        var galaxies = ParseData(data, 2);
        long total = 0;
        for (var i = 0; i < galaxies.Count; i++)
        {
            var g1 = galaxies[i];
            long subTotal = 0;
            for (var j = i; j < galaxies.Count; j++)
            {
                subTotal += g1.CalcDifference(galaxies[j]);
            }
            total += subTotal;
        }
        
        total.Should().Be(expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0, 2, 374)]
    [InlineData(true, 0, 10, 1030)]
    [InlineData(true, 0, 100, 8410)]
    [InlineData(false, 0, 1000000, 630728425490)]
    public void Test_2(bool isTest, int testDataCount, int spaceIncrease, long expectedAnswer)
    {
        var data = ReadInput(11, isTest, testDataCount);
        var galaxies = ParseData(data, spaceIncrease);
        long total = 0;
        for (var i = 0; i < galaxies.Count; i++)
        {
            var g1 = galaxies[i];
            long subTotal = 0;
            for (var j = i; j < galaxies.Count; j++)
            {
                subTotal += g1.CalcDifference(galaxies[j]);
            }
            total += subTotal;
        }
        
        total.Should().Be(expectedAnswer);
    }

    private class CoOrds
    {
        public long Column;
        public long Row;

        public long CalcDifference(CoOrds other)
        {
            return Math.Abs(other.Row - Row) + Math.Abs(other.Column - Column);
        }
    }

    private List<CoOrds> ParseData(List<List<char>> data, int spaceIncrease)
    {
        var galaxies = new List<CoOrds>();
        var columnsToSkip = new List<int>();
        for (var colIndex = 0; colIndex < data.First().Count; colIndex++)
        {
            if (data.All(line => line[colIndex] != '#'))
            {
                columnsToSkip.Add(colIndex);
            }
        }

        long rowIndexPlusEmpties = 0;
        foreach (var line in data)
        {
            if (!line.Contains('#'))
            {
                rowIndexPlusEmpties += spaceIncrease;
                continue;
            }
            long colIndexPlusEmpties = 0;
            for (var colIndex = 0; colIndex < line.Count; colIndex++)
            {
                if (columnsToSkip.Contains(colIndex))
                {
                    colIndexPlusEmpties += spaceIncrease;
                    continue;
                }
                if (line[colIndex] == '#')
                {
                    galaxies.Add(
                        new CoOrds { Row = rowIndexPlusEmpties, Column = colIndexPlusEmpties }
                    );
                }

                colIndexPlusEmpties += 1;
            }

            rowIndexPlusEmpties += 1;
        }

        return galaxies;
    }
    
    private List<List<char>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var data = new List<List<char>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            data.Add(line.ToList());
        }

        return data;
    }
}