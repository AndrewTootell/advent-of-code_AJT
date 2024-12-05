using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day19;

public class Day19
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 19;

    public Day19(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 19114)]
    [InlineData(false, 0, 397061)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var mods = ParseInput(input);
        long total = RunLogic(mods);
        _testOutputHelper.WriteLine($"total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(true, 0, 167409079868000)]
    //[InlineData(false, 0, 284132)]
    public void Test_2(bool isTest, int testDataCount, ulong expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var mods = ParseInput(input);
        ulong total = 0;
        _testOutputHelper.WriteLine($"total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private long RunLogic(Dictionary<string, Module> mods)
    {
        var total = 1;
        var startStates = mods.Values.ToList();
        var cont = true;
        var mod = mods["broadcaster"];
        var previousKey = "";
        var pulse = Pulse.Low;
        var modsToSend = new List<(string,string)>{ ("broadcaster","button") };
        var modsThatReceived = new List<string>();
        while (cont)
        {
            foreach (var keys in modsToSend)
            {
                mods[keys.Item1].SendPulse(keys.Item2,pulse);
            }
            var subCont = true;
            for (var index = 0; index < startStates.Count; index++)
            {
                subCont &= mods.Values.ToList()[index].Is(startStates[index]);
            }
            cont = !subCont;
        }
        return total;
    }

    private enum Pulse
    {
        High,
        Low,
        Null
    }

    private abstract class Module
    {
        public string Label;
        public List<string> GoesTo;

        protected Module(string label, string goesTo)
        {
            Label = label;
            GoesTo = goesTo.Split(',').ToList();
        }

        public abstract (Pulse, List<string>) SendPulse(string key, Pulse input);

        public bool Is(Module mod)
        {
            return Label == mod.Label;
        }
    }

    private class Broadcaster : Module
    {
        public Broadcaster(string label, string goesTo) : base(label, goesTo) { }

        public override (Pulse, List<string>) SendPulse(string key, Pulse input)
        {
            return (input, GoesTo);
        }
    }

    private class FlipFlop : Module
    {
        public bool State = false;

        public FlipFlop(string label, string goesTo) : base(label, goesTo) { }

        public override (Pulse, List<string>) SendPulse(string key, Pulse input)
        {
            if (input == Pulse.Low)
            {
                var pulseResult = State ? Pulse.Low : Pulse.High;
                State = !State;
                return (pulseResult, GoesTo);
            }
            return (Pulse.Null, new List<string>());
        }

        public bool Is(FlipFlop mod)
        {
            return base.Is(mod) && State == mod.State;
        }
    }

    private class Conjunction : Module
    {
        public Dictionary<string, Pulse> MostRecentPerModule = new();
        public int InputCount;

        public Conjunction(string label, string goesTo) : base(label, goesTo) { }

        public override (Pulse, List<string>) SendPulse(string key, Pulse input)
        {
            MostRecentPerModule[key] = input;
            if(MostRecentPerModule.Count == InputCount && MostRecentPerModule.All(pair=>pair.Value == Pulse.High))
            {
                return (Pulse.Low, GoesTo);
            }

            return (Pulse.High, GoesTo);
        }

        public bool Is(Conjunction mod)
        {
            return base.Is(mod) &&
                   InputCount == mod.InputCount &&
                   MostRecentPerModule.SequenceEqual(mod.MostRecentPerModule);
        }
    }

    private Dictionary<string,Module> ParseInput(List<string> inputs)
    {
        var data = new Dictionary<string,Module>();
        foreach (var input in inputs)
        {
            // %a -> b
            var line = input.Replace(" ", "");
            var parts = line.Split(" ->");
            string key;
            Module mod;
            switch (parts[0].First())
            {
                case '%':
                    key = string.Join("",parts[0].Skip(1));
                    mod = new FlipFlop(key, parts[1]);
                    break;
                case '&':
                    key = string.Join("",parts[0].Skip(1));
                    mod = new Conjunction(key, parts[1]);
                    break;
                default:
                    key = parts[0];
                    mod = new Broadcaster(key, parts[1]);
                    break;
            }
            data.Add(key, mod);
        }

        var conjunctions = data.Where(p => p.Value.GetType() == typeof(Conjunction));
        foreach (var keyValuePair in conjunctions)
        {
            ((Conjunction)keyValuePair.Value).InputCount = data.Count(p=> p.Value.GoesTo.Contains(keyValuePair.Key));
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
            var line = sr.ReadLine()!;
            input.Add(line);
        }
        return input;
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}