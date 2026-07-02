using Graph.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph.Reference;

public class AnchorReference
{

    internal Anchor _source;

    internal BezierGraph? _parent;

    public bool IsValid => _parent?.ContainsAnchor(this) ?? false;

    public AnchorReference(BPoint position, BPoint pLast, BPoint pNext)
    {
        _source = new(position, pLast - position, pNext - position);
    }

    internal AnchorReference(Anchor anchor)
    {
        _source = anchor;
    }

    internal AnchorReference(Anchor source, BezierGraph? parent): this(source)
    {
        _parent = parent;
    }

    public BPoint Position
    {
        get => GetPosition(0);
    }

    public BPoint PLast
    {
        get => GetPosition(1);
    }

    public BPoint PNext
    {
        get => GetPosition(2);
    }

    public BPoint GetPosition(PositionType type)
    {
        var bPoint = type switch
        {
            PositionType.Position => _source.Position,
            PositionType.PLast => _source.PLast,
            PositionType.PNext => _source.PNext,
            _ => throw new ArgumentException("invalid type")
        };
        if (_parent == null)
        {
            return bPoint;
        }
        return _parent.ToWorldPoint(bPoint);
    }

    private BPoint GetPosition(int type)
    {
        var bPoint = type switch
        {
            0 => _source.Position,
            1 => _source.PLast,
            2 => _source.PNext,
            _ => throw new ArgumentException("invalid type")
        };
        if (_parent == null) return bPoint;
        return _parent.ToWorldPoint(bPoint);
    }

    public override bool Equals(object? obj)
    {
        return obj is AnchorReference other
            && ReferenceEquals(_source, other._source)
            && ReferenceEquals(_parent, other._parent);
    }

    public static bool operator ==(AnchorReference? a, AnchorReference? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(AnchorReference? a, AnchorReference? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_source, _parent);
    }
}
