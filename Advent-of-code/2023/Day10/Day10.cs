using FluentAssertions;

namespace Advent_of_code.Day10;

public class Day10
{
    private List<List<char>> _data = null!;

    /*
       | is a vertical pipe connecting north and south.
       - is a horizontal pipe connecting east and west.
       L is a 90-degree bend connecting north and east.
       J is a 90-degree bend connecting north and west.
       7 is a 90-degree bend connecting south and west.
       F is a 90-degree bend connecting south and east.
       . is ground; there is no pipe in this tile.
     */
    
    [Fact]
    public void Test_1()
    {
        ReadInput(10);
        
        var snakeLocation = FindSnake();

        var path = StartPath(snakeLocation);
        
        path = FollowPath(path);
        
        Console.WriteLine($"Total: {path.First().Count}");
        // TestData0: 4
        // Data: 6757
    }

    [Theory]
    [InlineData(true, 1, 4)]
    [InlineData(true, 2, 8)]
    [InlineData(true, 3, 10)]
    [InlineData(false, 0, 523)]
    public void Test_2(bool isTest, int testDataCount, int expectedAnswer)
    {
        ReadInput(10, isTest, testDataCount);

        var snakeLocation = FindSnake();
        
        var path = StartPath(snakeLocation);
        
        path = FollowPath(path);

        var fullPath = path.First().Concat(path.Last()).ToList();
        var snakeToggles = IsSnakeVerticalLine(snakeLocation, path.First().First().Location, path.Last().First().Location);
        fullPath.Add(new Node { Value = 'S', Location = snakeLocation });
        var isCounting = false;
        var count = 0;
        
        for (var rowIndex = 0; rowIndex < _data.Count; rowIndex++)
        {
            for (var colIndex = 0; colIndex < _data[rowIndex].Count; colIndex++)
            {
                var value = _data[rowIndex][colIndex];
                if (fullPath.Any(n => n.Location.Equals(new CoOrds { Row = rowIndex, Column = colIndex })))
                {
                    // If F or 7 there will also be a J or L
                    if (value is '|' or 'F' or '7' || (value is 'S' && snakeToggles))
                    {
                        isCounting = !isCounting;
                    }
                    continue;
                }
                if (isCounting)
                {
                    count += 1;
                }
            }
        }
        
        Console.WriteLine($"Nest size: {count}");
        count.Should().Be(expectedAnswer);
    }

    private static bool IsSnakeVerticalLine(CoOrds startLocation, CoOrds firstLocation, CoOrds otherFirstLocation)
    {
        return (firstLocation.Row == startLocation.Row + 1 || otherFirstLocation.Row == startLocation.Row + 1) && 
            (firstLocation.Row == startLocation.Row || otherFirstLocation.Row == startLocation.Row || 
             firstLocation.Row == startLocation.Row -1 || otherFirstLocation.Row == startLocation.Row-1);
    }

    private List<List<Node>> StartPath(CoOrds snakeLocation)
    {
        var down = GetNodeByCo_ordAndDirection(snakeLocation, Direction.Down);
        var up = GetNodeByCo_ordAndDirection(snakeLocation, Direction.Up);
        var right = GetNodeByCo_ordAndDirection(snakeLocation, Direction.Right);
        var left = GetNodeByCo_ordAndDirection(snakeLocation, Direction.Left);

        var path = new List<List<Node>>();
        if (down != null)
        {
            path.Add(new List<Node>{down});
        }
        if (up != null)
        {
            path.Add(new List<Node>{up});
        }
        if (right != null)
        {
            path.Add(new List<Node>{right});
        }
        if (left != null)
        {
            path.Add(new List<Node>{left});
        }

        return path;
    }

    private List<List<Node>> FollowPath(List<List<Node>> path)
    {
        var keepLooping = true;
        var routesIndexToRemove = new List<List<Node>>();
        // path will always have 2 routes
        while (keepLooping) // just passed each other
        {
            path.ForEach(route =>
            {
                var currentNode = route.Last();
                var nextNode = GetNodeByCo_ordAndDirection(currentNode);
                if (nextNode == null)
                {
                    routesIndexToRemove.Add(route);
                }
                else
                {
                    route.Add(nextNode);
                }
            });
            routesIndexToRemove.ForEach(r =>
            {
                path.Remove(r);
            });
            routesIndexToRemove.Clear();
            var isSame = path.Count(r=>r.Last().Location.Equals(path.Last().Last().Location)) > 1;
            var hasPassed = path.Last().Count > 2 && 
                            path.Count(r=>r.Last().Location.Equals(path.Last()[path.Last().Count-2].Location)) > 1;
            keepLooping = !(isSame || hasPassed);
        }

        return path;
    }

    private CoOrds FindSnake()
    {
        var snakeRow = _data.Single(line => line.Contains('S'));
        return new CoOrds
        {
            Column = snakeRow.IndexOf('S'),
            Row = _data.IndexOf(snakeRow)
        };
    }

    private Node? GetNodeByCo_ordAndDirection(Node from)
    {
        return GetNodeByCo_ordAndDirection(from.Location, from.PointsTo);
    }
    
    private Node? GetNodeByCo_ordAndDirection(CoOrds startPos, Direction from)
    {
        var rowIndex = startPos.Row;
        switch (from)
        {
            case(Direction.Up):
                rowIndex -= 1;
                break;
            case(Direction.Down):
                rowIndex += 1;
                break;
        }
        if (rowIndex < 0 || rowIndex >= _data.Count)
        {
            return null;
        }

        var row = _data[rowIndex];
        var columnIndex = startPos.Column;
        switch (from)
        {
            case(Direction.Left):
                columnIndex -= 1;
                break;
            case(Direction.Right):
                columnIndex += 1;
                break;
        }
        if (columnIndex < 0 || columnIndex >= row.Count)
        {
            return null;
        }
        Direction? to = null;
        var value = row[columnIndex];
        if (value is '|' or '-')
        {
            to = from;
        }
        else
        {
            to = from switch
            {
                Direction.Down => value switch
                {
                    'L' => Direction.Right,
                    'J' => Direction.Left,
                    _ => null
                },
                Direction.Up => value switch
                {
                    'F' => Direction.Right,
                    '7' => Direction.Left,
                    _ => null
                },
                Direction.Right => value switch
                {
                    'J' => Direction.Up,
                    '7' => Direction.Down,
                    _ => null
                },
                Direction.Left => value switch
                {
                    'L' => Direction.Up,
                    'F' => Direction.Down,
                    _ => null
                },
                _ => to
            };
        }

        if (!to.HasValue)
        {
            return null;
        }
        return new Node
        {
            Location = new CoOrds
            {
                Row = rowIndex,
                Column = columnIndex
            },
            PointsTo = to.Value,
            Value = value
        };
    }

    private class Node
    {
        public CoOrds Location;
        public Direction PointsTo;
        public char Value;
    }
    
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private class CoOrds
    {
        public int Column;
        public int Row;
        public bool Equals(CoOrds obj)
        {
            return Column == obj.Column && Row == obj.Row;
        }
    }
    
    private void ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        _data = new List<List<char>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            _data.Add(line.ToList());
        }
    }
}