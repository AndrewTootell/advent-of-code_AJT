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
    [InlineData(true, 1, 21)]
    [InlineData(false, 0, 1000)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        long total = 0;
        
        data.ForEach(springRow =>
        {
            var subTotal = RunLogic(springRow.Id, springRow.Row, springRow.Groups);
            total += subTotal;
            //_testOutputHelper.WriteLine($"ID: {springRow.Id} subTotal: {subTotal} for {springRow.Row} {string.Join(",",springRow.Groups)}");
        });
        _testOutputHelper.WriteLine("");
        _testOutputHelper.WriteLine($"Total: {total}");
        //total.Should().Be(expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0,  0)]
    [InlineData(false, 0,  0)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(11, isTest, testDataCount);
        var data = ParseData(input);
        long total = 0;
        
        total.Should().Be(expectedAnswer);
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
            var ttCount = 0;
            for (var j = split.Count-1; j >= 0; j--)
            {
                var baseForIndex = Convert.ToInt32(Math.Pow(numBase, j));
                if (j == 0)
                {
                    toTry = toTry.Append(i-ttCount).ToList();
                    break;
                }
                toTry = toTry.Append((i-ttCount)/baseForIndex).ToList();
                ttCount += toTry.Last() * baseForIndex;
            }
            if (toTry.Sum() == max)
            {
                count += TryVariations(id, toTry, split, groups);
            }
        }

        return count;
    }

    private int TryVariations(int id, List<int> toTry, List<string> split, List<int> groups)
    {
        var subCount = 1;
        var previousGroupsSum = 0;
        for (var i = 0; i < toTry.Count; i++)
        {
            var groupSize = toTry[i];
            if (groupSize == 0)
            {
                continue;
            }

            var subSubCount = FindPossibilities(id, split[i], groups.Skip(previousGroupsSum).Take(groupSize).ToList());
            subCount *= subSubCount;
            //_testOutputHelper.WriteLine($"ID: {id} subSubCount:{subSubCount} Count: {subCount} for {split[i]} {string.Join(",",groups.Skip(previousGroupsSum).Take(groupSize).ToList())}");
            previousGroupsSum += groupSize;
        }
        //_testOutputHelper.WriteLine("");
        
        return subCount;
    }

    private int FindPossibilities(int id, string springRow, List<int> groups)
    {
        (springRow, groups) = RemoveWhereMaxHasOnePossibility(springRow, groups);
        if (!springRow.All(c => c is '#' or '?'))
        {
            _testOutputHelper.WriteLine($"ERROR!  ID: {id} for {springRow} {string.Join(',', groups)}");
        }
        if (HasNoPossibilities(springRow, groups))
        {
            return 0;
        }
        
        if (HasOnePossibility(springRow, groups))
        {
            return 1;
        }

        if (!springRow.Contains('#') && groups.Count <= 3)//5)
        {
            return AllUnknown(springRow, groups);
        }

        if (groups.Count == 1)
        {
            return SingleGroup(springRow, groups);
        }
        
        (springRow, groups) = RemoveWhereFirstStringMustBeFirstGroupOrLastStringMustBeLastGroup(springRow, groups);
        if (!springRow.All(c => c is '#' or '?'))
        {
            _testOutputHelper.WriteLine($"ERROR!  ID: {id} for {springRow} {string.Join(',', groups)}");
        }
        if (HasNoPossibilities(springRow, groups))
        {
            return 0;
        }
        
        if (HasOnePossibility(springRow, groups))
        {
            return 1;
        }

        if (!springRow.Contains('#') && groups.Count <= 3)//5)
        {
            return AllUnknown(springRow, groups);
        }

        if (!springRow.Contains('#'))
        {
            return 0;
        }

        if (groups.Count == 1)
        {
            return SingleGroup(springRow, groups);
        }

        if (groups.Count <= 3)
        {
            return BruteForce(springRow, groups);
        }
        
        
        _testOutputHelper.WriteLine($"UNSOLVED! ID: {id} for {springRow} {string.Join(',', groups)}");
        return 0;
    }

    private bool HasNoPossibilities(string springRow, List<int> groups)
    {
        var springRowToShort = springRow.Length < groups.Sum() + groups.Count - 1;
        var knownIdMoreThanSum = springRow.Count(c => c == '#') > groups.Sum();
        var longestHashtag = GroupSpringRowByChar(springRow).Where(s => s.Contains('#')).MaxBy(s => s.Length);
        var longestKnownIsLongerThanMax = (longestHashtag != null && groups.Count > 0 && longestHashtag.Length > groups.Max());
        return springRowToShort || knownIdMoreThanSum || longestKnownIsLongerThanMax;
    }
    
    private bool HasOnePossibility(string springRow, List<int> groups)
    {
        return springRow.Length == 0 || groups.Count == 0 || 
               !springRow.Contains('?') || 
               springRow.Length == groups.Sum() + groups.Count - 1;
    }
    
    private (string, List<int>) RemoveWhereMaxHasOnePossibility(string springRow, List<int> groupsParsed)
    {
        var sections = GroupSpringRowByChar(springRow);
        while (groupsParsed.Count > 0 && sections.Count(s => s.Contains('#') && s.Length == groupsParsed.Max()) == 1)
        {
            var index = sections.FindIndex(s => s.Contains('#') && s.Length == groupsParsed.Max());
            sections.RemoveAt(index);
            groupsParsed.Remove(groupsParsed.Max());
            if (index-1 >= 0)
            {
                sections[index - 1] = string.Join("",sections[index - 1].Skip(1));
                if (string.IsNullOrEmpty(sections[index - 1]))
                {
                    sections.RemoveAt(index-1);
                    index -= 1;
                }
            }
            if (index < sections.Count)
            {
                sections[index] = string.Join("",sections[index].Skip(1));
                if (string.IsNullOrEmpty(sections[index]))
                {
                    sections.RemoveAt(index);
                }
            }
            
            
        }

        return (string.Join("",sections), groupsParsed);
    }
    
    private List<string> GroupSpringRowByChar(string springRowParsedOnce)
    {
        var springRowGroupedByChar = new List<string>();
        var previousChar = ' ';
        var currentString = "";
        foreach (var c in springRowParsedOnce)
        {
            if (c == previousChar)
            {
                currentString += c;
                continue;
            }

            if (previousChar != ' ')
            {
                springRowGroupedByChar.Add(currentString);
            }
            previousChar = c;
            currentString = $"{c}";
        }
        springRowGroupedByChar.Add(currentString);

        return springRowGroupedByChar;
    }

    private int AllUnknown(string springRow, List<int> groupsParsed)
    {
        if (groupsParsed.Count == 1)
        {
            return springRow.Length + 1 - groupsParsed.First();
        }
        
        var minPatternSize = (groupsParsed.Sum() + groupsParsed.Count)-1;
        var triangleHeight = (springRow.Length - minPatternSize) + 1;
        
        if (groupsParsed.Count == 2)
        {
            return TriangleSize(triangleHeight);
        }

        var triangleTotal = 0;

        for (var i = 1; i <= triangleHeight; i++)
        {
            triangleTotal += TriangleSize(triangleHeight);
        }
        
        return triangleTotal;
    }

    private int TriangleSize(int triangleHeight)
    {
        return triangleHeight * (triangleHeight+1)/2;
    }
    
    private int SingleGroup(string springRow, List<int> groups)
    {
        var sections = GroupSpringRowByChar(springRow);
        var min = groups.Min();
        var firstSet = sections.FindIndex(s => s.Contains('#'));
        var lastSet = sections.FindLastIndex(s => s.Contains('#'));

        if (firstSet != lastSet)
        {
            // ??????#?#??????
            min -= sections.Skip(firstSet).Take(lastSet - firstSet).ToList().Sum(s=>s.Length);
        }
        
        if (firstSet > 0)
        {
            // ???# => 4
            min = sections[firstSet - 1].Length+1 < min
                ? sections[firstSet - 1].Length+1
                : min;
        }
        if (lastSet < sections.Count-1)
        {
            // #?? => 3
            min = sections[lastSet + 1].Length+1 < min
                ? sections[lastSet + 1].Length+1
                : min;
        }

        return min;
    }

    private int BruteForce(string springRow, List<int> groups)
    {
        var max = AllUnknown(springRow, groups);
        //????#?? 1,4
        return max;
    }
    
    private (string, List<int>) RemoveWhereFirstStringMustBeFirstGroupOrLastStringMustBeLastGroup(
        string springRow, List<int> groups)
    {
        var sections = GroupSpringRowByChar(springRow);
        while (sections.Count > 0 &&
               groups.Count > 0)
        {
            sections = GroupSpringRowByChar(string.Join("",sections));
            
            if (string.IsNullOrEmpty(sections.First()))
            {
                sections = sections.Skip(1).ToList();
                continue;
            }
            
            if (string.IsNullOrEmpty(sections.Last()))
            {
                sections = sections.Take(sections.Count-1).ToList();
                continue;
            }

            if (sections.First().Contains('#'))
            {
                (sections, groups) = RemoveStartingHashtags(sections, groups);
                continue;
            }

            if (sections.Last().Contains('#'))
            {
                (sections, groups) = RemoveEndingHashtags(sections, groups);
                continue;
            }
            break;
        }

        return (string.Join("",sections), groups);
    }

    private (List<string>, List<int>) RemoveStartingHashtags(
        List<string> sections, List<int> groups)
    {
        // remove group
        var firstStringGroup = sections.First();
        var firstGroup = groups.First();
        sections = sections.Skip(1).ToList();
        groups = groups.Skip(1).ToList();
        if (firstStringGroup.Length == firstGroup)
        {
            // reduce next group by 1
            sections[0] = string.Join("",sections.First().Skip(1));
            return (sections,groups);
        }

        // first group must be more than first line and next line must be ?'s
        // +1 to the group for the .
        var howManyMoreToRemove = firstGroup + 1 - firstStringGroup.Length;
        while (howManyMoreToRemove > 0 && sections.Count > 0)
        {
            // reduce line by 1
            var newFirst = string.Join("",sections.First().Skip(1));
            if (!string.IsNullOrEmpty(newFirst))
            {
                sections[0] = newFirst;
            }
            else
            {
                sections = sections.Skip(1).ToList();
            }

            howManyMoreToRemove--;
        }

        return (sections, groups);
    }

    private (List<string>, List<int>) RemoveEndingHashtags(
        List<string> sections, List<int> groups)
    {
        // remove group
        var lastStringGroup = sections.Last();
        var lastGroup = groups.Last();
        sections = sections.Take(sections.Count-1).ToList();
        groups = groups.Take(groups.Count-1).ToList();
        if (lastStringGroup.Length == lastGroup)
        {
            // reduce previous group by 1
            sections[^1] = string.Join("",sections.Last().Skip(1));
            return (sections, groups);
        }
                
        // last group must be more than last line and previous line must be ?'s
        // +1 to the group for the .
        var howManyMoreToRemove = lastGroup + 1 - lastStringGroup.Length;
        while (howManyMoreToRemove > 0  && sections.Count > 0)
        {
            // reduce line by 1
            var newLast = string.Join("",sections.Last().Skip(1));
            if (!string.IsNullOrEmpty(newLast))
            {
                sections[^1] = newLast;
            }
            else
            {
                sections = sections.Take(sections.Count - 1).ToList();
            }
            howManyMoreToRemove--;
        }
        return (sections, groups);
    }
    
    private class SpringRow
    {
        public int Id;
        public string Row;
        public List<int> Groups;
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