using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Basic;

internal class Anchor(BPoint p1, BPoint p2, BPoint p3)
{
    public BPoint Position { get; set; } = p1;

    public BPoint PLastOffset { get; set; } = p2;

    public BPoint PNextOffset { get; set; } = p3;

    public BPoint PLast
    {
        get => Position + PLastOffset;
        set => PLastOffset = value - Position;
    }

    public BPoint PNext
    {
        get => Position + PNextOffset;
        set => PNextOffset = value - Position;
    }

    public static BBezierSegment ToSegment(Anchor p1, Anchor p2)
        => new (p1.Position, p1.PNext, p2.PLast, p2.Position);

    public void Scale(double factor) => Scale(factor, new BPoint(0, 0));

    public void Scale(double factor, BPoint center)
    {
        Position = (Position - center) * factor + center;
        PLastOffset *= factor;
        PNextOffset *= factor;
    }

    public void Rotate(double angle) => Rotate(angle, new BPoint(0, 0));

    public void Rotate(double angle, BPoint center)
    {
        Position = Position.Rotate(angle, center);
        PLastOffset = PLastOffset.Rotate(angle);
        PNextOffset = PNextOffset.Rotate(angle);
    }

    public void Move(BPoint offset)
    {
        Position += offset;
    }
}
