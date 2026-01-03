using Colibri.Theory.Structs;

namespace Colibri.Theory;

public sealed class TheorySnapshotWrapper(TheorySnapshot theorySnapshot)
{
    private TheorySnapshot _theorySnapshot = theorySnapshot;

    public ref readonly TheorySnapshot TheorySnapshot => ref _theorySnapshot;
}