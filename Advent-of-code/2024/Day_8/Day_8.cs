using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_8;

public class Day_8
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 8;

    public Day_8(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 14)]
    [InlineData(false, 0, 371)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 9)]
    [InlineData(false, 0, 1941)]
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
        var antennas = new List<Node>();
        var antinodes = new Dictionary<string, Node>();
        var antennaTypes = new List<char>();
        var rowMax = input.Count;
        var colMax = input[0].Count;
        for (var rowIndex = 0; rowIndex < rowMax; rowIndex++)
        {
            for (var colIndex = 0; colIndex < colMax; colIndex++)
            {
                var value = input[rowIndex][colIndex];
                if (value == '.') continue;
                if (!antennaTypes.Contains(value))
                {
                    antennaTypes.Add(value);
                }
                var antenna = new Node(rowIndex, colIndex, value);
                antennas.Add(antenna);
            }
        }

        foreach (var antennaType in antennaTypes)
        {
            var curAntennas = antennas.Where(n => n.antennaId == antennaType).ToList();
            for (var i = 0; i < curAntennas.Count; i++)
            {
                var antenna_1 = curAntennas[i];
                for (var j = i+1; j < curAntennas.Count; j++)
                {
                    var antenna_2 = curAntennas[j];
                    FindAntinodeLocations(antenna_1, antenna_2, rowMax, colMax, antinodes, input);
                }
            }
        }
        
        var total = antinodes.Count;
        foreach (var antinode in antinodes)
        {
            WriteLine(antinode.Key);
        }

        return total;
    }

    private void FindAntinodeLocations(Node antenna_1, Node antenna_2, int rowMax, int colMax, Dictionary<string, Node> antinodes, List<List<char>> input)
    {
        var row_1 = antenna_1.rowIndex; // 2
        var col_1 = antenna_1.colIndex; // 5
        var row_2 = antenna_2.rowIndex; // 3
        var col_2 = antenna_2.colIndex; // 7

        var rowDelta = Math.Abs(row_2 - row_1); // 1
        var colDelta = Math.Abs(col_2 - col_1); // 2

        var antinodeRow_1 = CalcAntinodeIndex(row_1, row_2, rowDelta, rowMax);
        var antinodeCol_1 = CalcAntinodeIndex(col_1, col_2, colDelta, colMax);

        try
        {
            var antinode_1 = new Node(antinodeRow_1, antinodeCol_1, input[antinodeRow_1][antinodeCol_1], true);
            antinodes.TryAdd($"{antinodeRow_1}_{antinodeCol_1}", antinode_1);
        }
        catch (Exception)
        {
            // ignored
        }
        
        var antinodeRow_2 = CalcAntinodeIndex(row_2, row_1, rowDelta, rowMax);
        var antinodeCol_2 = CalcAntinodeIndex(col_2, col_1, colDelta, colMax);

        try
        {
            var antinode_2 = new Node(antinodeRow_2, antinodeCol_2, input[antinodeRow_2][antinodeCol_2], true);
            antinodes.TryAdd($"{antinodeRow_2}_{antinodeCol_2}", antinode_2);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private int CalcAntinodeIndex(int antenna_1, int antenna_2, int delta, int max)
    {
        var antinode = 0;
        if (antenna_1 < antenna_2)
        {
            antinode = antenna_1 - delta;
        }
        else
        {
            antinode = antenna_1 + delta;
        }
        return antinode;
    }

    private class Node
    {
        public int rowIndex;
        public int colIndex;
        public char antennaId;
        private char antinodeId;

        public Node(int r, int c, char v, bool node = false)
        {
            rowIndex = r;
            colIndex = c;
            if (!node)
            {
                antennaId = v;
            }
            else
            {
                antinodeId = v;
            }
        }
    }

    private int ParseInput_2(List<List<char>> input)
    {
        var antennas = new List<Node>();
        var antinodes = new Dictionary<string, Node>();
        var antennaTypes = new List<char>();
        var rowMax = input.Count;
        var colMax = input[0].Count;
        for (var rowIndex = 0; rowIndex < rowMax; rowIndex++)
        {
            for (var colIndex = 0; colIndex < colMax; colIndex++)
            {
                var value = input[rowIndex][colIndex];
                if (value == '.') continue;
                if (!antennaTypes.Contains(value))
                {
                    antennaTypes.Add(value);
                }
                var antenna = new Node(rowIndex, colIndex, value);
                antennas.Add(antenna);
            }
        }

        foreach (var antennaType in antennaTypes)
        {
            var curAntennas = antennas.Where(n => n.antennaId == antennaType).ToList();
            for (var i = 0; i < curAntennas.Count; i++)
            {
                var antenna_1 = curAntennas[i];
                for (var j = i+1; j < curAntennas.Count; j++)
                {
                    var antenna_2 = curAntennas[j];
                    FindAntinodeLocations(antenna_1, antenna_2, rowMax, colMax, antinodes, input);
                }
            }
        }
        
        var total = antinodes.Count;
        foreach (var antinode in antinodes)
        {
            WriteLine(antinode.Key);
        }

        return total;
    }

    private void FindAntinodeLocations_2(Node antenna_1, Node antenna_2, int rowMax, int colMax, Dictionary<string, Node> antinodes, List<List<char>> input)
    {
        var row_1 = antenna_1.rowIndex; // 2
        var col_1 = antenna_1.colIndex; // 5
        var row_2 = antenna_2.rowIndex; // 3
        var col_2 = antenna_2.colIndex; // 7

        var rowDelta = Math.Abs(row_2 - row_1); // 1
        var colDelta = Math.Abs(col_2 - col_1); // 2

        var antinodeRow_1 = CalcAntinodeIndex(row_1, row_2, rowDelta, rowMax);
        var antinodeCol_1 = CalcAntinodeIndex(col_1, col_2, colDelta, colMax);

        var continue_1 = true;

        while (continue_1)
        {
            try
            {
                var antinode_1 = new Node(antinodeRow_1, antinodeCol_1, input[antinodeRow_1][antinodeCol_1], true);
                antinodes.TryAdd($"{antinodeRow_1}_{antinodeCol_1}", antinode_1);
            }
            catch (Exception)
            {
                // ignored
            }
            
        }
        
        var antinodeRow_2 = CalcAntinodeIndex(row_2, row_1, rowDelta, rowMax);
        var antinodeCol_2 = CalcAntinodeIndex(col_2, col_1, colDelta, colMax);

        try
        {
            var antinode_2 = new Node(antinodeRow_2, antinodeCol_2, input[antinodeRow_2][antinodeCol_2], true);
            antinodes.TryAdd($"{antinodeRow_2}_{antinodeCol_2}", antinode_2);
        }
        catch (Exception)
        {
            // ignored
        }
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