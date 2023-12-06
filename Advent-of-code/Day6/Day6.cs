namespace Advent_of_code.Day6;

public class Day6
{
    [Test]
    public void Test_1()
    {
        var data = ReadInput("Day6/Data.txt");
        var lines = data.Split('\n');
        var timesLine = lines[0].Split(':')[1];
        var recordLine = lines[1].Split(':')[1];

        var timeList = timesLine.Split(' ').ToList().Where(s => s != "").ToList().ConvertAll(int.Parse).ToList();
        var recordList = recordLine.Split(' ').ToList().Where(s => s != "").ToList().ConvertAll(int.Parse).ToList();
        double total = 1;
        
        for (var i = 0; i < timeList.Count; i++)
        {
            var time = timeList[i];
            var record = recordList[i];
            var (min, max) = CalculateChargeRange(time, record);
            Console.WriteLine($"Min: {min} Max: {max}");
            Console.WriteLine($"Change by: {max-min+1}");
            total *= max-min+1;
            Console.WriteLine($"Total: {total}");
        }
        
        Console.WriteLine(total);
    }
    
    [Test]
    public void Test_2()
    {
        var data = ReadInput("Day6/Data.txt");
        var lines = data.Split('\n');
        var timesLine = lines[0].Split(':')[1];
        var recordLine = lines[1].Split(':')[1];

        var timeList = timesLine.Split(' ').ToList().Where(s => s != "").ToList().ConvertAll(int.Parse).ToList();
        var recordList = recordLine.Split(' ').ToList().Where(s => s != "").ToList().ConvertAll(int.Parse).ToList();
        double total = 1;

        var time3 = "";
        var record3 = "";
        timesLine.Split(' ').ToList().Where(s => s != "").ToList().ForEach(t=>time3+=t);
        recordLine.Split(' ').ToList().Where(s => s != "").ToList().ForEach(r=>record3+=r);
        
        var time = long.Parse(time3);
        var record = long.Parse(record3);
        Console.WriteLine($"Time: {time} Record: {record}");
        var (min, max) = CalculateChargeRange(time, record);
        Console.WriteLine($"Min: {min} Max: {max}");
        Console.WriteLine($"Change by: {max-min+1}");
        total *= max-min+1;
        Console.WriteLine($"Total: {total}");
        
        
        Console.WriteLine(total);
    }

    private (double, double) CalculateChargeRange(long time, long distance)
    {
        /*
         * time to travel * mph = distance
         * (time - charge) * charge = distance
         * time*charge - charge*charge = distance
         * -x^2+bx-c = 0
         * x = (-b+-root(bb-4*-1*c))/2*-1
         * x = (-b+-root(bb+4c))/-2
         * charge = (-time+-root(time^2+4*distance)/-2
         */
        var s1 = time * time - 4 * distance;
        var s2 = Math.Sqrt(s1);
        var c1 = -time + s2;
        var c11 = c1 / -2;
        
        var c2 = -time - s2;
        var c22 = c2 / -2;
        

        //var charge1 = (-time + Math.Sqrt(time * time - 4 * distance)) / -2;
        //var charge2 = (-time - Math.Sqrt(time * time - 4 * distance)) / -2;
        var result = (Math.Ceiling(c11), Math.Floor(c22));
        if (c11 == result.Item1)
        {
            result.Item1 += 1;
        }
        if (c22 == result.Item2)
        {
            result.Item2 -= 1;
        }
        return result;
    }
    
    private static string ReadInput(string filePathEnd)
    {
        var line = "";

        using var sr = new StreamReader(
            "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/" + filePathEnd);
        while (!sr.EndOfStream)
        {
            line = sr.ReadToEnd();
        }

        return line;
    }
}