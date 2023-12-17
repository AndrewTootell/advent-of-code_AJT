using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day12;

public class Day12
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Dictionary<string,int> _tryVariationsCache = new();
    private readonly List<long> _subTotals = new();
    
    public Day12(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    #region tests
    [Theory]
    [InlineData(true, 0, 21)]
    [InlineData(false, 0, 7344)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var startTime = DateTime.Now;
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        long total = RunLogic(data);
        WriteLine($"Total: {total}");
        var endTime = DateTime.Now;
        WriteLine($"Time: {endTime-startTime} < 10.0397110");
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
        var springRowObj = new SpringRow(0, testCase.springRow, testCase.groups);
        long total = RunLogic(new List<SpringRow>{springRowObj});
        
        total.Should().Be(testCase.expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0,  525152)]
    [InlineData(true, 1,  506250)]
    [InlineData(false, 0,  0)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var startTime = DateTime.Now;
        var input = ReadInput(12, isTest, testDataCount);
        var data = ParseData(input);
        data = UnFold(data);
        long total = RunLogic(data);
        
        WriteLine($"Total: {total} < 7427");
        var endTime = DateTime.Now;
        WriteLine($"Time: {endTime-startTime} < 10.0397110");
        total.Should().Be(expectedAnswer);
    }
    
    public static IEnumerable<object[]> GetUnfoldedTestCases()
    {
        return Day12TestCases.UnfoldedTestCases();
    }
    
    [Theory]
    [MemberData(nameof(GetUnfoldedTestCases))]
    public void Test_2_1(Day12TestCases.TestCase testCase)
    {
        var startTime = DateTime.Now;
        var springRowObj = new SpringRow(0, testCase.springRow, testCase.groups);
        long total = RunLogic(new List<SpringRow>{springRowObj});
        WriteLine($"Total: {total} < 7427");
        WriteLine($"Time: {DateTime.Now-startTime} < 10.0397110");
        total.Should().Be(testCase.expectedAnswer);
    }
    
    public static IEnumerable<object[]> GetTestCasesToUnfold()
    {
        return Day12TestCases.TestCasesToUnfold();
    }
    
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void Test_2_2(Day12TestCases.TestCase testCase)
    {
        var startTime = DateTime.Now;
        var springRowObj = new SpringRow(0, testCase.springRow, testCase.groups);
        var data = UnFold(new List<SpringRow>{springRowObj});
        long total = RunLogic(data);
        WriteLine($"Total: {total} < 7427");
        WriteLine($"Time: {DateTime.Now-startTime} < 10.0397110");
        //total.Should().Be(testCase.expectedAnswer);
    }
    
    #endregion tests

    private long RunLogic(List<SpringRow> data)
    {
        var time = DateTime.Now;
        data.ForEach(springRow =>
            {
                //WriteLine($"yield return new object[] {{ new TestCase{{springRow = \"{springRow.Row}\", groups = new List<int> {{{string.Join(",",springRow.Groups)}}}, expectedAnswer = 1L }}}};");
                GetVariationsAction(springRow);
                WriteLine($"{springRow.Id}  {DateTime.Now - time}    {springRow.Row}");
                time = DateTime.Now;
            }
        );
        return _subTotals.ToList().Sum();
    }
    
    #region getVariations
    private void GetVariationsAction(SpringRow springRow)
    {
        if (springRow.Row.Length == springRow.Groups.Sum() + springRow.Groups.Count - 1)
        {
            _subTotals.Add(1);
            return;
        }
        GetVariations(springRow.Id, springRow.Row.Split('.').ToList(), springRow.Groups);
    }
    
    private void GetVariations(int id, List<string> split, List<int> groups)
    {
        var splitCount = split.Count;
        var groupsCount = groups.Count;
        
        var caseToTry = new List<List<int>>();
        var first = new List<int>(Enumerable.Repeat(0,splitCount))
        {
            [0] = groupsCount
        };
        
        Recursive(id, splitCount-1, first,splitCount-1, groupsCount + 1, split, groups, caseToTry);
    }
    
    private void Recursive(int id, int splitCount, List<int> toTry/*500*/, int depth/*3*/, int j/*6*/, List<string> split, List<int> groups, List<List<int>> caseToTry)
    {
        if (depth == 0)
        {
            caseToTry.Add(toTry);
            var subTotal = TryVariations(id, toTry, split, groups);
            _subTotals.Add(subTotal);
            return;
        }
        
        for (var i = 1; i <= j; i++)
        {
            var index = splitCount - depth;
            if (CouldBeValid(toTry, split, groups, index))
            {
                Recursive(id, splitCount, toTry.ToList(), depth-1, i, split, groups, caseToTry);
            }
            toTry[splitCount-depth] -= 1;
            toTry[splitCount-depth+1] += 1;
        }
    }

    private bool CouldBeValid(List<int> toTry, List<string> split, List<int> groups, int index)
    {
        var skip = toTry.Take(index).Sum();
        var take = toTry[index];
        var g = groups.Skip(skip).Take(take);
        return split[index].Length >= g.Sum() + take - 1;
    }

    #endregion getVariations
    
    #region tryVariations
    
    private int TryVariations(int id, List<int> toTry, List<string> split, List<int> groups)
    {
        var subCount = 1;
        var previousGroupsSum = 0;
        
        for (var i = 0; i < toTry.Count; i++)
        {
            var groupSize = toTry[i];
            var s = split[i];
            var g = groups.Skip(previousGroupsSum).Take(groupSize).ToList();
            if (HasNoPossibilities(id, s, g))
            {
                return 0;
            }
            previousGroupsSum += groupSize;
        }
        previousGroupsSum = 0;
        for (var i = 0; i < toTry.Count; i++)
        {
            var groupSize = toTry[i];
            int subSubCount;
            var s = split[i];
            var g = groups.Skip(previousGroupsSum).Take(groupSize).ToList();
            if (_tryVariationsCache.TryGetValue(s + string.Join("", g), out var value))
            {
                subSubCount = value;
            }
            else
            {
                subSubCount = FindPossibilities(id, s, g);
                if (!_tryVariationsCache.TryGetValue(s+string.Join("",g), out _))
                {
                    _tryVariationsCache.Add(s+string.Join("",g), subSubCount);
                }
            }

            if (subSubCount == 0)
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
        if (HasOnePossibility(id, springRow, groups))
        {
            return 1;
        }
        
        var result = BruteForce(id, springRow, groups);
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
        
        var groupStrings = new List<string>();
        groups.ForEach(g=> groupStrings.Add(new string('#', g)));
        var pattern = string.Join('.', groupStrings);
        if (springRow.Length == pattern.Length)
        {
            return !CanLineMatch(id, pattern.ToList(), springRow.ToList());
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

        if (groups.Count == 1 && springRow.Contains(new string('#',groups.First())) && groups.First() == springRow.Count(c=>c=='#'))
        {
            return true;
        }
        var groupStrings = new List<string>();
        groups.ForEach(g=> groupStrings.Add(new string('#', g)));
        var pattern = string.Join('.', groupStrings);
        
        if (AllUnknownsAreEmpty(springRow,groups,pattern))
        {
            return true;
        }
        
        if (springRow.Length == pattern.Length)
        {
            return CanLineMatch(id, pattern.ToList(), springRow.ToList());
        }
        
        return false;
    }

    private bool CanLineMatch(int id, IReadOnlyList<char> pattern, IReadOnlyList<char> springRow)
    {
        return !pattern.Where((t, i) => t == '.' && springRow[i] == '#').Any();
    }

    private bool AllUnknownsAreEmpty(string springRow, List<int> groups, string pattern)
    {
        var springReplace = springRow.Replace('?', '.');
        var builtString = new string('.', springRow.Length - pattern.Length);
        var b0 = groups.Sum() == springRow.Count(c => c == '#');
        var b1 = springReplace.Equals($"{pattern}{builtString}");
        var b2 = $"{builtString}{pattern}".Equals(springReplace);
        return b0 && (b1 || b2);
    }
    

    private int BruteForce(int id, string springRow, List<int> groups)
    {
        var score = RecursiveMakeLines(id, springRow, groups, new List<int>(Enumerable.Repeat(1, groups.Count-1)), groups.Count-1, 0);
        return score;
    }

    private readonly Dictionary<string, int> _recursiveMakeLinesCache = new();

    private int RecursiveMakeLines(int id, string springRow, List<int> groups, List<int> separators, int depth, int startIndex)
    {
        var key = $"{springRow}_{string.Join(',', groups)}_{string.Join(',', separators)}_{depth}";
        if (_recursiveMakeLinesCache.TryGetValue(key, out var value))
        {
            return value;
        }
        (var matches, startIndex) = MatchCount(groups.Take(groups.Count-depth).ToList(), separators.Take(separators.Count-depth).ToList(), springRow, depth==0, startIndex);
        if (matches == 0 && depth != 0)
        {
            if (!_recursiveMakeLinesCache.TryGetValue(key, out _))
            {
                _recursiveMakeLinesCache.Add(key, matches);
            }
            return 0;
        }
        var matchCount = 0;
        if (depth == 0)
        {
            if (!_recursiveMakeLinesCache.TryGetValue(key, out _))
            {
                _recursiveMakeLinesCache.Add(key, matches);
            }
            return matches;
        }

        while (groups.Sum() + separators.Sum() <= springRow.Length)
        {
            matchCount += RecursiveMakeLines(id, springRow, groups, separators.ToList(), depth - 1, startIndex);
            separators[^depth] += 1;
        }
        if (!_recursiveMakeLinesCache.TryGetValue(key, out _))
        {
            _recursiveMakeLinesCache.Add(key, matchCount);
        }
        return matchCount;
    }

    private (int, int) MatchCount(List<int> groups, List<int> separators, string springRow, bool finalCheck, int externalStartIndex)
    {
        var pattern = CreatePattern(groups, separators);
        var count = 0;
        var changedStartIndex = false;
        var startIndex = 0;
        while (startIndex + pattern.Count <= springRow.Length)
        {
            if (!springRow.Skip(startIndex).Take(pattern.Count).Contains('#'))
            {
                if (!finalCheck)
                {
                    return (1, startIndex);
                }
                if (!springRow.Contains('#'))
                {
                    count++;
                    if (!changedStartIndex)
                    {
                        externalStartIndex = startIndex;
                        changedStartIndex = true;
                    }
                    startIndex++;
                    continue;
                }
            }
            for (var i = 0; i < pattern.Count; i++)
            {
                if (pattern[i] == '.' && springRow[i+startIndex] == '#')
                {
                    break;
                }

                // is end of for loop
                if (i != pattern.Count - 1)
                {
                    continue;
                }
                
                if (!finalCheck)
                {
                    return (1, startIndex);
                }
                if (!springRow.Contains('#'))
                {
                    count++;
                    break;
                }
                if (springRow.IndexOf('#') >= startIndex && 
                    springRow.LastIndexOf('#') < startIndex + pattern.Count)
                {
                    count++;
                }
            }
            if (!changedStartIndex)
            {
                externalStartIndex = startIndex;
                changedStartIndex = true;
            }
            startIndex++;
        }
        return (count, externalStartIndex);
    }
    
    private List<char> CreatePattern(List<int> groups, List<int> separators)
    {
        var line = new List<char>();
        for (var i = 0; i < separators.Count; i++)
        {
            line = line.Concat(Enumerable.Repeat('#', groups[i])).ToList();
            line = line.Concat(Enumerable.Repeat('.', separators[i])).ToList();
        }
        var pattern = line.Concat(Enumerable.Repeat('#', groups.Last())).ToList();
        return pattern;
    }
    #endregion tryVariations
    
    #region helpers
    private class SpringRow
    {
        public readonly int Id;
        public string Row;
        public List<int> Groups;

        public SpringRow(int id, string row, List<int> groups)
        {
            Id = id;
            Row = row;
            Groups = groups;
        }
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

    private void WriteLine(string message)
    {
        _testOutputHelper.WriteLine(message);
    }

    private List<SpringRow> ParseData(List<string> input)
    {
        var data = new List<SpringRow>();
        var id = 0;
        input.ForEach(line =>
        {
            var lineSplit = line.Split(' ');
            var row = lineSplit[0];//.TrimStart('.').TrimEnd('.');
            while (row.Contains(".."))
            {
                row = row.Replace("..", ".");
            }

            data.Add(new SpringRow(id, row, lineSplit[1].Split(',').ToList().ConvertAll(int.Parse)));
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
    #endregion helpers
}