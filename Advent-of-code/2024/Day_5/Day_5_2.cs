using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_5;

public class Day_5_2
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 5;

    public Day_5_2(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 143)]
    [InlineData(false, 0, 2568)]
    public void Test_1(bool isTest, int testDataCount, int expectedAnswer)
    {
        var (rules, books) = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(rules, books);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 9)]
    [InlineData(false, 0, 1941)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        var (rules, books) = ReadInput(Day, isTest, testDataCount);
        var total = ParseInput(rules, books);
        
        WriteLine();
        WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private List<int> SortRules(List<(int, int)> rules)
    {
        var nodes = new Dictionary<int,Node>();
        foreach (var rule in rules)
        {
            nodes.TryGetValue(rule.Item1, out var first);
            nodes.TryGetValue(rule.Item2, out var second);
            if (first == null)
            {
                first = new Node(rule.Item1);
                nodes.Add(rule.Item1, first);
            }
            if (second == null)
            {
                second = new Node(rule.Item2);
                nodes.Add(rule.Item2, second);
            }
            second.moreThan.Add(first.id);
        }

        foreach (var node in nodes)
        {
            WriteLine($"{node.Key} is more than {string.Join(',',node.Value.moreThan)}");
        }
        
        var smallest = nodes.Values.Where(n => n.moreThan.Count == 0);
        var parsedIds = new List<int>();

        while (smallest.Count() != 0)
        {
            foreach (var small in smallest)
            {
                parsedIds.Add(small.id);
                foreach (var node in nodes)
                {
                    if (node.Value.moreThan.Contains(small.id))
                    {
                        node.Value.weight += small.weight;
                        node.Value.moreThan.Remove(small.id);
                    }
                }
            }
            smallest = nodes.Values.Where(n => !parsedIds.Contains(n.id) && n.moreThan.Count == 0);
        }
        WriteLine();

        foreach (var node in nodes)
        {
            node.Value.moreThan.Sort();
            WriteLine($"{node.Key} is more than {string.Join(',',node.Value.moreThan)} with weight: {node.Value.weight}");
        }
        
        
        
        return new List<int>();
    }

    private class Node
    {
        public List<int> moreThan;
        public int id;
        public long weight = 1;

        public Node(int i)
        {
            this.id = i;
            moreThan = new List<int>();
        }
        
    }
    

    private int ParseInput(List<(int, int)> rules, List<List<int>> books)
    {
        var pageNumberRules = SortRules(rules);
        
        WriteList(pageNumberRules);
        var total = 0;

        foreach (var book in books)
        {
            var highestIndex = -2;
            var isGoodBook = true;
            foreach (var pageNumber in book)
            {
                var i = pageNumberRules.IndexOf(pageNumber);
                if (i == -1)
                {
                    continue;
                }
                if (i > highestIndex)
                {
                    highestIndex = i;
                    continue;
                }
                isGoodBook = false;
                break;
            }

            if (isGoodBook)
            {
                var middlePage = (book.Count - 1) / 2;
                total += book[middlePage];
            }
            
        }
        
        return total;
    }

    private int ParseInput_2()
    {
        var total = 0;
        return total;
    }
    
    private (List<(int, int)>, List<List<int>>) ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var rules = new List<(int, int)>();
        var books = new List<List<int>>();
        
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/2024/Day_{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");

        var firstHalf = true;
        
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            if (line.Length == 0)
            {
                firstHalf = false;
                continue;
            }
            if (firstHalf)
            {
                var newLine = line.Split('|').ToList().ConvertAll(c=>int.Parse($"{c}"));
                rules.Add((newLine[0], newLine[1]));
                continue;
            }

            books.Add(line.Split(',').ToList().ConvertAll(c => int.Parse($"{c}")));

        }
        return (rules, books);
    }
    
    private void WriteLine(int message)
    {
        _testOutputHelper.WriteLine(message.ToString());
    }

    private void WriteList(List<int> values)
    {
        WriteLine(string.Join(',', values));
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}