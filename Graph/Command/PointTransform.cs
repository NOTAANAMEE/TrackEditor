using Graph.Basic;
using Graph.Graph;
using Graph.Graph.Reference;

namespace Graph.Command;

public class PointTransform: CommandBase
{
    private record PointTransformRecord(
        AnchorReference Reference, PositionType Type, BPoint Start, BPoint End);

    private readonly List<PointTransformRecord> transforms = [];

    public void Add(IEnumerable<PositionChanger> changers)
    {
        foreach(var changer in changers)
        {
            transforms.Add(new PointTransformRecord(
                changer.reference, changer.positionType,
                changer.reference.GetPosition(changer.positionType),
                changer.position
                ));
        }
    }

    public void Add(PositionChanger changer)
    {
        transforms.Add(new PointTransformRecord(
                changer.reference, changer.positionType,
                changer.reference.GetPosition(changer.positionType),
                changer.position
                ));
    }

    public override void Execute(BezierGraph graph)
    {
        graph.ChangePosition(
            transforms.Select(a => new PositionChanger(a.Reference, a.Type, a.End)));
    }

    public override void Undo(BezierGraph graph)
    {
        graph.ChangePosition(
            transforms.Select(a => new PositionChanger(a.Reference, a.Type, a.Start)));
    }
}
