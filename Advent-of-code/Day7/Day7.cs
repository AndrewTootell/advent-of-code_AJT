namespace Advent_of_code.Day6;

public class Day7
{
    [Test]
    public void Test_1()
    {
        var data = ReadInput("Day7/Data.txt");
        var handDict = new Dictionary<Score, List<Hand>>();
        data.ForEach(hand =>
            {
                var score = CalcScore(hand.Values);
                if (handDict.TryGetValue(score, out var value))
                {
                    value.Add(hand);
                }
                else
                {
                    handDict.Add(score, new List<Hand> { hand });
                }
            }
            );
        
        long total = 0;
        var count = 0;
        var scores = new List<Score>
            { Score.HighCard, Score.Pair, Score.TwoPair, Score.Three, Score.Fullhouse, Score.Four, Score.Five };
        scores.ForEach(s =>
        {
            handDict.TryGetValue(s, out var hands);
            if (hands == null) return;
            total += ScoreTotal(hands, count);
            count += hands.Count;
        });
        
        Console.WriteLine(total);
        // Test: 6440
        // 250898830
    }
    
    [Test]
    public void Test_2()
    {
        var data = ReadInput("Day7/Data.txt");
        var handDict = new Dictionary<Score, List<Hand>>();
        data.ForEach(hand =>
            {
                var score = CalcScore2(hand.Values);
                var jokerCount = hand.Values.Count(c => c == '1');
                score = ScoreIncrease(score, jokerCount);
                if (handDict.TryGetValue(score, out var value))
                {
                    value.Add(hand);
                }
                else
                {
                    handDict.Add(score, new List<Hand> { hand });
                }
            }
        );
        
        long total = 0;
        var count = 0;
        var scores = new List<Score>
            { Score.HighCard, Score.Pair, Score.TwoPair, Score.Three, Score.Fullhouse, Score.Four, Score.Five };
        scores.ForEach(s =>
        {
            handDict.TryGetValue(s, out var hands);
            if (hands == null) return;
            total += ScoreTotal(hands, count);
            count += hands.Count;
        });
        
        Console.WriteLine(total);
        // Test: 5905
        // 252127335
    }

    private static Score ScoreIncrease(Score scoreIn, int jokerCount)
    {
        if (jokerCount == 5)
        {
            return Score.Five;
        }
        switch (scoreIn)
        {
            case(Score.HighCard):
                switch (jokerCount)
                {
                    case(4):
                        return Score.Five;
                    case(3):
                        return Score.Four;
                    case(2):
                        return Score.Three;
                    case(1):
                        return Score.Pair;
                }
                break;
            case(Score.Pair):
                switch (jokerCount)
                {
                    case(3):
                        return Score.Five;
                    case(2):
                        return Score.Four;
                    case(1):
                        return Score.Three;
                }
                break;
            case(Score.Three):
                switch (jokerCount)
                {
                    case(2):
                        return Score.Five;
                    case(1):
                        return Score.Four;
                }
                break;
            case(Score.TwoPair):
                switch (jokerCount)
                {
                    case(1):
                        return Score.Fullhouse;
                }
                break;
            case(Score.Four):
                switch (jokerCount)
                {
                    case(1):
                        return Score.Five;
                }
                break;
            case(Score.Fullhouse):
            case(Score.Five):
                break;
        }

        return scoreIn;
    }

    private static long ScoreTotal(IEnumerable<Hand> hands, int count)
    {
        var orderedHands = hands.OrderBy(h => OrderValue(h.Values)).ToList();
        long total = 0;
        for (var i = 1; i <= orderedHands.Count; i++)
        {
            total += orderedHands[i-1].Bet * (i + count);
        }
        return total;
    }

    private static Score CalcScore(string values)
    {
        var toCheck = new List<char>{'1','2','3','4','5','6','7','8','9','a','b','c','d','e'};
        var counted = 0;
        foreach (var count in toCheck.Select(c => values.Count(s => s == c)).Where(count => count is not (1 or 0)))
        {
            if (counted == 0)
            {
                switch (count)
                {
                    case(4):
                        return Score.Four;
                    case (5):
                        return Score.Five;
                    case (2):
                    case (3):
                        counted = count;
                        break;
                }
                continue;
            }
            
            switch (count+counted)
            {
                case 4:
                    return Score.TwoPair;
                case 5:
                    return Score.Fullhouse;
            }
        }

        return counted switch
        {
            2 => Score.Pair,
            3 => Score.Three,
            _ => Score.HighCard
        };
    }
    
    private static Score CalcScore2(string values)
    {
        var toCheck = new List<char>{'2','3','4','5','6','7','8','9','a','b','c','d','e'};
        var counted = 0;
        foreach (var count in toCheck.Select(c => values.Count(s => s == c)).Where(count => count is not (1 or 0)))
        {
            if (counted == 0)
            {
                switch (count)
                {
                    case(4):
                        return Score.Four;
                    case (5):
                        return Score.Five;
                    case (2):
                    case (3):
                        counted = count;
                        break;
                }
                continue;
            }
            
            switch (count+counted)
            {
                case 4:
                    return Score.TwoPair;
                case 5:
                    return Score.Fullhouse;
            }
        }

        return counted switch
        {
            2 => Score.Pair,
            3 => Score.Three,
            _ => Score.HighCard
        };
    }
    
    private static List<Hand> ReadInput(string filePathEnd)
    {
        var hands = new List<Hand>();

        using var sr = new StreamReader(
            "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/" + filePathEnd);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            var bet = int.Parse(line.Split(' ')[1]);
            var valuesString = line.Split(' ')[0];
            var valuesHex = valuesString.Replace('A', 'e')
                .Replace('K', 'd')
                .Replace('Q', 'c')
                .Replace('J', '1')
                //.Replace('J', 'b')
                .Replace('T', 'a');
            hands.Add(new Hand
            {
                Bet = bet,
                Values = valuesHex
            });
        }

        return hands;
    }

    private static double OrderValue(string valueHex)
    {
        double total = 0;
        var toCheck = new List<char>{'1','2','3','4','5','6','7','8','9','a','b','c','d','e'};

        for (var i = valueHex.Length-1; i >= 0; i--)
        {
            var c = valueHex[valueHex.Length-i-1];
            total += toCheck.IndexOf(c) * Math.Pow(toCheck.Count,i);
        }
        return total;
    }

    private class Hand
    {
        public string Values { get; set; }
        public int Bet { get; set; }
    }

    private enum Score
    {
        Five,
        Four,
        Fullhouse,
        Three,
        TwoPair,
        Pair,
        HighCard
    }
}