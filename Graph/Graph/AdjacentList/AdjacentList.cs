using Graph.Basic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Graph.Graph;

internal partial class AdjacentList
{
    private int _segmentCount;
    public int AnchorCount => _adjacentList.Count;
    public int SegmentCount => _segmentCount;

    private readonly Dictionary<Anchor, AnchorInfo> _adjacentList = [];

    public event NotifyCollectionChangedEventHandler? AnchorChanged;
    public event NotifyCollectionChangedEventHandler? SegmentChanged;

    public MyProperty this[Anchor anchor] => GetAnchorPropertyPrivate(anchor);

    public MyProperty this[AnchorSegment s] => GetSegmentPropertyPrivate(s.From, s.To);

    public MyProperty this[Anchor from, Anchor to] => GetSegmentPropertyPrivate(from, to);

    public void AddSegment(Anchor from, Anchor to, MyProperty property)
    {
        AddEdgePrivate(from, to, property);
        NotifySegmentAdded([new AnchorSegment(from, to)]);
    }

    public bool RemoveSegment(Anchor from, Anchor to, 
        [NotNullWhen(true)]out MyProperty? property)
    {
        if (!RemoveEdgePrivate(from, to, out property)) return false;
        NotifySegmentRemoved([new AnchorSegment(from, to)]);
        return true;
    }

    public bool ContainsSegment(Anchor from, Anchor to)
        => ContainsSegmentPrivate(from, to);

    public void ClearSegment()
    {
        ClearSegmentPrivate();
        NotifySegmentReset();
    }

    public void AddAnchor(Anchor anchor, MyProperty property)
    {
        AddAnchorPrivate(anchor, property);
        NotifyAnchorAdded([anchor]);
    }

    public bool RemoveAnchor(Anchor anchor,
        [NotNullWhen(true)] out MyProperty? property)
    {
        property = null;
        if (!ContainsAnchorPrivate(anchor)) return false;
        var edges = new List<AnchorSegment>();

        if (TryGetNextAnchor(anchor, out var next)) edges.Add(new(anchor, next));
        if (TryGetLastAnchor(anchor, out var last)) edges.Add(new(last, anchor));

        edges.ForEach(e => RemoveEdgePrivate(e.From, e.To, out var _));
        NotifySegmentRemoved(edges);
        if (!RemoveAnchorPrivateSimple(anchor, out property)) return false;// for lang checker
        NotifyAnchorRemoved([anchor]);
        return true;
    }

    public bool ContainsAnchor(Anchor anchor) => ContainsAnchorPrivate(anchor);

    public void ClearAnchor()
    {
        ClearAnchorPrivate();
        _segmentCount = 0;
        NotifyAnchorResetSimple();
        NotifySegmentReset();
    }

    #region Mapping
    /// <summary>
    /// This function adds the anchor to the adjacent list
    /// and sets the property to the given property
    /// </summary>
    /// <param name="anchor">The anchor to be added</param>
    /// <param name="property">The property to be set</param>
    /// <exception cref="ArgumentException">The anchor exists in the list</exception>
    private void AddAnchorPrivate(Anchor anchor, MyProperty property)
    {
        if (_adjacentList.ContainsKey(anchor))
            throw new ArgumentException("");
        _adjacentList[anchor] = new AnchorInfo(property);
    }

    /// <summary>
    /// This function simply removes the anchor from the list.
    /// **It will not check the edges!**
    /// </summary>
    /// <param name="anchor">The anchor to be removed</param>
    /// <param name="property">The property of the point</param>
    /// <returns>whether the anchor was in the list before removal</returns>
    private bool RemoveAnchorPrivateSimple(Anchor anchor, 
        [NotNullWhen(true)]out MyProperty? property)
    {
        property = null;
        if (!_adjacentList.TryGetValue(anchor, out var info)) return false;
        property = info.Property;
        _adjacentList.Remove(anchor);
        return true;
    }

    /// <summary>
    /// This function adds the segment to the list.
    /// <c>from</c> and <c>to</c> should both exist in the list.<br/>
    /// It will both add segment info to the <c>from</c> anchor and the <c>to</c> anchor
    /// and <c>_edgeCount++</c>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="property"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void AddEdgePrivate(Anchor from, Anchor to, MyProperty property)
    {
        if (from == to) throw new ArgumentException("Invalid segment");
        if (!_adjacentList.TryGetValue(from, out var fromInfo)
            || !_adjacentList.TryGetValue(to, out var toInfo))
            throw new InvalidOperationException("Both anchors must exist in the graph.");
        if (fromInfo.NextAnchor != null || toInfo.LastAnchor != null)
            throw new InvalidOperationException("Segment Invalid");
        fromInfo.NextAnchor = to;
        toInfo.LastAnchor = from;
        fromInfo.NextProperty = property;
        _segmentCount++;
    }

