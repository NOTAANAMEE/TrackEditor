using Graph.Command;
using Graph.Graph;
using Graph.Graph.Reference;

namespace SpecificControls.Editor.Default;

public static class AnchorRemoveSpliceHelper
{
    public record RemoveSlice(AnchorReference Start, AnchorReference Stop, bool IsRing);

    public static ComplexTransform GetRemoveTransform(BezierGraph graph, IReadOnlySet<AnchorReference> anchors)
    {
        var ret = new ComplexTransform();
        foreach (var slice in Slice(graph, anchors))
        {
            ret.Add(GenerateComplexTransformForOneSlice(graph, slice));
        }
        ret.Add(new SimpleAnchorsRemoveTransform(anchors, graph));
        return ret;
    }

    private static ComplexTransform GenerateComplexTransformForOneSlice(
        BezierGraph graph, RemoveSlice slice)
    {
        var complexTransform = new ComplexTransform();
        complexTransform.Add(
            new SegmentsRemoveTransform(GetSliceRemovingSegment(graph, slice), graph));
        if (slice.IsRing) return complexTransform;

        AnchorReference? prevAnchor = null, nextAnchor = null;
        if (graph.TryGetLastSegment(slice.Start, out var prevSeg)) prevAnchor = prevSeg.From;
        if (graph.TryGetNextSegment(slice.Stop, out var nextSeg)) nextAnchor = nextSeg.To;
        if (prevAnchor != null)
        {
            var lastSegment = graph.GetLastSegment(slice.Start);
            complexTransform.Add(new SegmentRemoveTransform(lastSegment, graph));
            if (nextAnchor != null && nextAnchor != prevAnchor)
            {
                complexTransform.Add(
                    new SegmentAddTransform(
                        new SegmentReference(prevAnchor, nextAnchor), graph[lastSegment]));
            }
        }
        return complexTransform;
    }


    private static List<RemoveSlice> Slice(BezierGraph graph, IReadOnlySet<AnchorReference> anchors)
    {
        var visited = new HashSet<AnchorReference>();
        List<RemoveSlice> removeSlices = [];
        foreach (var anchor in anchors)
        {
            if (visited.Contains(anchor)) continue;
            var thisSlice = GetSlice(anchor, graph, anchors, visited);
            removeSlices.Add(thisSlice);
        }
        return removeSlices;
    }

    private static IEnumerable<SegmentReference> GetSliceRemovingSegment(
        BezierGraph graph, RemoveSlice slice)
    {
        // Since it is a ring, it must have NextSegment
        if (slice.IsRing) return graph.GetSegmentsRelated(graph.GetNextSegment(slice.Start));
        if (!graph.TryGetNextSegment(slice.Stop, out var segment))
            return graph.GetSegmentsFromTo(slice.Start, slice.Stop);
        return graph.GetSegmentsFromTo(slice.Start, segment.To);
    }

    private static RemoveSlice GetSlice(
        AnchorReference start,
        BezierGraph graph, 
        IReadOnlySet<AnchorReference> anchors, 
        HashSet<AnchorReference> visited)
    {
        visited.Add(start);
        var startAnchor = GetStart(start, graph, anchors, visited, out var isRing);
        if (isRing) return new RemoveSlice(start, start, true);
        var endAnchor = GetEnd(start, graph, anchors, visited);
        return new RemoveSlice(startAnchor, endAnchor, false);
    }

    private static AnchorReference GetStart(
        AnchorReference start,
        BezierGraph graph,
        IReadOnlySet<AnchorReference> anchors,
        HashSet<AnchorReference> visited,
        out bool ring)
    {
        var first = start;
        ring = false;
        while (true)
        {
            if (!graph.TryGetLastSegment(start, out var reference)) break;
            var prevAnchor = reference.From;
            if (!anchors.Contains(prevAnchor)) break;
            start = prevAnchor;
            if (start == first)
            {
                ring = true;
                break;
            }
            visited.Add(start);
        }
        return start;
    }

    private static AnchorReference GetEnd(
        AnchorReference start,
        BezierGraph graph,
        IReadOnlySet<AnchorReference> anchors,
        HashSet<AnchorReference> visited)
    {
        var first = start;
        while (true)
        {
            if (!graph.TryGetNextSegment(start, out var reference)) break;
            var NextAnchor = reference.To;
            if (!anchors.Contains(NextAnchor)) break;
            start = NextAnchor;
            if (start == first) break;
            visited.Add(start);
        }
        return start;
    }
}
