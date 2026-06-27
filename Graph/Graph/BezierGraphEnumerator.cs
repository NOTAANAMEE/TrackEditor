using Graph.Basic;
using Graph.Graph.Reference;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph;

public partial class BezierGraph
{
    private class SegmentReferenceEnumerator(BezierGraph graph, 
        IEnumerable<AnchorSegment> segments): 
        IEnumerable<SegmentReference>
    {
        private readonly IEnumerable<AnchorSegment> _segments 
            = segments;
        

        public IEnumerator<SegmentReference> GetEnumerator()
        {
            foreach (var segment in _segments)
            {
                yield return graph.ToSegmentReference(segment);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class SegmentReferenceFromToEnumerator(BezierGraph graph,
        AnchorReference from, AnchorReference to) :
        IEnumerable<SegmentReference>
    {
        public IEnumerator<SegmentReference> GetEnumerator()
        {
            var current = from;
            while (graph.HasNextSegment(current))
            {
                var segment = graph.GetNextSegment(current);
                yield return segment;
                current = segment.To;
                if (current == to) break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    private class AnchorReferenceEnumerator(BezierGraph graph, IEnumerable<Anchor> anchors) :
        IEnumerable<AnchorReference>
    {
        private readonly IEnumerable<Anchor> _anchors
            = anchors;

        public IEnumerator<AnchorReference> GetEnumerator()
        {
            foreach (var anchor in _anchors)
            {
                yield return new AnchorReference(anchor, graph);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
