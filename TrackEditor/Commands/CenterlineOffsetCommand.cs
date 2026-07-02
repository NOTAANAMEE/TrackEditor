using Graph.Command;
using Graph.Graph;
using Graph.Graph.Reference;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Selection;

namespace TrackEditor.Commands;

public struct CenterlineOffsetCommandParameter
{
    public bool Left;
    public bool Right;
    public double Offset;
}

public class CenterlineOffsetCommand() : EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "SegmentCommand:CenterlineOffsetCommand";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        
        if (!args.Selectables.Any()) return;
        if (args.Parameters is null ||
            args.Parameters is not CenterlineOffsetCommandParameter param) return;
        var offset = param.Offset;
        ComplexTransform transform = new();
        if (param.Left) transform.Add(Command(args, offset, LeftRightEnum.Left));
        if (param.Right) transform.Add(Command(args, offset, LeftRightEnum.Right));
        args.Graph.Execute(transform);
    }

    private static ComplexTransform Command(
        EditorSignalArgs args, double offset, LeftRightEnum leftRight)
    {
        var list = new List<SegmentReference>();
        var transforms = GetSegments(
            args.Graph.Graph, 
            args.Selectables.OfType<MyTracableSegment>().Select(a => a.SegmentReference))
            .Select(a => new CenterlineOffsetFitTransform(a, offset, leftRight));
        return new ComplexTransform([..transforms]);
    }

    private static List<IEnumerable<SegmentReference>> GetSegments(
        BezierGraph graph,
        IEnumerable<SegmentReference> segments)
    {
        HashSet<SegmentReference> segmentSet = [.. segments];
        HashSet<SegmentReference> visited = [];
        var ret = new List<IEnumerable<SegmentReference>>();
        foreach (var segment in segmentSet)
        {
            if (visited.Contains(segment)) continue;
            visited.Add(segment);
            var last = segment;
            var next = segment;
            while (graph.TryGetLastSegment(last, out var seg) 
                && segmentSet.Contains(seg) && seg != next)
            {
                visited.Add(seg);
                last = seg;
            }
            while (graph.TryGetNextSegment(next, out var seg) 
                && segmentSet.Contains(seg) && seg != last)
            {
                visited.Add(seg);
                next = seg;
            }
            ret.Add(graph.GetSegmentsFromTo(last.From, next.To));
        }
        return ret;
    }
}
