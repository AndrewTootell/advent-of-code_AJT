using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit.Abstractions;

namespace Advent_of_code.Day12;

public class Day12
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Day12(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 21)]
    [InlineData(false, 0, 7344)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        long total = 0;
        
        data.ForEach(springRow =>
        {
            var subTotal = RunLogic(springRow.Id, springRow.Row, springRow.Groups);
            total += subTotal;
        });
        _testOutputHelper.WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0,  0)]
    [InlineData(false, 0,  0)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        data = UnFold(data);
        long total = 0;
        
        data.ForEach(springRow =>
        {
            var subTotal = RunLogic(springRow.Id, springRow.Row, springRow.Groups);
            total += subTotal;
            WriteLine($"Id: {springRow.Id} subTotal: {subTotal} springRow: {springRow.Row} groups: {string.Join(",",springRow.Groups)}");
            //WriteLine("");
        });
        _testOutputHelper.WriteLine($"Total: {total} < 7427");
        //total.Should().Be(expectedAnswer);
    }

    private void WriteLine(string message)
    {
        //_testOutputHelper.WriteLine(message);
    }

    private int RunLogic(int id, string springRow, List<int> groups)
    {
        var max = groups.Count;
        var split = springRow.Split('.').ToList();
        
        var count = 0;
        var numBase = groups.Count + 1;
        for (var i = (int)Math.Pow(groups.Count + 1, split.Count)-1; i >= 0; i--)
        {
            var toTry = new List<int>();
            long ttCount = 0;
            for (var j = split.Count-1; j >= 0; j--)
            {
                var baseForIndex = Convert.ToInt64(Math.Pow(numBase, j));
                if (j == 0)
                {
                    toTry = toTry.Append((int)(i-ttCount)).ToList();
                    break;
                }
                toTry = toTry.Append((int)((i-ttCount)/baseForIndex)).ToList();
                ttCount += toTry.Last() * baseForIndex;
            }
            if (toTry.Sum() == max)
            {
                count += TryVariations(id, toTry, split, groups);
            }
        }

        return count;
    }
    
    private Dictionary<(string, List<int>),int> cache = new();
    
    private int TryVariations(int id, List<int> toTry, List<string> split, List<int> groups)
    {
        var subCount = 1;
        var previousGroupsSum = 0;
        for (var i = 0; i < toTry.Count; i++)
        {
            var groupSize = toTry[i];
            int subSubCount;
            var s = split[i];
            var g = groups.Skip(previousGroupsSum).Take(groupSize).ToList();
            if (cache.TryGetValue((s, g), out var value))
            {
                subSubCount = value;
            }
            else
            {
                _testOutputHelper.WriteLine(".");
                subSubCount = FindPossibilities(id, s, g);
                cache.Add((s, g), subSubCount);
            }
            
            if (subSubCount != 0)
            {
                WriteLine($"Id: {id} subSubCount: {subSubCount} springRow: {split[i]} groups: {string.Join(",",groups.Skip(previousGroupsSum).Take(groupSize).ToList())}");
                WriteLine("");
            }
            subCount *= subSubCount;
            previousGroupsSum += groupSize;
        }
        //WriteLine($"Id: {id} subCount: {subCount} springRow: {string.Join(".",split)} groups: {string.Join(",",groups)} toTry: {string.Join(",",toTry)}");
        //WriteLine("");
        return subCount;
    }

    private int FindPossibilities(int id, string springRow, List<int> groups)
    {
        if (HasNoPossibilities(id, springRow, groups))
        {
            return 0;
        }
        if (HasOnePossibility(id, springRow, groups))
        {
            return 1;
        }
        return BruteForce(id, springRow, groups);
    }
    
    private bool HasNoPossibilities(int id, string springRow, List<int> groups)
    {
        if (springRow.Length < groups.Sum() + groups.Count - 1)
        {
            return true;
        }

        if (springRow.Count(c => c == '#') > groups.Sum())
        {
            return true;
        }
        return false;
    }
    
    private bool HasOnePossibility(int id,string springRow, List<int> groups)
    {
        if (springRow.Length == 0 || groups.Count == 0)
        {
            WriteLine("Spring or groups are 0");
            return true;
        }

        if (!springRow.Contains('?'))
        {
            WriteLine("no unknowns");
            return true;
        }
        
        return false;
    }

    private int BruteForce(int id, string springRow, List<int> groups)
    {
        var lines = RecursiveMakeLines(springRow, groups, new List<int>(Enumerable.Repeat(1, groups.Count-1)), groups.Count-1, 0);
        var score = lines.Count(line => CanLineMatch(id, line, springRow.ToList()));
        return score;
    }

    private List<List<char>> RecursiveMakeLines(string springRow, List<int> groups, List<int> separators, int maxDepth, int depth)
    {
        if (depth == maxDepth)
        {
            var newLines = new List<List<char>>();
            var pattern = CreatePattern(groups, separators);
            var count = 0;
            while (springRow.Length - pattern.Count-count >= 0)
            {
                var line = pattern.Concat(Enumerable.Repeat('.', springRow.Length - pattern.Count-count)).ToList();
                line = Enumerable.Repeat('.', count).Concat(line).ToList();
                newLines.Add(line);
                count++;
            }
            return newLines;
        }

        var lines = new List<List<char>>();
        while (groups.Sum() + separators.Sum() < springRow.Length)
        {
            lines = lines.Concat(RecursiveMakeLines(springRow, groups, separators.ToList(), maxDepth, depth + 1)).ToList();
            separators[^(maxDepth-depth)] += 1;
        }
        lines = lines.Concat(RecursiveMakeLines(springRow, groups, separators.ToList(), maxDepth, depth + 1)).ToList();
        
        return lines;
    }

    private List<char> CreatePattern(List<int> groups, List<int> seperators)
    {
        var line = new List<char>();
        for (var i = 0; i < seperators.Count; i++)
        {
            line = line.Concat(Enumerable.Repeat('#', groups[i])).ToList();
            line = line.Concat(Enumerable.Repeat('.', seperators[i])).ToList();
        }
        return line.Concat(Enumerable.Repeat('#', groups.Last())).ToList();
    }
    
    private bool CanLineMatch(int id, List<char> line, List<char> pattern)
    {
        for (var i = 0; i < line.Count; i++)
        {
            if (line[i] == '.' && pattern[i] == '#')
            {
                return false;
            }
        }
        WriteLine($"id: {id} line: {string.Join("",line)} pattern: {string.Join("",pattern)}");
        return true;
    }
    
    private class SpringRow
    {
        public int Id;
        public string Row;
        public List<int> Groups;
    }

    /*
     * To unfold the records,
     * on each row,
     * replace the list of spring conditions with five copies of itself (separated by ?) and
     * replace the list of contiguous groups of damaged springs with five copies of itself (separated by ,).
     */
    private List<SpringRow> UnFold(List<SpringRow> data)
    {
        foreach (var springRow in data)
        {
            springRow.Row = springRow.Row +"?"+ springRow.Row +"?"+ springRow.Row +"?"+ springRow.Row +"?"+ springRow.Row;
            springRow.Groups = springRow.Groups.Concat(springRow.Groups).Concat(springRow.Groups).Concat(springRow.Groups).Concat(springRow.Groups).ToList();
        }

        return data;
    }

    private List<SpringRow> ParseData(List<string> input)
    {
        var data = new List<SpringRow>();
        var id = 0;
        input.ForEach(line =>
        {
            var lineSplit = line.Split(' ');
            var row = lineSplit[0] = lineSplit[0].TrimStart('.').TrimEnd('.');
            
            while (row.Contains(".."))
            {
                row = row.Replace("..", ".");
            }
            data.Add(new SpringRow
            {
                Id = id,
                Row = row,
                Groups = lineSplit[1].Split(',').ToList().ConvertAll(int.Parse)
            });
            id += 1;
        });
        
        return data;
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