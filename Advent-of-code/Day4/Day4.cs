using System.Net;

namespace Advent_of_code.Day4;

public class Day4
{
    [Fact]
    public void Test_1()
    {
        var sumTotal = 0;
        
        using (var sr = new StreamReader(
                   "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day4/Data.txt"))
        {
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == null)
                {
                    continue;
                }
                var x = line.Split(':')[1];
                var numbers = x.Split('|');
                var own = numbers[0].Split(' ').ToList();
                var winning = numbers[1].Split(' ').ToList();
                
                List<int> ownList = new();
                own.ForEach(n =>
                {
                    if(int.TryParse(n, out int m))
                        ownList.Add(m);
                });
                
                List<int> winningList = new();
                winning.ForEach(n =>
                {
                    if(int.TryParse(n, out int m))
                        winningList.Add(m);
                });

                var value = 0;
                
                ownList.ForEach(n =>
                {
                    if (!winningList.Contains(n)) return;
                    
                    if (value == 0)
                    {
                        value = 1;
                        return;
                    }
                    
                    value *= 2;
                });
                
                sumTotal += value;
            }
        }
        
        Console.WriteLine(sumTotal);
    }
    
    [Fact]
    public void Test_1_2()
    {
        List<(List<int>, List<int>)> cards = new();
        
        using (var sr = new StreamReader(
                   "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day4/Data.txt"))
        {
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == null)
                {
                    continue;
                }
                var x = line.Split(':')[1];
                var numbers = x.Split('|');
                var own = numbers[0].Split(' ').ToList();
                var winning = numbers[1].Split(' ').ToList();
                
                List<int> ownList = new();
                own.ForEach(n =>
                {
                    if(int.TryParse(n, out int m))
                        ownList.Add(m);
                });
                
                List<int> winningList = new();
                winning.ForEach(n =>
                {
                    if(int.TryParse(n, out int m))
                        winningList.Add(m);
                });

                cards.Add((ownList,winningList));
            }
        }
        
        var sumTotal = 0;
        Dictionary<int, int> scores = new();
        
        for(var i = cards.Count-1; i >= 0 ; i--)
        {
            //sumTotal += 1;
            var score = 1;
            var ownList = cards[i].Item1;
            var winningList = cards[i].Item2;
            
            var newCards = ownList.Count(n => winningList.Contains(n));
            
            for (var j = 1; j <= newCards; j++)
            {
                 score += scores[i + j];
            }
            scores.Add(i, score);
            sumTotal += score;
        }
        
        Console.WriteLine(sumTotal);
    }
}