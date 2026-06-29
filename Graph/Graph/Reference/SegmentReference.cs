using Graph.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph.Reference;

public class SegmentReference(AnchorReference from, AnchorReference to)
{
    public readonly AnchorReference From = from;

    public readonly AnchorReference To = to;

    public BPoint P0 => From.Position;

    public BPoint P1 => From.PNext;

    public BPoint P2 => To.PLast;

    public BPoint P3 => To.Position;

    public BBezierSegment Segment => new(P0, P1, P2, P3);

    public override bool Equals(object? obj)
    {
        return obj is SegmentReference other
            && From.Equals(other.From)
            && To.Equals(other.To);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(From, To);
    }
}
