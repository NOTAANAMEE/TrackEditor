using SpecificControls.Graph;

namespace SpecificControls.Project.Commands;

public class CellAddCommand(CellInfo cell) : ProjectCommandBase
{
    public override void Execute(TrackProjectDTO track) => track.AddCell(cell);

    public override void Undo(TrackProjectDTO track) => track.RemoveCell(cell);
}

public class CellRemoveCommand(CellInfo cell) : ProjectCommandBase
{
    public override void Execute(TrackProjectDTO track) => track.RemoveCell(cell);

    public override void Undo(TrackProjectDTO track) => track.AddCell(cell);
}