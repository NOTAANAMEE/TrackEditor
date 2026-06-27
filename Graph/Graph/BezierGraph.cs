using Graph.Basic;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using Graph.Graph.Reference;
using System.Diagnostics.CodeAnalysis;

namespace Graph.Graph;

/// <summary>
/// Stores anchors and directed Bezier segments in a graph-local coordinate system.
/// </summary>
/// <remarks>
/// Public anchor and segment handles use world coordinates. Anchors are converted
/// to graph-local coordinates when they are added, and converted back to world
/// coordinates when they are removed.
/// </remarks>
public partial class BezierGraph
{
    private static readonly BPoint Origin = new();

    private BPoint _center;
    private double _rotation;
    private double _radius;
    private double _scale = 1;
    private readonly AdjacentList _adjacentList = new();

    /// <summary>
    /// Occurs when <see cref="Center"/> changes.
    /// </summary>
    public event EventHandler<BPoint>? CenterChanged;

    /// <summary>
    /// Occurs when <see cref="Rotation"/> changes.
    /// </summary>
    public event EventHandler<double>? RotationChanged;

    /// <summary>
    /// Occurs when <see cref="Radius"/> changes.
    /// </summary>
    public event EventHandler<double>? RadiusChanged;

    /// <summary>
    /// Occurs when <see cref="Scale"/> changes.
    /// </summary>
    public event EventHandler<double>? ScaleChanged;

    /// <summary>
    /// Occurs when anchors are added, removed, or reset.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? AnchorChanged;

    /// <summary>
    /// Occurs when segments are added, removed, or reset.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? SegmentChanged;

    /// <summary>
    /// Occurs after one or more anchor positions or handles are changed.
    /// </summary>
    public event EventHandler<HashSet<AnchorReference>>? AnchorPositionChanged;

    public MyProperty this[AnchorReference reference]
    {
        get => _adjacentList[reference._source];
    }

    public MyProperty this[SegmentReference reference]
    {
        get => _adjacentList[reference.From._source, reference.To._source];
    }

    /// <summary>
    /// Initializes an empty Bezier graph.
    /// </summary>
    public BezierGraph()
    {
        _adjacentList.AnchorChanged += AdjacentList_AnchorChanged;
        _adjacentList.SegmentChanged += AdjacentList_SegmentChanged;
    }

