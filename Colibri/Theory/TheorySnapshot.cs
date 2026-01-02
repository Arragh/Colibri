using Colibri.Theory.Structs;

namespace Colibri.Theory;

public struct TheorySnapshot
{
    private readonly char[] _pathSegments;
    private readonly int[] _steps;

    private Segment[] _segments;
    
    public TheorySnapshot(int pathsLenght, int stepsLength)
    {
        _pathSegments = new char[pathsLenght];
        _steps = new int[stepsLength];
    }
    
    public Segment[] Segments => _segments;
}