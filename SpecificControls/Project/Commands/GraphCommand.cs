using Graph.Command;
using Graph.Graph;

namespace SpecificControls.Project.Commands;

/// <summary>
/// Execute command on the target graph.
/// </summary>
/// <param name="command"></param>
public class GraphCommand(CommandBase command, TrackProjectDTO track) : ProjectCommandBase
{
    private BezierGraph graph = track.SelectedGraph.Graph;

    public override void Execute(TrackProjectDTO track)
        => command.Execute(graph);

    public override void Undo(TrackProjectDTO track)
        => command.Undo(graph);
    
}
