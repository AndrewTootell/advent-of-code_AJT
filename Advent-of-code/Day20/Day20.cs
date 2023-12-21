using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day20;

public class Day20
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 20;

    public Day20(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 32000000)]
    [InlineData(true, 1, 11687500)]
    [InlineData(false, 0, 670984704)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var mods = ParseInput(input);
        long total = RunLogic(mods);
        _testOutputHelper.WriteLine($"total: {total}");
        total.Should().Be(expectedAnswer);
    }
    [Theory]
    [InlineData(false, 0, 0)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        var mods = ParseInput(input);
        long total = RunLogic2(mods);
        _testOutputHelper.WriteLine($"total: {total}");
        total.Should().Be(expectedAnswer);
    }

    private long RunLogic(IReadOnlyDictionary<string, Module> mods)
    {
        var totalHigh = 0;
        var totalLow = 0;
        int count;
        for (count = 0; count < 1000; count++)
        {
            var (totalLowDelta, totalHighDelta, _) = DoLogic(mods);
            totalLow += totalLowDelta;
            totalHigh += totalHighDelta;
        }
        return (totalHigh) * (totalLow);
    }
    
    private long RunLogic2(IReadOnlyDictionary<string, Module> mods)
    {
        long count = 0;
        var startMods = mods.Values.ToList();
        while (true)
        {
            count += 1;
            var (totalLowDelta, totalHighDelta, rxLowCount) = DoLogic(mods, false);
            if (rxLowCount >= 1)
            {
                break;
            }

            // var isInfinite = true;
            // var currentMods = mods.Values.ToList();
            // for (var index = 0; index < mods.Count; index++)
            // {
            //     if (currentMods[index].GetType() == typeof(FlipFlop))
            //     {
            //         isInfinite &= ((FlipFlop)currentMods[index]).Is((FlipFlop)startMods[index]);
            //         continue;
            //     }
            //     if (currentMods[index].GetType() == typeof(Conjunction))
            //     {
            //         isInfinite &= ((Conjunction)currentMods[index]).Is((Conjunction)startMods[index]);
            //         continue;
            //     }
            //     isInfinite &= currentMods[index].Is(startMods[index]);
            // }
            //
            // if (isInfinite)
            // {
            //     break;
            // }
        }
        return count;
    }

    private (int,int, int) DoLogic(IReadOnlyDictionary<string, Module> mods, bool isTest1 = true)
    {
        var lowDelta = 0;
        var highDelta = 0;
        var rxLowCount = 0;
        var modsToSend = new Queue<(string,string,Pulse)>();
        modsToSend.Enqueue(("broadcaster","button",Pulse.Low));
        lowDelta += 1;
        while (modsToSend.Count > 0)
        {
            var items = modsToSend.Dequeue();
            if (!mods.TryGetValue(items.Item1, out var mod))
            {
                continue;
            }
            var (newPulse, newMods) = mod.SendPulse(items.Item2,items.Item3);
            if (newMods.Count > 0)
            {
                WriteLine($"{items.Item1} {mod.GetType()} {newPulse} => {string.Join(",", newMods)}");
            }
            if (isTest1)
            {
                switch (newPulse)
                {
                    case Pulse.High:
                        highDelta += newMods.Count;
                        break;
                    case Pulse.Low:
                        lowDelta += newMods.Count;
                        break;
                }
            }
            else
            {
                if (newMods.Contains("rx") && newPulse == Pulse.Low)
                {
                    rxLowCount += 0;
                }
            }
            foreach (var newMod in newMods)
            {
                modsToSend.Enqueue((newMod,items.Item1,newPulse));
            }
        }

        return (lowDelta, highDelta, rxLowCount);
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
            MostRecentPerModule.TryAdd(key, Pulse.Null);
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
            var parts = line.Split("->");
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