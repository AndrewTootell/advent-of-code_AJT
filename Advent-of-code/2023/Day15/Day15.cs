using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day15;

public class Day15
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Day15(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 1320)]
    [InlineData(true, 1, 52)]
    [InlineData(false, 0, 502139)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(15, isTest, testDataCount);
        var words = string.Join("",input).Split(',').ToList();
        long total = 0;
        words.ForEach(word =>
        {
           total += Hash(word);
        });
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine("");
    }

    private readonly Dictionary<int, List<Lens>> _boxes = new();
    
    [Theory]
    [InlineData(true, 0, 145)]
    [InlineData(false, 0, 284132)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(15, isTest, testDataCount);
        var words = string.Join("",input).Split(',').ToList();
        var id = 0;
        words.ForEach(word =>
        {
            OrganiseBoxes(id, word);
            id++;
        });
        long total = 0;
        foreach (var key in _boxes.Keys.ToList().Order())
        {
            var box = _boxes[key];
            total += CalcPower(box);
        }
        total.Should().Be(expectedAnswer);
        _testOutputHelper.WriteLine($"total: {total}");
    }

    private void OrganiseBoxes(int id, string command)
    {
        if (command.Contains('='))
        {
            AddOrReplaceLens(command);
            return;
        }
        if (command.Contains('-'))
        {
            RemoveLens(command);
            return;
        }
        _testOutputHelper.WriteLine($"ERROR! id: {id} command: {command}");
    }

    private void AddOrReplaceLens(string command)
    {
        var commandParts = command.Split('=');
        var label = commandParts[0];
        var strength = int.Parse(commandParts[1]);
        var hash = Hash(label);
        if(!_boxes.TryGetValue(hash, out var box))
        {
            box = new List<Lens>();
            _boxes.Add(hash, box);
        }

        var lens = box.SingleOrDefault(lens => lens.Label == label);
        if (lens != null)
        {
            lens.Strength = strength;
            return;
        }
        box.Add(new Lens{Label = label, Box = hash, Strength = strength});
    }

    private void RemoveLens(string command)
    {
        var commandParts = command.Split('-');
        var label = commandParts[0];
        var hash = Hash(label);
        if(!_boxes.TryGetValue(hash, out var box))
        {
            return;
        }

        var lensToRemove = box.SingleOrDefault(l => l.Label == label);
        if (lensToRemove == null)
        {
            return;
        }

        box.Remove(lensToRemove);
    }

    private int CalcPower(List<Lens> box)
    {
        if (box.Count == 0)
        {
            return 0;
        }
        var subTotal = 0;
        var boxHash = box.First().Box;
        for (var lensIndex = 0; lensIndex < box.Count; lensIndex++)
        {
            var lens = box[lensIndex];
            var focalPower = lens.Strength;
            subTotal += (boxHash+1) * (lensIndex+1) * focalPower;
        }
        
        return subTotal;
    }

    private int Hash(string word)
    {
        List<int> wordAscii = new();
        word.ToList().ForEach(c =>
        {
            wordAscii.Add(c);
        });
        var currentValue = 0;
        
        wordAscii.ForEach(c =>
        {
            currentValue += c;
            currentValue *= 17;
            currentValue %= 256;
        });
        return currentValue;
    }

    private class Lens
    {
        public string Label;
        public int Box;
        public int Strength;
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