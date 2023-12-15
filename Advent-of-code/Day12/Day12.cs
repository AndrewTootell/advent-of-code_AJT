using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day12;

public class Day12
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Dictionary<string,List<List<int>>> _tryGetVariations = new();
    private readonly Dictionary<string,int> _tryVariationsCache = new();
    private readonly List<(List<List<int>>, SpringRow)> _vars = new();
    private readonly List<long> _subTotals = new();
    
    public Day12(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 21)]
    [InlineData(false, 0, 7344)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var startTime = DateTime.Now;
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        long total = RunLogic(data);
        _testOutputHelper.WriteLine($"Total: {total}");
        total.Should().Be(expectedAnswer);
    }

    public static IEnumerable<object[]> GetTestCases()
    {
        return Day12TestCases.TestCases();
    }
    
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void Test_1_1(Day12TestCases.TestCase testCase)
    {
        var springRow = testCase.springRow;
        var groups = testCase.groups;
        var springRowObj = new SpringRow
        {
            Groups = testCase.groups,
            Row = testCase.springRow
        };
        long total = RunLogic(new List<SpringRow>{springRowObj});
        
        total.Should().Be(testCase.expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0,  0)]
    [InlineData(true, 1,  0)]
//    [InlineData(false, 0,  0)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var startTime = DateTime.Now;
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        data = UnFold(data);
        long total = RunLogic(data);
        
        _testOutputHelper.WriteLine($"Total: {total} < 7427");
        var endTime = DateTime.Now;
        _testOutputHelper.WriteLine($"Time: {endTime-startTime} < 00.7295700");
        //total.Should().Be(expectedAnswer);
    }

    private long RunLogic(List<SpringRow>? data)
    {
        var startTime = DateTime.Now;

        var workItemCount = 0;
        var completedWork = 0;
        data.ForEach(springRow =>
            {
                ThreadPool.QueueUserWorkItem(data=>
                {
                    GetVariationsAction(data);
                    WriteLine($"GetVariations {completedWork+1}/{workItemCount}");
                    completedWork++;
                }, springRow);
                workItemCount++;
            }
        );
        while(completedWork < workItemCount){}
        
        var getVar = DateTime.Now;
        _testOutputHelper.WriteLine($"GetVariations: {getVar - startTime} < 00.1624260");
        List<(List<List<int>>, SpringRow)> variations;
        lock (_vars)
        {
            variations = _vars.ToList();
        }
        
        workItemCount = 0;
        completedWork = 0;
        variations.ForEach(t =>
            {
                ThreadPool.QueueUserWorkItem(data=>
                {
                    TryVariationsAction(data);
                    WriteLine($"TryVariations {completedWork+1}/{workItemCount}");
                    completedWork++;
                }, t);
                workItemCount++;
            }
            );
        while(completedWork < workItemCount){}
        var endTime = DateTime.Now;
        _testOutputHelper.WriteLine($"TryVariations: {endTime - getVar} < 00.0564950");
        long total;
        lock (_subTotals)
        {
            total = _subTotals.Sum();
        }
        return total;
    }
    private void GetVariationsAction(object input)
    {
        var springRow = (SpringRow)input;
        if (springRow.Row.Length == springRow.Groups.Sum() + springRow.Groups.Count - 1)
        {
            lock (_subTotals)
            {
                _subTotals.Add(1);
            }
            return;
        }
        var variations = GetVariations(springRow.Id, springRow.Row.Split('.').ToList().Count, springRow.Groups.Count);
        lock (_vars)
        {
            _vars.Add((variations,springRow));
        }
    }

    private void TryVariationsAction(object input)
    {
        var t = ((List<List<int>>, SpringRow))input;
        var (variations, springRow) = t;
        variations.ForEach(v =>
            {
                var subTotal = TryVariations(springRow.Id, v, springRow.Row, springRow.Groups);
                lock (_subTotals)
                {
                    _subTotals.Add(subTotal);
                }
            }
        );
    }

    private void WriteLine(string message)
    {
        _testOutputHelper.WriteLine(message);
    }
    
    private List<List<int>> GetVariations(int id, int splitCount, int groupsCount)
    {
        var key = $"{splitCount}_{groupsCount}";
        bool hasValue;
        List<List<int>>? result;
        lock (_tryGetVariations)
        {
            hasValue = _tryGetVariations.TryGetValue(key, out result);
        }
        if(hasValue)
        {
            return result;
        }
        var count = 0;
        var numBase = groupsCount + 1;
        var caseToTry = new List<List<int>>();
        List<int> toTry;
        long ttCount;
        int posInNumReversed;
        var differentWayToSplitGroups = (int)Math.Pow(numBase, splitCount) - 1;
        for (var i = differentWayToSplitGroups; i >= 0; i-=groupsCount)
        {
            toTry = new List<int>();
            ttCount = 0;
            posInNumReversed = splitCount - 1;
            while (toTry.Sum() <= groupsCount)
            {
                var baseForIndex = Convert.ToInt64(Math.Pow(numBase, posInNumReversed));
                if (posInNumReversed == 0)
                {
                    toTry = toTry.Append((int)(i-ttCount)).ToList();
                    break;
                }
                toTry = toTry.Append((int)((i-ttCount)/baseForIndex)).ToList();
                ttCount += toTry.Last() * baseForIndex;
                posInNumReversed--;
            }
            if (toTry.Sum() == groupsCount)
            {
                caseToTry.Add(toTry);
            }
        }

        lock (_tryGetVariations)
        {
            if(!_tryGetVariations.TryGetValue(key, out List<List<int>>? _))
            {
                _tryGetVariations.Add(key,caseToTry);
            }
        }
        
        return caseToTry;
    }
    
    private int TryVariations(int id, List<int> toTry, string springRow, List<int> groups)
    {
        var split = springRow.Split('.');
        var subCount = 1;
        var previousGroupsSum = 0;
        for (var i = 0; i < toTry.Count; i++)
        {
            var groupSize = toTry[i];
            int subSubCount;
            var s = split[i];
            var g = groups.Skip(previousGroupsSum).Take(groupSize).ToList();
            bool gotValue;
            int value;
            lock (_tryVariationsCache)
            {
                gotValue = _tryVariationsCache.TryGetValue(s + string.Join("", g), out value);
            }
            if (gotValue)
            {
                subSubCount = value;
            }
            else
            {
                subSubCount = FindPossibilities(id, s, g);
                lock (_tryVariationsCache)
                {
                    if (!_tryVariationsCache.TryGetValue(s+string.Join("",g), out _))
                    {
                        _tryVariationsCache.Add(s+string.Join("",g), subSubCount);
                    }
                }
                
            }

            if (subCount == 0)
            {
                return 0;
            }
            subCount *= subSubCount;
            previousGroupsSum += groupSize;
        }
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

        var result = BruteForce(id, springRow, groups);
        if (result <= 1)
        {
            var b = "p";
        }

        return result;
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

        var split = springRow.Split('?');
        if (split.Length > 0 && groups.Count > 0)
        {
            var maxString = split.MaxBy(s=>s.Length);
            if (maxString != null)
            {
                var largestSet = maxString.Length;
                if (largestSet > groups.Max())
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private bool HasOnePossibility(int id,string springRow, List<int> groups)
    {
        if (springRow.Length == 0 || groups.Count == 0)
        {
            return true;
        }

        if (!springRow.Contains('?'))
        {
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

    private readonly Dictionary<string, List<List<char>>> _recursiveMakeLinesCache = new();

    private List<List<char>> RecursiveMakeLines(string springRow, List<int> groups, List<int> separators, int maxDepth, int depth)
    {
        var key = $"{springRow}@{string.Join(',',groups)}@{string.Join(',',separators)}";
        bool gotValue;
        List<List<char>> value;
        lock (_recursiveMakeLinesCache)
        {
            gotValue = _recursiveMakeLinesCache.TryGetValue(key, out value);
        }
        if (gotValue)
        {
            return value;
        }
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
        lock (_recursiveMakeLinesCache)
        {
            if (!_recursiveMakeLinesCache.TryGetValue(key, out _))
            {
                _recursiveMakeLinesCache.Add(key, lines);
            }
        }
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