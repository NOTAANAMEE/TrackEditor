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
        segment.Segment.Split(t, out var leftSeg, out var rightSeg);
        var anchor = new AnchorReference(leftSeg.P3, leftSeg.P2, rightSeg.P1);
        var command = new ComplexTransform();
        command.Add(new SegmentRemoveTransform(segment, graph));
        var pointTransform = new PointTransform();
        pointTransform.Add(new PositionChanger(segment.From, PositionType.PNext, leftSeg.P1));
        pointTransform.Add(new PositionChanger(segment.To, PositionType.PLast, rightSeg.P2));
        command.Add(pointTransform);
        command.Add(new AnchorAddTransform(anchor));
        command.Add(new SegmentAddTransform(new(segment.From, anchor), graph[segment]));
        command.Add(new SegmentAddTransform(new(anchor, segment.To)));
        return command;
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
