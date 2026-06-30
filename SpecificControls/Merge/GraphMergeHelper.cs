using Graph.Command;
using Graph.Graph.Reference;
using SpecificControls.Graph;

namespace SpecificControls.Merge;

public static class GraphMergeHelper
{
    /// <summary>
    /// Copy source to target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static CommandBase MergeGraph(GraphInfo source)
    {
        var complexTransform = new ComplexTransform();
        var sourceGraph = source.Graph.Graph;
        var anchorsDict = sourceGraph.Anchors
            .Select(a => new KeyValuePair<AnchorReference, AnchorReference>(
                a, new AnchorReference(a.Position, a.PLast, a.PNext))).ToDictionary();
        var segments = sourceGraph.Segments
            .Select(s => new SegmentReference(anchorsDict[s.From], anchorsDict[s.To]));
        var segmentProperties = sourceGraph.Segments.Select(a => sourceGraph[a].Copy());
        var anchors = anchorsDict.Select(a => a.Value);
        var anchorProperties = anchorsDict.Select(a => sourceGraph[a.Key].Copy());
        complexTransform.Add(new AnchorsAddTransformWithProperty(anchors, anchorProperties));
        complexTransform.Add(new SegmentsAddPropertyTransform(segments, segmentProperties));
        return complexTransform;
    }
}