    private void AdjacentList_SegmentChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        NotifyCollectionChangedEventArgs args = e.Action switch
        {
            NotifyCollectionChangedAction.Add => new(e.Action, ToSegReferenceList(e.NewItems), e.NewStartingIndex),
            NotifyCollectionChangedAction.Remove => new(e.Action, ToSegReferenceList(e.OldItems), e.OldStartingIndex),
            NotifyCollectionChangedAction.Reset => new(e.Action),
            _ => throw new InvalidOperationException("")
        };
        SegmentChanged?.Invoke(this, args);
    }

    private void AdjacentList_AnchorChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        NotifyCollectionChangedEventArgs args = e.Action switch
        {
            NotifyCollectionChangedAction.Add => new(e.Action, ToReferenceList(e.NewItems), e.NewStartingIndex),
            NotifyCollectionChangedAction.Remove => new(e.Action, ToReferenceList(e.OldItems), e.OldStartingIndex),
            NotifyCollectionChangedAction.Reset => new(e.Action),
            _ => throw new InvalidOperationException("")
        };
        AnchorChanged?.Invoke(this, args);
    }

    private void NotifyAnchorPositionChanged(HashSet<AnchorReference> changed)
    {
        AnchorPositionChanged?.Invoke(this, changed);
    }

    #region properties
    /// <summary>
    /// Gets or sets the world-space offset of the graph-local origin.
    /// </summary>
    public BPoint Center
    {
        get => _center;
        set
        {
            if (_center == value) return;
            _center = value;
            CenterChanged?.Invoke(this, _center);
        }
    }

    /// <summary>
    /// Gets or sets the graph rotation, in radians.
    /// </summary>
    public double Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation == value) return;
            _rotation = value;
            RotationChanged?.Invoke(this, _rotation);
        }
    }

    /// <summary>
    /// Gets or sets the graph display radius.
    /// </summary>
    public double Radius
    {
        get => _radius;
        set
        {
            if (_radius == value) return;
            _radius = value;
            RadiusChanged?.Invoke(this, _radius);
        }
    }

    /// <summary>
    /// Gets or sets the scale from graph-local coordinates to world coordinates.
    /// </summary>
    public double Scale
    {
        get => _scale;
        set
        {
            if (_scale == value) return;
            _scale = value;
            ScaleChanged?.Invoke(this, _scale);
        }
    }

    /// <summary>
    /// Gets the number of anchors currently stored in the graph.
    /// </summary>
    public int AnchorCount => _adjacentList.AnchorCount;

    /// <summary>
    /// Gets the number of directed segments currently stored in the graph.
    /// </summary>
    public int SegmentCount => _adjacentList.SegmentCount;
    #endregion

    #region collections
    /// <summary>
    /// Adds an anchor to the graph with a default property bag.
    /// </summary>
    /// <param name="reference">The unattached anchor reference to add.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="reference"/> already belongs to a graph.
    /// </exception>
    public void AddAnchor(AnchorReference reference)
        => AddAnchorWithProperty(reference, new MyProperty());

    /// <summary>
    /// Adds an anchor to the graph with the specified property bag.
    /// </summary>
    /// <param name="reference">The unattached anchor reference to add.</param>
    /// <param name="property">The property bag associated with the anchor.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="reference"/> already belongs to a graph.
    /// </exception>
    public void AddAnchorWithProperty(AnchorReference reference, MyProperty property)
    {
        if (reference._parent != null)
            throw new InvalidOperationException("AnchorReference already belongs to a graph.");
        var anchor = reference._source;
        _adjacentList.AddAnchor(anchor, property);
        var position = ToCurrentPoint(anchor.Position);
        var pLast = ToCurrentPoint(anchor.PLast);
        var pNext = ToCurrentPoint(anchor.PNext);
        anchor.Position = position;
        anchor.PLast = pLast;
        anchor.PNext = pNext;
        reference._parent = this;
    }

    /// <summary>
    /// Removes an anchor and any segments connected to it.
    /// </summary>
    /// <param name="reference">The anchor reference to remove.</param>
    /// <returns><see langword="true"/> if the anchor was present and removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveAnchor(AnchorReference reference)
    {
        return RemoveAnchorWithProperty(reference, out var _);
    }

    /// <summary>
    /// Removes an anchor, returns its property bag, and removes any segments connected to it.
    /// </summary>
    /// <param name="reference">The anchor reference to remove.</param>
    /// <param name="property">The property bag associated with the removed anchor.</param>
    /// <returns><see langword="true"/> if the anchor was present and removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveAnchorWithProperty(AnchorReference reference, 
        [NotNullWhen(true)]out MyProperty? property)
    {
        if (!_adjacentList.RemoveAnchor(reference._source, out property)) return false;
        var position = reference.Position;
        var pLast = reference.PLast;
        var pNext = reference.PNext;
        reference._source.Position = position;
        reference._source.PLast = pLast;
        reference._source.PNext = pNext;
        reference._parent = null;
        return true;
    }

    /// <summary>
    /// Determines whether the graph contains the specified anchor.
    /// </summary>
    /// <param name="reference">The anchor reference to look up.</param>
    /// <returns><see langword="true"/> if the anchor is in the graph; otherwise, <see langword="false"/>.</returns>
    public bool ContainsAnchor(AnchorReference reference)
    {
        return _adjacentList.ContainsAnchor(reference._source);
    }

    /// <summary>
    /// Removes all anchors and segments from the graph.
    /// </summary>
    public void ClearAnchor()
    {
        _adjacentList.ClearAnchor();
    }

    /// <summary>
    /// Adds a directed segment with a default property bag.
    /// </summary>
    /// <param name="reference">The segment reference whose endpoints belong to this graph.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when either endpoint is not in the graph, the source already has an outgoing segment,
    /// or the target already has an incoming segment.
    /// </exception>
    public void AddSegment(SegmentReference reference)
    {
        _adjacentList.AddSegment(
            reference.From._source,
            reference.To._source, new MyProperty());
    }

    /// <summary>
    /// Adds a directed segment with the specified property bag.
    /// </summary>
    /// <param name="reference">The segment reference whose endpoints belong to this graph.</param>
    /// <param name="property">The property bag associated with the segment.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when either endpoint is not in the graph, the source already has an outgoing segment,
    /// or the target already has an incoming segment.
    /// </exception>
    public void AddSegmentWithProperty(SegmentReference reference, MyProperty property)
    {
        _adjacentList.AddSegment(
            reference.From._source,
            reference.To._source, property);
    }

    /// <summary>
    /// Removes a directed segment from the graph.
    /// </summary>
    /// <param name="reference">The segment reference to remove.</param>
    /// <returns><see langword="true"/> if the segment was present and removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveSegment(SegmentReference reference)
    {
        return _adjacentList.RemoveSegment(
            reference.From._source, reference.To._source, out var _);
    }

    /// <summary>
    /// Removes a directed segment and returns its property bag.
    /// </summary>
    /// <param name="reference">The segment reference to remove.</param>
    /// <param name="property">The property bag associated with the removed segment.</param>
    /// <returns><see langword="true"/> if the segment was present and removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveSegmentWithProperty(SegmentReference reference,
        [NotNullWhen(true)] out MyProperty? property)
    {
        return _adjacentList.RemoveSegment(
            reference.From._source, reference.To._source, out property);
    }

    /// <summary>
    /// Determines whether the graph contains the specified directed segment.
    /// </summary>
    /// <param name="reference">The segment reference to look up.</param>
    /// <returns><see langword="true"/> if the segment is in the graph; otherwise, <see langword="false"/>.</returns>
    public bool ContainsSegment(SegmentReference reference)
    {
        return _adjacentList.ContainsSegment(reference.From._source, reference.To._source);
    }

    /// <summary>
    /// Removes all segments while keeping the anchors.
    /// </summary>
    public void ClearSegment()
    {
        _adjacentList.ClearSegment();
    }
    #endregion

    /// <summary>
    /// Applies a batch of world-space position changes to anchors and handles.
    /// </summary>
    /// <remarks>
    /// Apply anchor position changes before handle changes. Handles move with the anchor position,
    /// so changing both an anchor and its handles in one batch should list the anchor position first.
    /// </remarks>
    /// <param name="changers">The changes to apply.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when any referenced anchor is not currently in the graph.
    /// </exception>
    public void ChangePosition(IEnumerable<PositionChanger> changers)
    {
        if (changers.Any(p => !ContainsAnchor(p.reference)))
            throw new ArgumentException("Some of the positions are not valid");
        HashSet<AnchorReference> anchorsChanged = [];
        foreach (var changer in changers)
        {
            var anchor = changer.reference._source;
            anchorsChanged.Add(changer.reference);
            switch (changer.positionType)
            {
                case PositionType.Position:
                    anchor.Position = ToCurrentPoint(changer.position);
                    break;
                case PositionType.PLast:
                    anchor.PLast = ToCurrentPoint(changer.position); 
                    break;
                case PositionType.PNext:
                    anchor.PNext = ToCurrentPoint(changer.position);
                    break;
            }
        }
        NotifyAnchorPositionChanged(anchorsChanged);
    }

    /// <summary>
    /// Applies a single world-space position change to an anchor or handle.
    /// </summary>
    /// <param name="changer">The change to apply.</param>
    public void ChangePosition(PositionChanger changer)
        => ChangePosition([changer]);

    #region transform
    private SegmentReference ToSegmentReference(Anchor from, Anchor to)
        => new(new(from, this), new(to, this));

    private SegmentReference ToSegmentReference(AnchorSegment seg)
        => ToSegmentReference(seg.From, seg.To);

    /// <summary>
    /// Converts a graph-local point to world coordinates.
    /// </summary>
    /// <param name="point">The point in graph-local coordinates.</param>
    /// <returns>The corresponding point in world coordinates.</returns>
    public BPoint ToWorldPoint(BPoint point)
        => point.Scale(Scale, Origin).Rotate(Rotation, Origin) + Center;

    /// <summary>
    /// Converts a world-space point to graph-local coordinates.
    /// </summary>
    /// <param name="point">The point in world coordinates.</param>
    /// <returns>The corresponding point in graph-local coordinates.</returns>
    public BPoint ToCurrentPoint(BPoint point)
        => (point - Center).Scale(1 / Scale, Origin).Rotate(-Rotation, Origin);

    private List<AnchorReference> ToReferenceList(IList? list)
    {
        var ret = new List<AnchorReference>();
        if (list == null) return ret;
        foreach (var item in list)
        {
            if (item is not Anchor anchor) continue;
            ret.Add(new AnchorReference(anchor, this));
        }
        return ret;
    }

    private List<SegmentReference> ToSegReferenceList(IList? list)
    {
        var ret = new List<SegmentReference>();
        if (list == null) return ret;
        foreach (var item in list)
        {
            if (item is not AnchorSegment segment) continue;
            ret.Add(ToSegmentReference(segment.From, segment.To));
        }
        return ret;
    }
    #endregion

    #region Enumerator
    /// <summary>
    /// Enumerates the anchors currently stored in the graph.
    /// </summary>
    public IEnumerable<AnchorReference> Anchors 
        => new AnchorReferenceEnumerator(this, _adjacentList.Anchors);

    /// <summary>
    /// Enumerates the directed segments currently stored in the graph.
    /// </summary>
    public IEnumerable<SegmentReference> Segments 
        => new SegmentReferenceEnumerator(this, _adjacentList.Segments);

    /// <summary>
    /// Enumerates directed segments by following outgoing links from the specified anchor.
    /// </summary>
    /// <param name="reference">The anchor where traversal starts.</param>
    /// <returns>The outgoing path starting at <paramref name="reference"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="reference"/> is not currently in the graph.
    /// </exception>
    public IEnumerable<SegmentReference> SegmentsFrom(AnchorReference reference)
    {
        if (!ContainsAnchor(reference))
            throw new ArgumentException("The anchor is not in the graph.", nameof(reference));
        return new SegmentReferenceEnumerator(this, _adjacentList.SegmentsFrom(reference._source));
    }

    /// <summary>
    /// Enumerates directed segments by following incoming links backward from the specified anchor.
    /// </summary>
    /// <param name="reference">The anchor where traversal starts.</param>
    /// <returns>The incoming path ending at <paramref name="reference"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="reference"/> is not currently in the graph.
    /// </exception>
    public IEnumerable<SegmentReference> SegmentsTo(AnchorReference reference)
    {
        if (!ContainsAnchor(reference))
            throw new ArgumentException("The anchor is not in the graph.", nameof(reference));
        return new SegmentReferenceEnumerator(this, _adjacentList.SegmentsTo(reference._source));
    }

    public IEnumerable<SegmentReference> GetSegmentsRelated(SegmentReference reference)
    {
        var from = reference.From;
        var to = reference.To;
        while (HasLastSegment(from) && from != to)
        {
            from = GetLastSegment(from).From;
        }
        while (HasNextSegment(to) && from != to)
        {
            to = GetNextSegment(to).To;
        }
        return new SegmentReferenceFromToEnumerator(this, from, to);
    }

    public IEnumerable<SegmentReference> GetSegmentsFromTo(AnchorReference from, AnchorReference to)
    {
        return new SegmentReferenceFromToEnumerator(this, from, to);
    }

    /// <summary>
    /// Gets a mutable collection view of the graph anchors.
    /// </summary>
    public ICollection<AnchorReference> AnchorCollection => new MyAnchorCollection(this);

    /// <summary>
    /// Gets a mutable collection view of the graph segments.
    /// </summary>
    public ICollection<SegmentReference> SegmentCollection => new MySegmentCollection(this);

    /// <summary>
    /// Gets a collection-changed view of the graph anchors.
    /// </summary>
    public INotifyCollectionChanged NotifyAnchorCollection => new MyAnchorCollection(this);

    /// <summary>
    /// Gets a collection-changed view of the graph segments.
    /// </summary>
    public INotifyCollectionChanged NotifySegmentCollection => new MySegmentCollection(this);
    #endregion

    #region Finder
    /// <summary>
    /// Determines whether the anchor has an outgoing segment.
    /// </summary>
    /// <param name="anchor">The anchor to inspect.</param>
    /// <returns><see langword="true"/> if the anchor has an outgoing segment; otherwise, <see langword="false"/>.</returns>
    public bool HasNextSegment(AnchorReference anchor)
        => _adjacentList.HasNext(anchor._source);

    /// <summary>
    /// Tries to get the outgoing segment from an anchor.
    /// </summary>
    /// <param name="anchor">The anchor to inspect.</param>
    /// <param name="reference">The outgoing segment, when one exists.</param>
    /// <returns><see langword="true"/> if an outgoing segment exists; otherwise, <see langword="false"/>.</returns>
    public bool TryGetNextSegment(AnchorReference anchor,
        [NotNullWhen(true)] out SegmentReference? reference)
    {
        reference = null;
        if (!_adjacentList.TryGetNextAnchor(anchor._source, out var next)) return false;
        reference = ToSegmentReference(anchor._source, next);
        return true;
    }

    /// <summary>
    /// Gets the outgoing segment from an anchor.
    /// </summary>
    /// <param name="anchor">The anchor to inspect.</param>
    /// <returns>The outgoing segment.</returns>
    /// <exception cref="Exception">Thrown when the anchor has no outgoing segment.</exception>
    public SegmentReference GetNextSegment(AnchorReference anchor)
    {
        var next = _adjacentList.GetNextAnchor(anchor._source);
        return ToSegmentReference(anchor._source, next);
    }

    /// <summary>
    /// Determines whether the anchor has an incoming segment.
    /// </summary>
    /// <param name="anchor">The anchor to inspect.</param>
    /// <returns><see langword="true"/> if the anchor has an incoming segment; otherwise, <see langword="false"/>.</returns>
    public bool HasLastSegment(AnchorReference anchor)
        => _adjacentList.HasLast(anchor._source);

    /// <summary>
    /// Tries to get the incoming segment for an anchor.
    /// </summary>
    /// <param name="anchor">The anchor to inspect.</param>
    /// <param name="reference">The incoming segment, when one exists.</param>
    /// <returns><see langword="true"/> if an incoming segment exists; otherwise, <see langword="false"/>.</returns>
    public bool TryGetLastSegment(AnchorReference anchor,
        [NotNullWhen(true)] out SegmentReference? reference)
    {
        reference = null;
        if (!_adjacentList.TryGetLastAnchor(anchor._source, out var last)) return false;
        reference = ToSegmentReference(last, anchor._source);
        return true;
    }

    /// <summary>
    /// Gets the incoming segment for an anchor.
    /// </summary>
    /// <param name="anchor">The anchor to inspect.</param>
    /// <returns>The incoming segment.</returns>
    /// <exception cref="Exception">Thrown when the anchor has no incoming segment.</exception>
    public SegmentReference GetLastSegment(AnchorReference anchor)
    {
        var last = _adjacentList.GetLastAnchor(anchor._source);
        return ToSegmentReference(last, anchor._source);
    }
    #endregion
}
