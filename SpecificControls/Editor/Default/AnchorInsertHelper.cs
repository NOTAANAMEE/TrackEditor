using Controls;
using Graph.Basic;
using Graph.Command;
using Graph.Graph;
using Graph.Graph.Reference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpecificControls.Editor.Default;

public class AnchorInsertHelper
{
    public static CommandBase GetCommand(BezierGraph graph, Point worldPos)
    {
        var segment = ClosestSegment(graph, worldPos, out var t);
        if (segment == null) return new ComplexTransform();
        return new AnchorInsertOnSegmentTransform(segment, t, graph);
    }

    private static SegmentReference? ClosestSegment(BezierGraph graph, Point worldPos, out double t)
    {
        var bPoint = worldPos.ToBPoint();
        SegmentReference? closestSegment = null;
        var distance = double.PositiveInfinity;
        t = 0.0;
        foreach (var bezierSegment in graph.Segments)
        {
            var d = CurveToPoint(bezierSegment.Segment, bPoint, out var t2);
            if (d >= distance) continue;
            distance = d;
            closestSegment = bezierSegment;
            t = t2;
        }
        if (distance > 2) return null;
        return closestSegment;
    }

    private static double CurveToPoint(BBezierSegment segment, BPoint point, out double t)
    {
        return segment.DistanceToPoint2(point, out t);
    }
}
