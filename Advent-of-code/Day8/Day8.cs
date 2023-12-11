
namespace Advent_of_code.Day8;

public class Day8
{
    [Fact]
    public void Test_1()
    {
        var (input,data) = ReadInput("Day8/Data.txt");
        var nodes = ParseInputToMap(data);
        var steps = 0;
        var count = 0;
        var currentNode = nodes["AAA"];
        while (currentNode.Name != "ZZZ")
        {
            var direction = input[count];
            
            currentNode = (direction switch
            {
                'L' => currentNode.LeftNode,
                'R' => currentNode.RightNode,
                _ => currentNode
            })!;
            count = count == input.Length-1? 0: count+1;
            steps += 1;
        }
        
        Console.WriteLine($"Total steps: {steps}");
        // TestData1: 2
        // TestData2: 6
        // Data: 16271
    }
    
    [Fact]
    public void Test_2()
    {
        var (input,data) = ReadInput("Day8/Data.txt");
        
        var nodes = ParseInputToMap(data);
        var steps = 0;
        var count = 0;
        var currentNodes = nodes.Values.Where(k=> k.Name[2] == 'A').ToList();
        var history = new List<Dictionary<string, int>>();
        for (var i = 0; i < currentNodes.Count; i++)
        {
            history.Add(new Dictionary<string, int>());
        }
        var loopingNodes = new Dictionary<Node,(int,int)>();
        while (currentNodes.Any())
        {
            // find if the path loops
            var i = 0;
            var nodesToRemove = new List<Node>();
            currentNodes.ForEach(currentNode =>
            {
                var hasLooped = history[i].TryGetValue(currentNode.Name+count, out _);
                if (!hasLooped)
                {
                    history[i].Add(currentNode.Name+count, steps);
                    i += 1;
                    return;
                };
                history.RemoveAt(i);
                loopingNodes.Add(currentNode, (count-1,steps-1));
                nodesToRemove.Add(currentNode);
            });
            nodesToRemove.ForEach(n=> currentNodes.Remove(n));

            var direction = input[count];
            var nextNodes = new List<Node>();
            currentNodes.ForEach(currentNode =>
            {
                var nextNode = (direction switch
                {
                    'L' => currentNode.LeftNode,
                    'R' => currentNode.RightNode,
                    _ => currentNode
                })!;
                nextNodes.Add(nextNode);
            });
            currentNodes = nextNodes;
            count = count == input.Length-1? 0: count+1;
            steps += 1;
        }
        
        var factors = new List<int>();
        foreach (var keyValuePair in loopingNodes)
        {
            CalcFactors(keyValuePair.Value, factors);
        }
        long total = 1;
        foreach (var factor in factors)
        {
            total *= factor;
        }
        
        Console.WriteLine($"Total steps: {total}");
        // TestData3: 6
        // Data: 14265111103729
    }

    private void CalcFactors((int, int) values, List<int> factors)
    {
        var value = values.Item2 - values.Item1;
        var root = Math.Sqrt(value);
        for (var i = 2; i < root; i++)
        {
            if (value % i != 0) continue;
            
            var lower = factors.Contains(i);
            var higher = factors.Contains(value/i);
            if (!lower)
            {
                factors.Add(i);
            }
            if (!higher)
            {
                factors.Add(value/i);
            }
        }
    }

    private Dictionary<string, Node> ParseInputToMap(List<string> data)
    {
        var nodes = new Dictionary<string, Node>();
        data.ForEach(line =>
            {
                var parts = line.Split('=');
                var connectedNodes = parts[1].Split(',');
                var nodeName = parts[0].TrimEnd();
                var leftNodeName = connectedNodes[0].TrimStart().TrimStart('(');
                var rightNodeName = connectedNodes[1].TrimStart().TrimEnd(')');
                nodes.TryGetValue(nodeName, out Node? node);
                if (node == null)
                {
                    node = new Node
                    {
                        Name = nodeName
                    };
                    nodes.Add(nodeName, node);
                }
                
                nodes.TryGetValue(leftNodeName, out Node? leftNode);
                if (leftNode == null)
                {
                    leftNode = new Node
                    {
                        Name = leftNodeName
                    };
                    nodes.Add(leftNodeName, leftNode);
                }
                node.LeftNode = leftNode;
                
                nodes.TryGetValue(rightNodeName, out Node? rightNode);
                if (rightNode == null)
                {
                    rightNode = new Node
                    {
                        Name = rightNodeName
                    };
                    nodes.Add(rightNodeName, rightNode);
                }
                node.RightNode = rightNode;
            }
        );
        return nodes;
    }

    private class Node
    {
        public required string Name;
        public Node? LeftNode;
        public Node? RightNode;
    }
    
    private static (string, List<string>) ReadInput(string filePathEnd)
    {
        var lines = new List<string>();
        var count = 0;
        string input = "";
        using var sr = new StreamReader(
            "/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/" + filePathEnd);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            if (count < 2)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    input = line;
                }

                count += 1;
                continue;
            }
            lines.Add(line);
        }

        return (input,lines);
    }
}