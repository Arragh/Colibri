using Colibri.Services.SnapshotProvider.Models;
using Colibri.Theory.Structs;

namespace Colibri.Services.SnapshotProvider.Interfaces;

public interface ISnapshotProvider
{
    GlobalSnapshot GlobalSnapshot { get; }
    ref readonly TheorySnapshot TheorySnapshot { get; }
}