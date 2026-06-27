using Graph.Basic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace Controls;

public abstract class TracableSegment: Selectable
{
    public abstract Point P0 { get; }

    public abstract Point P1 { get; }

    public abstract Point P2 { get; }

    public abstract Point P3 { get; }

    public override string? SelectionGroup { get; }

    public BBezierSegment Segment 
        => new(P0.ToBPoint(), P1.ToBPoint(), P2.ToBPoint(), P3.ToBPoint());

    public Action? ControlPointChanged;
}

public static class Helper
{
    public static BPoint ToBPoint(this Point point) 
        => new(point.X, -point.Y);

    public static Point ToPoint(this BPoint point)
        => new(point.X, -point.Y);
}
