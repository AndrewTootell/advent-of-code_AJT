using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit.Abstractions;

namespace Advent_of_code.Day16;

public class Day16
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int Day = 16;
    private List<List<Space>> _board;
    private List<Beam> _beams;

    public Day16(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(true, 0, 46)]
    [InlineData(false, 0, 7939)]
    public void Test_1(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        ParseData(input);

        _beams = new List<Beam>{new (0, -1, Direction.Right)};
        RunLogic();

        long total = _board.Sum(row=> row.Count(space=>space.HasBeenPassed()));
        WriteLine($"Total: {total}");
        
        total.Should().Be(expectedAnswer);
    }
    
    [Theory]
    [InlineData(true, 0, 51)]
    [InlineData(false, 0, 8318)]
    public void Test_2(bool isTest, int testDataCount, long expectedAnswer)
    {
        var input = ReadInput(Day, isTest, testDataCount);
        ParseData(input);
        
        long total = RunLogicLooped();

        WriteLine($"Total: {total}");
        
        total.Should().Be(expectedAnswer);
    }

    private int RunLogicLooped()
    {
        var rowCount = _board.Count;
        var totals = new List<int>();
        int total;
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            _beams = new List<Beam>{new (rowIndex, -1, Direction.Right)};
            RunLogic();
            total = _board.Sum(row=> row.Count(space=>space.HasBeenPassed()));
            totals.Add(total);
            ClearBoard();
            _beams = new List<Beam>{new (rowIndex, rowCount, Direction.Left)};
            RunLogic();
            total = _board.Sum(row=> row.Count(space=>space.HasBeenPassed()));
            totals.Add(total);
            ClearBoard();
        }
        var colCount = _board.First().Count;
        for (var colIndex = 0; colIndex < colCount; colIndex++)
        {
            _beams = new List<Beam>{new (-1, colIndex, Direction.Down)};
            RunLogic();
            total = _board.Sum(row=> row.Count(space=>space.HasBeenPassed()));
            totals.Add(total);
            ClearBoard();
            _beams = new List<Beam>{new (colCount, colIndex, Direction.Up)};
            RunLogic();
            total = _board.Sum(row=> row.Count(space=>space.HasBeenPassed()));
            totals.Add(total);
            ClearBoard();
        }
        _beams = new List<Beam>{new (0, -1, Direction.Right)};
        return totals.Max();
    }

    private void ClearBoard()
    {
        foreach (var spaces in _board)
        {
            foreach (var space in spaces)
            {
                space.ClearSpace();
            }
        }
    }

    private void RunLogic()
    {
        while (_beams.Count != 0)
        {
            var (stoppedBeams, newBeams) = BeamStep();

            foreach (var stoppedBeam in stoppedBeams)
            {
                _beams.Remove(stoppedBeam);
            }
            foreach (var newBeam in newBeams)
            {
                _beams.Add(newBeam);
            }
        }
    }

    private (List<Beam>,List<Beam>) BeamStep()
    {
        var stoppedBeams = new List<Beam>();
        var newBeams = new List<Beam>();
        foreach (var beam in _beams)
        {
            if (beam.Moving == Direction.Stop)
            {
                stoppedBeams.Add(beam);
                continue;
            }
            switch (beam.Moving)
            {
                case Direction.Left:
                    beam.CurrentCol -= 1;
                    break;
                case Direction.Right:
                    beam.CurrentCol += 1;
                    break;
                case Direction.Up:
                    beam.CurrentRow -= 1;
                    break;
                case Direction.Down:
                    beam.CurrentRow += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (beam.CurrentRow < 0 || beam.CurrentRow >= _board.Count ||
                beam.CurrentCol < 0 || beam.CurrentCol >= _board[beam.CurrentRow].Count)
            {
                stoppedBeams.Add(beam);
                continue;
            }
            var nextSpace = _board[beam.CurrentRow][beam.CurrentCol];
            var newDirections = nextSpace.GetDirectionChange(beam.Moving);
            beam.Moving = newDirections.Item1;
            if (newDirections.Item2 != Direction.Stop)
            {
                newBeams.Add(new Beam(beam.CurrentRow,beam.CurrentCol,newDirections.Item2));
            }
        }
        return (stoppedBeams,newBeams);
    }

    private class Beam
    {
        public int CurrentRow;
        public int CurrentCol;
        public Direction Moving;

        public Beam(int row, int col, Direction moving)
        {
            CurrentRow = row;
            CurrentCol = col;
            Moving = moving;
        }
    }
    
    private class Space
    {
        private enum SpaceType
        {
            MirrorDown,
            MirrorUp,
            SplitterVertical,
            SplitterHorizontal,
            Empty,
            Null
        }

        public readonly char c;
        private readonly SpaceType _type;
        private List<Direction> _passed;

        public Space(char c)
        {
            this.c = c;
            _passed = new List<Direction>();
            _type = c switch
            {
                '.' => SpaceType.Empty,
                '/' => SpaceType.MirrorUp,
                '\\' => SpaceType.MirrorDown,
                '-' => SpaceType.SplitterHorizontal,
                '|' => SpaceType.SplitterVertical,
                _ => SpaceType.Null
            };
        }

        public void ClearSpace()
        {
            _passed = new List<Direction>();
        }
        

        public bool HasBeenPassed()
        {
            return _passed.Count != 0;
        }

        public (Direction, Direction) GetDirectionChange(Direction input)
        {
            var reverse = Direction.Stop;
            var outDirection = Direction.Stop;
            switch (_type)
            {
                case SpaceType.Empty:
                    outDirection =  input;
                    break;
                case SpaceType.MirrorDown: // \
                    outDirection = input switch
                    {
                        Direction.Up => Direction.Left,
                        Direction.Left => Direction.Up,
                        Direction.Down => Direction.Right,
                        Direction.Right => Direction.Down,
                        _ => outDirection
                    };
                    break;
                case SpaceType.MirrorUp: //  /
                    outDirection = input switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Left => Direction.Down,
                        Direction.Down => Direction.Left,
                        Direction.Right => Direction.Up,
                        _ => outDirection
                    };
                    break;
                case SpaceType.SplitterHorizontal:
                    switch (input)
                    {
                        case Direction.Up:
                        case Direction.Down:
                            outDirection = Direction.Left;
                            reverse = Direction.Right;
                            break;
                        default:
                            outDirection = input;
                            break;
                    }
                    break;
                    case SpaceType.SplitterVertical:
                        switch (input)
                        {
                            case Direction.Left:
                            case Direction.Right:
                                outDirection = Direction.Up;
                                reverse = Direction.Down;
                                break;
                            default:
                                outDirection = input;
                                break;
                        }
                        break;
            }

            if (_passed.Contains(outDirection))
            {
                return (Direction.Stop,Direction.Stop);
            }
            _passed.Add(outDirection);
            return (outDirection,reverse);
        }
        
        private Direction ReverseDirection(Direction inDirection, SpaceType type)
        {
            switch (inDirection)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                default:
                    throw new ArgumentOutOfRangeException($"Error direction could not be reversed {inDirection}");
            }
        }
    }
    
    private void PrintBoard()
    {
        foreach (var spaces in _board)
        {
            var line = "";
            foreach (var space in spaces)
            {
                if (space.HasBeenPassed())
                {
                    line += "#";
                    continue;
                }

                line += '.';
            }
            WriteLine(line);
        }
        WriteLine();
    }

    private enum Direction
    {
        Up,
        Left,
        Down,
        Right,
        Stop
    }
    
    private void ParseData(List<List<char>> data)
    {
        _board = data.Select(row => row.Select(t => new Space(t)).ToList()).ToList();
    }
    
    private List<List<char>> ReadInput(int day, bool isTest = false, int testDataCount = 0)
    {
        var input = new List<List<char>>();
        using var sr = new StreamReader(
            $"/Users/Andy.Tootell/RiderProjects/Advent-of-code/Advent-of-code/Day{day}/{(isTest?"Test":"")}Data{(isTest?testDataCount:"")}.txt");
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            input.Add(line.ToList());
        }

        return input;
    }
    
    private void WriteLine(string message = "")
    {
        _testOutputHelper.WriteLine(message);
    }
}