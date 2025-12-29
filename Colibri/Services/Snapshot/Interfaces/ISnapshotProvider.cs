using Colibri.Services.Snapshot.Models;

namespace Colibri.Services.Snapshot.Interfaces;

public interface ISnapshotProvider
{
    GlobalSnapshot GlobalSnapshot { get; }
}