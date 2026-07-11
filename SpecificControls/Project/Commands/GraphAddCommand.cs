using SpecificControls.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpecificControls.Project.Commands;

public class GraphAddCommand(GraphInfo2 graph) : ProjectCommandBase
{
    public override void Execute(TrackProjectDTO track)
    {
        track.AddGraph(graph);
    }

    public override void Undo(TrackProjectDTO track)
    {
        track.RemoveGraph(graph);
    }
}

public class GraphRemoveCommand(GraphInfo2 graph) : ProjectCommandBase
{
    public override void Execute(TrackProjectDTO track)
    {
        track.RemoveGraph(graph);
    }

    public override void Undo(TrackProjectDTO track)
    {
        track.AddGraph(graph);
    }
}

