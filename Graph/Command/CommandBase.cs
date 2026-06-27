using Graph.Graph;

namespace Graph.Command;

public abstract class CommandBase
{
    public abstract void Execute(BezierGraph graph);

    public abstract void Undo(BezierGraph graph);
}
