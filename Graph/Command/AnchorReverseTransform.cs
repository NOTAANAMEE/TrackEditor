using Graph.Graph;
using Graph.Graph.Reference;

namespace Graph.Command;

public class AnchorReverseTransform(IEnumerable<AnchorReference> anchors)
    : CommandBase
{
    AnchorReference[] _anchors = [.. anchors];

    public override void Execute(BezierGraph graph)
    {
        foreach (var anchor in _anchors)
        {
            graph.ChangePosition(
                [ 
                new(anchor, PositionType.PLast, anchor.PNext),
                new(anchor, PositionType.PNext, anchor.PLast)
                ]);
        }
    }

    public override void Undo(BezierGraph graph)
    {
        foreach (var anchor in _anchors)
        {
            graph.ChangePosition(
                [
                new(anchor, PositionType.PLast, anchor.PNext),
                new(anchor, PositionType.PNext, anchor.PLast)
                ]);
        }
    }
}
