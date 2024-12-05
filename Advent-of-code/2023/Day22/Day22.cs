using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day22;

public class Day22
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 22;

    public Day22(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 5)]
    [InlineData(false, 0, 489)] // too low!
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var data = ParseInput(input);
        int total = 0;
        
        total = RunLogic(data);
        
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

    private int RunLogic(List<Block> data)
    {
        Dictionary<int, List<Block>> tower = new() { { 0, new List<Block>() } };
        Dictionary<string, Block> required = new();
        foreach (var block in data)
        {
            var layer = tower.MaxBy(keyPair => keyPair.Key);
            var height = layer.Key;
            var blocks = layer.Value;
            while (!BlockTouchesBlocks(block, blocks) && height > 0)
            {
                height--;
                blocks = tower[height];
            }
            var layerKey = height;
            var blockHeightCount = 0;
            while (blockHeightCount < block.End.Height-block.Start.Height + 1)
            {
                layerKey++;
                blockHeightCount++;
                tower.TryAdd(layerKey, new List<Block>());
                tower[layerKey].Add(block);
            }

            if (block.SupportedBy.Count == 1)
            {
                var req = block.SupportedBy.First();
                required.TryAdd(req.Key, req);
            }
        }
        foreach (var keyValuePair in tower.Reverse())
        {
            var line = "[";
            foreach (var block in keyValuePair.Value)
            {
                line += block.Key + ",";
            }

            line += "]";
            WriteLine(line);
        }

        var requiredPrint = "";
        foreach (var keyValuePair in required)
        {
            requiredPrint += keyValuePair.Key + ", ";
        }
        WriteLine(requiredPrint);


        return data.Count - required.Count;
    }
    private bool BlockTouchesBlocks(Block block, List<Block> blocks)
    {
        var touches = false;
        foreach (var block1 in blocks)
        {
            if (block1.Start.Row <= block.End.Row && block1.End.Row >= block.Start.Row &&
                block1.Start.Col <= block.End.Col && block1.End.Col >= block.Start.Col)
            {
                touches = true;
                block.SupportedBy.Add(block1);
                block1.Supports.Add(block);
            }
        }

        return touches;
    }

    private class Block
    {
        public string Key;
        public Position Start;
        public Position End;

        public List<Block> SupportedBy;
        public List<Block> Supports;

        public Block(string input, int count)
        {
            Key = count.ToString();
            // 1,0,1~1,2,1
            var parts = input.Split('~');
            Start = new Position(parts[0]);
            End = new Position(parts[1]);

            SupportedBy = new List<Block>();
            Supports = new List<Block>();
        }
    }

    private class Position
    {
        public readonly int Row;
        public readonly int Col;
        public int Height;
        
        public string Key => $"{Row:00}_{Col:00}_{Height:00}";

        public Position(string input)
        {
            // 1,0,1
            var parts = input.Split(',').ToList().ConvertAll(int.Parse);
            Row = parts[0];
            Col = parts[1];
            Height = parts[2];
        }

        public Position(int row, int col, int height)
        {
            Row = row;
            Col = col;
            Height = height;
        }
    }

    private List<Block> ParseInput(List<string> input)
    {
        var data = new List<Block>();
        var count = 0;
        foreach (var s in input)
        {
            data.Add(new Block(s, count));
            count++;
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
            input.Add(sr.ReadLine()!);
        }
        return input;
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}