    /// <summary>
    /// This function removes the segment to the list.
    /// The segment should exist. <br/>
    /// It will both remove segment info in the <c>from</c> anchor and the <c>to</c> anchor
    /// and <c>_edgeCount--</c>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool RemoveEdgePrivate(Anchor from, Anchor to,
        [NotNullWhen(true)] out MyProperty? property)
    {
        property = null;
        if (!_adjacentList.TryGetValue(from, out var fromInfo)
            || !_adjacentList.TryGetValue(to, out var toInfo))
            return false;
        if (!fromInfo.HasNext() || fromInfo.NextAnchor != to || toInfo.LastAnchor != from) 
            return false;
        property = fromInfo.NextProperty;
        fromInfo.NextAnchor = null;
        fromInfo.NextProperty = null;
        toInfo.LastAnchor = null;
        _segmentCount--;
        return true;
    }

    public bool HasNext(Anchor anchor)
    {
        return _adjacentList[anchor].HasNext();
    }

    public bool TryGetNextAnchor(
        Anchor anchor, 
        [NotNullWhen(true)]out Anchor? next)
    {
        next = _adjacentList[anchor].NextAnchor;
        return next != null;
    }

    public Anchor GetNextAnchor(Anchor anchor)
    {
        var ret = _adjacentList[anchor].NextAnchor;
        if (ret != null) return ret;
        throw new InvalidOperationException("This anchor has no ongoing anchor");
    }

    public bool HasLast(Anchor anchor)
    {
        return _adjacentList[anchor].LastAnchor != null;
    }

    public bool TryGetLastAnchor(
        Anchor anchor,
        [NotNullWhen(true)] out Anchor? last)
    {
        last = _adjacentList[anchor].LastAnchor;
        return last != null;
    }

    public Anchor GetLastAnchor(Anchor anchor)
    {
        var ret = _adjacentList[anchor].LastAnchor;
        if (ret != null) return ret;
        throw new InvalidOperationException("This anchor has no incoming anchor");
    }

    private bool ContainsAnchorPrivate(Anchor anchor) => _adjacentList.ContainsKey(anchor);

    private bool ContainsSegmentPrivate(Anchor from, Anchor to)
        => _adjacentList.TryGetValue(from, out var fromInfo) && fromInfo.NextAnchor == to
        && _adjacentList.TryGetValue(to, out var toInfo) && toInfo.LastAnchor == from;

    private MyProperty GetAnchorPropertyPrivate(Anchor anchor)
        => _adjacentList[anchor].Property;

    private MyProperty GetSegmentPropertyPrivate(Anchor from, Anchor to)
    {
        var info = _adjacentList[from];
        if (info.HasNext() && info.NextAnchor == to)
            return info.NextProperty;
        throw new InvalidOperationException("This anchor has no ongoing anchor");
    }

    private void ClearAnchorPrivate() => _adjacentList.Clear();

    private void ClearSegmentPrivate()
    {
        foreach (var pair in _adjacentList)
        {
            pair.Value.NextAnchor = null;
            pair.Value.NextProperty = null;
            pair.Value.LastAnchor = null;
        }
        _segmentCount = 0;
    }
    #endregion

    #region Notification
    private void NotifyAnchorAdded(Anchor[] anchor)
    {
        AnchorChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, anchor));
    }

    private void NotifyAnchorRemoved(Anchor[] anchor)
    {
        AnchorChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, anchor));
    }

    /// <summary>
    /// This function will not invoke SegmentChanged
    /// </summary>
    /// <param name="anchor"></param>
    private void NotifyAnchorResetSimple()
    {
        AnchorChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
    }

    private void NotifySegmentAdded(AnchorSegment[] segments)
    {
        SegmentChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, segments));
    }

    private void NotifySegmentRemoved(List<AnchorSegment> segments)
    {
        if (segments.Count == 0) return;
        SegmentChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, segments));
    }

    private void NotifySegmentReset()
    {
        SegmentChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
    }
    #endregion

    #region Enumerable
    public IEnumerable<Anchor> Anchors => _adjacentList.Keys;
    public IEnumerable<AnchorSegment> Segments => new FullSegmentEnumerable(this);

    public IEnumerable<AnchorSegment> SegmentsFrom(Anchor anchor) 
        => new SegmentEnumerableFrom(anchor, this);

    public IEnumerable<AnchorSegment> SegmentsTo(Anchor anchor)
        => new SegmentEnumerableTo(anchor, this);
    #endregion
}
