using Graph.Graph;
using Graph.Graph.Reference;

namespace Graph.Command;

public class SegmentReverseTransform: ComplexTransform
{
    private SegmentReference[] _segments;

    private SegmentReference[] _newSegments;

    private MyProperty[] _properties;

    public SegmentReverseTransform(IEnumerable<SegmentReference> segments, BezierGraph graph)
    {
        _segments = [.. segments];
        _newSegments = [.. _segments.Select(a => new SegmentReference(a.To, a.From))];
        _properties = [.. _segments.Select(a => graph[a])];
        var anchorSet = new HashSet<AnchorReference>();
        foreach (var segment in _segments)
        {
            anchorSet.Add(segment.From);
            anchorSet.Add(segment.To);
        }
        Add(new SegmentsRemoveTransform(_segments, graph));
        Add(new AnchorReverseTransform(anchorSet));
        Add(new SegmentsAddPropertyTransform(_newSegments, _properties));
    }

    public static bool CheckSegments(IEnumerable<SegmentReference> segments, BezierGraph graph)
    {
        IReadOnlySet<SegmentReference> segmentSet = segments.ToHashSet();
        var ret = true;
        foreach (var segment in segmentSet)
        {
            var leftCheck =
                !graph.TryGetLastSegment(segment.From, out var lastSeg) || 
                segmentSet.Contains(lastSeg);
            var rightCheck = 
                !graph.TryGetNextSegment(segment.To, out var nextSeg) || 
                segmentSet.Contains(nextSeg);
            ret  = ret && leftCheck && rightCheck;
        }
        return ret;
    }
}


