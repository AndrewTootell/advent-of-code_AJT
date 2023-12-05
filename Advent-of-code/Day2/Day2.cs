namespace Advent_of_code.Day2;

public class Day2
{
    [Test]
    public void Test_1()
    {
        var gameCount = 0;
        using (var sr = new StreamReader(
                   "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day2/Data.txt"))
        {
            string? line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine()!;
                var lineParts = line.Split(':').ToList();
//                line = lineParts.ElementAtOrDefault(1);
                var id = Int32.Parse(lineParts[0].Split(' ')[1]);
                line = lineParts[1];
                var games = line.Split(';');
                var validGame = true;
                var gameDict = new Dictionary<string, int>();
                foreach (var game in games)
                {
                    var colours = game.Split(',');
                    foreach (var colour in colours)
                    {
                        var data = colour.Split(' ');
                        var colourCount = Int32.Parse(data[1]);
                        var colourName = data[2];
                        if(gameDict.ContainsKey(colourName))
                        {
                            if (gameDict[colourName] < colourCount)
                            {
                                gameDict[colourName] = colourCount;
                            }
                            continue;
                        }
                        gameDict.Add(colourName, colourCount);
                    }
                }

                validGame = validGame && !(gameDict.ContainsKey("red") && gameDict["red"] > 12);
                validGame = validGame && !(gameDict.ContainsKey("green") && gameDict["green"] > 13);
                validGame = validGame && !(gameDict.ContainsKey("blue") && gameDict["blue"] > 14);
                // Console.WriteLine($"Game {id}: {line}");
                // Console.WriteLine($"is {validGame} => " +
                //                   $"red: {gameDict.GetValueOrDefault("red")}, " +
                //                   $"green: {gameDict.GetValueOrDefault("green")}, " +
                //                   $"blue: {gameDict.GetValueOrDefault("blue")}, ");
                // Console.WriteLine("");
                if (validGame)
                {
                    gameCount += id;
                }
            }
            
            Console.WriteLine($"game count: {gameCount}");
        }
        /*
         * Read line
         * split on ':'
         * split on ';'
         * record highest of each colour
         * test each colour is less than limit
         * if true count +1
         *
         */
        
    }
    
    [Test]
    public void Test_2()
    {
        var gameCount = 0;
        using (var sr = new StreamReader(
                   "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day2/Data.txt"))
        {
            string? line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine()!;
                var lineParts = line.Split(':').ToList();
                line = lineParts[1];
                var games = line.Split(';');
                var validGame = true;
                var gameDict = new Dictionary<string, int>();
                foreach (var game in games)
                {
                    var colours = game.Split(',');
                    foreach (var colour in colours)
                    {
                        var data = colour.Split(' ');
                        var colourCount = Int32.Parse(data[1]);
                        var colourName = data[2];
                        if(gameDict.ContainsKey(colourName))
                        {
                            if (gameDict[colourName] < colourCount)
                            {
                                gameDict[colourName] = colourCount;
                            }
                            continue;
                        }
                        gameDict.Add(colourName, colourCount);
                    }
                }

                gameCount += gameDict.GetValueOrDefault("red", 1) * gameDict.GetValueOrDefault("green", 1) * gameDict.GetValueOrDefault("blue", 1);
                
                // Console.WriteLine($"Game {id}: {line}");
                // Console.WriteLine($"is {validGame} => " +
                //                   $"red: {gameDict.GetValueOrDefault("red")}, " +
                //                   $"green: {gameDict.GetValueOrDefault("green")}, " +
                //                   $"blue: {gameDict.GetValueOrDefault("blue")}, ");
                // Console.WriteLine("");
            }
            
            Console.WriteLine($"game count: {gameCount}");
        }
    }
}