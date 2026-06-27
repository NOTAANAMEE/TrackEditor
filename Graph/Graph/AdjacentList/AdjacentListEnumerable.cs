using Graph.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph;

internal partial class AdjacentList
{
    private class FullSegmentEnumerable(AdjacentList list) :
        IEnumerable<AnchorSegment>
    {
        public IEnumerator<AnchorSegment> GetEnumerator()
        {
            foreach (var from in list.Anchors)
            {
                if (!list.TryGetNextAnchor(from, out var to)) continue;
                yield return new AnchorSegment(from, to);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class SegmentEnumerableFrom(Anchor anchor, AdjacentList list)
        : IEnumerable<AnchorSegment>
    {
        public IEnumerator<AnchorSegment> GetEnumerator()
        {
            var current = anchor;
            while (list.TryGetNextAnchor(current, out var next))
            {
                yield return new AnchorSegment(current, next);
                if (next == anchor) yield break;
                current = next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class SegmentEnumerableTo(Anchor anchor, AdjacentList list)
        : IEnumerable<AnchorSegment>
    {
        public IEnumerator<AnchorSegment> GetEnumerator()
        {
            var current = anchor;
            while (list.TryGetLastAnchor(current, out var last))
            {
                yield return new AnchorSegment(last, current);
                if (last == anchor) yield break;
                current = last;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
