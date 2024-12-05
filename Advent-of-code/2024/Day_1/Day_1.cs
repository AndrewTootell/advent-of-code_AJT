using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_1;

public class Day_1
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 1;

    public Day_1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 11)]
    [InlineData(false, 0, 2166959)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 31)]
    [InlineData(false, 0, 23741109)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput_2(input);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private int ParseInput(List<List<int>> input)
    {
        var total = 0;

        for (var index = 0; index < input[0].Count; index++)
        {
            total += Math.Abs(input[0][index] - input[1][index]);
        }
        
        return total;
    }

    private int ParseInput_2(List<List<int>> input)
    {
        var total = 0;
        var cacheNumber = 0;
        var indexRight = 0;
        var countDict = new Dictionary<int, int>();
        var subTotal = 0;

        for (var indexLeft = 0; indexLeft < input[0].Count; indexLeft++)
        {
            if (input[0][indexLeft] == cacheNumber)
            {
                countDict.TryGetValue(cacheNumber, out int cachedSubTotal);
                total += cachedSubTotal;
                continue;
            }

            cacheNumber = input[0][indexLeft];
            var count = 0;
            while (input[1][indexRight] < cacheNumber)
            {
                indexRight += 1;
            }
            
            while (input[1][indexRight] == cacheNumber)
            {
                count++;
                indexRight += 1;
            }

            subTotal = cacheNumber * count;
            total += subTotal;
            countDict.Add(cacheNumber, subTotal);
            WriteLine($"{cacheNumber} * {count}");
            WriteLine(total);
        }
        
        return total;
    }
    
    private List<List<int>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<int>>();
        input.Add(new List<int>());
        input.Add(new List<int>());
        
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day_{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            var values = line.Split("   ");
            if (values.Length == 2)
            {
                input[0].Add(int.Parse(values[0]));
                input[1].Add(int.Parse(values[1]));
            }
        }
        input[0].Sort();
        input[1].Sort();
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