using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code._2024.Day_5;

public class Day_5
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 5;

    public Day_5(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 143)]
    [InlineData(false, 0, 2532)]
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
    
    private static Dictionary<Guid, Node> nodes = new();

    private int ParseInput(List<(int, int)> rules, List<List<int>> books)
    {
        

        var curId = OrderRules(rules);
        if (curId == null)
        {
            WriteLine("ERROR too many graphs!");
        }
        var ruleOrder = new List<int>();

        while (curId != null)
        {
            nodes.TryGetValue(curId.Value, out var node);
            ruleOrder.Add(node.value);
            if (node.biggerValues.Count > 1)
            {
                WriteLine("ERROR too many children!");
            }
            if (node.biggerValues.Count == 1)
            {
                curId = node.biggerValues[0];
            }
            else
            {
                curId = null;
            }
        }

        var total = 0;

        foreach (var book in books)
        {
            var highestIndex = -2;
            var isGoodBook = true;
            foreach (var pageNumber in book)
            {
                var i = ruleOrder.IndexOf(pageNumber);
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

    private Guid? OrderRules(List<(int, int)> rules)
    {
        var startNodes = new List<Guid>();

        foreach (var rule in rules)
        {
            Node? smaller = null;
            Node? bigger = null;
            foreach (var startId in startNodes)
            {
                nodes.TryGetValue(startId, out var startNode);
                if (startNode == null)
                {
                    continue;
                }
                var foundSmaller = startNode.GetByValue(rule.Item1);
                if (foundSmaller != null)
                {
                    nodes.TryGetValue(foundSmaller.Value, out smaller);
                }
                var foundBigger = startNode.GetByValue(rule.Item2);
                if (foundBigger != null)
                {
                    nodes.TryGetValue(foundBigger.Value, out bigger);
                }
            }

            if (smaller == null)
            {
                smaller = new Node(rule.Item1);
                nodes.Add(smaller.id, smaller);
                startNodes.Add(smaller.id);
            }
            
            if (bigger == null)
            {
                bigger = new Node(rule.Item2, smaller.id);
                nodes.Add(bigger.id, bigger);
            }
            else
            {
                var startNodesClone = new List<Guid>(startNodes);
                foreach (var startId in startNodes)
                {
                    nodes.TryGetValue(startId, out var startNode);
                    if (startNode.value == bigger.value)
                    {
                        startNodesClone.Remove(startNode.id);
                    }
                }
                startNodes = startNodesClone;
            }

            if (smaller.GetByValue(rule.Item2) == null)
            {
                bigger.parents.Add(smaller.id);
                smaller.biggerValues.RemoveAll(childId =>
                    {
                        nodes.TryGetValue(childId, out var child);
                        var has = bigger.GetByValue(child.value);
                        return has != null;
                    }
                );
                smaller.biggerValues.Add(bigger.id);
            }

            RemoveSibling(smaller.id, bigger.id);
            
        }

        if (startNodes.Count != 1)
        {
            return null;
        }

        return startNodes[0];
    }

    private void RemoveSibling(Guid smallerId, Guid biggerId,
        int depth = 0)
    {
        if (depth > 50)
        {
            return;
        }
        nodes.TryGetValue(smallerId, out var smaller);
        nodes.TryGetValue(biggerId, out var bigger);
        if (smaller.parents.Count == 0)
        {
            return;
        }
        foreach (var parentId in smaller.parents)
        {
            nodes.TryGetValue(parentId, out var parent);
            parent.biggerValues.RemoveAll(nId =>
            {
                nodes.TryGetValue(nId, out var n);
                return n.value == bigger.value;
            });
            nodes[parentId] = parent;
            RemoveSibling(parent.id, bigger.id, depth + 1);
        }

        return;

    }

    private class Node
    {
        public Guid id;
        public int value;
        public List<Guid> biggerValues;
        public List<Guid> parents;

        public Node(int val, Guid? parent = null)
        {
            id = Guid.NewGuid();
            value = val;
            biggerValues = new List<Guid>();

            parents = new List<Guid>();
            if (parent != null)
            {
                parents.Add(parent.Value);
            }
        }
        
        public Guid? GetByValue(int find)
        {
            if (value == find)
            {
                return this.id;
            }

            foreach (var nodeId in biggerValues)
            {
                nodes.TryGetValue(nodeId, out var node);
                var found = node.GetByValue(find);
                if (found != null)
                {
                    return found;
                }
            }
            
            return null;
        }
        public new string ToString(int depth = 0)
        {
            var ret = $"{new string(' ', depth * 2)}{value}\n";
            foreach (var biggerId in biggerValues)
            {
                nodes.TryGetValue(biggerId, out var bigger);
                ret += bigger.ToString(depth + 1);
            }

            return ret;
        }
        
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