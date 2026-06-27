using Graph.Graph.Reference;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Graph.Graph;

public partial class BezierGraph
{
    private class MySegmentCollection(BezierGraph graph) :
        ICollection<SegmentReference>, INotifyCollectionChanged
    {
        private BezierGraph _graph = graph;

        public int Count => _graph.SegmentCount;

        public bool IsReadOnly => false;

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => _graph.SegmentChanged += value;
            remove => _graph.SegmentChanged -= value;
        }

        public void Add(SegmentReference item) => _graph.AddSegment(item);

        public void Clear() => _graph.ClearSegment();

        public bool Contains(SegmentReference item) => _graph.ContainsSegment(item);

        public bool Remove(SegmentReference item) => _graph.RemoveSegment(item);

        public void CopyTo(SegmentReference[] array, int arrayIndex)
        {
            foreach (SegmentReference item in _graph.Segments)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<SegmentReference> GetEnumerator()
            => _graph.Segments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class MyAnchorCollection(BezierGraph graph) :
        ICollection<AnchorReference>, INotifyCollectionChanged
    {
        private BezierGraph _graph = graph;

        public int Count => _graph.AnchorCount;

        public bool IsReadOnly => false;

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => _graph.AnchorChanged += value;
            remove => _graph.AnchorChanged -= value;
        }

        public void Add(AnchorReference item) => _graph.AddAnchor(item);

        public void Clear() => _graph.ClearAnchor();

        public bool Contains(AnchorReference item) => _graph.ContainsAnchor(item);

        public bool Remove(AnchorReference item) => _graph.RemoveAnchor(item);

        public void CopyTo(AnchorReference[] array, int arrayIndex)
        {
            foreach (AnchorReference item in _graph.Anchors)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<AnchorReference> GetEnumerator()
            => _graph.Anchors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
