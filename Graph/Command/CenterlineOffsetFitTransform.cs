using Graph.Basic;
using Graph.Graph;
using Graph.Graph.Reference;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Command;

public enum LeftRightEnum
{
    Left,
    Right
}

public class CenterlineOffsetFitTransform : ComplexTransform
{
    private const double StraightSegmentTolerance = 0.5;

    private const double StraightSegmentTolerance2 = StraightSegmentTolerance * StraightSegmentTolerance;

    private const double SimplifyPointTolerance = 0.25;

    private const double SimplifyPointTolerance2 = SimplifyPointTolerance * SimplifyPointTolerance;

    private double _width;

    private LeftRightEnum _leftRight;

    private AnchorReference[] _anchors;

    private SegmentReference[] _addingSegments;

    public CenterlineOffsetFitTransform(
        IEnumerable<SegmentReference> segments, 
        double width, 
        LeftRightEnum leftRight): base([])
    {
        _width = width;
        _leftRight = leftRight;
        List<SegmentReference> seg = [.. segments];
        CheckEligible(seg);
        var points = GenerateLineSegments(seg, out var connected);
        _anchors = GenerateAnchors(points, connected);
        _addingSegments = GenerateSegments(_anchors, connected);
        Add(new AnchorsAddTransform(_anchors));
        Add(new SegmentsAddTransform(_addingSegments));
    }

    private static void CheckEligible(IEnumerable<SegmentReference> segments)
    {
        AnchorReference? nextAnchor = null;
        foreach (SegmentReference segment in segments)
        {
            if (nextAnchor != null && segment.From != nextAnchor)
                throw new InvalidOperationException("");
            nextAnchor = segment.To;
        }
    }

    private List<BPoint> GenerateLineSegments(IEnumerable<SegmentReference> segments, out bool connected)
    {
        var points = new List<BPoint>();
        var firstSegment = segments.First();
        var firstPoint = firstSegment.From.Position;
        var tangent = firstSegment.Segment.GetDerivative(0).Normalize();
        var normal = new BPoint(-tangent.Y, tangent.X);
        var point = firstPoint + normal * _width * (_leftRight == LeftRightEnum.Left ? 1 : -1);
        points.Add(point);
        connected = segments.Last().To == segments.First().From;
        foreach (SegmentReference segment in segments)
        {
            GenerateLineSegmentsForBezier(segment, points);
        }
        if (connected) points.RemoveAt(points.Count - 1);
        SimplifyNearlyStraightPoints(points, connected);
        return points;
    }

    private void GenerateLineSegmentsForBezier(SegmentReference segment, List<BPoint> points)
    {
        if (IsAlmostLine(segment.Segment))
        {
            AddOffsetPoint(segment, 1, points);
            return;
        }

        var samplesPerSegment = 8;
        for (int i = 1; i <= samplesPerSegment; i++)
        {
            double t = (double)i / samplesPerSegment;
            AddOffsetPoint(segment, t, points);
        }
    }

    private void AddOffsetPoint(SegmentReference segment, double t, List<BPoint> points)
    {
        var c = segment.Segment.GetPoint(t);
        var tangent = segment.Segment.GetDerivative(t).Normalize();
        var normal = new BPoint(-tangent.Y, tangent.X);

        var point = c + normal * _width * (_leftRight == LeftRightEnum.Left ? 1 : -1);
        points.Add(point);
    }

    private static bool IsAlmostLine(BBezierSegment segment)
    {
        return segment.P1.DistanceToLine2(segment.P0, segment.P3) < StraightSegmentTolerance2
            && segment.P2.DistanceToLine2(segment.P0, segment.P3) < StraightSegmentTolerance2;
    }

    private static void SimplifyNearlyStraightPoints(List<BPoint> points, bool connected)
    {
        var minCount = connected ? 3 : 2;
        if (points.Count <= minCount) return;

        var changed = true;
        while (changed && points.Count > minCount)
        {
            changed = connected
                ? RemoveOneClosedNearlyStraightPoint(points)
                : RemoveOneOpenNearlyStraightPoint(points);
        }
    }

    private static bool RemoveOneOpenNearlyStraightPoint(List<BPoint> points)
    {
        for (int i = 1; i < points.Count - 1; i++)
        {
            if (!CanRemovePoint(points[i - 1], points[i], points[i + 1])) continue;
            points.RemoveAt(i);
            return true;
        }

        return false;
    }

    private static bool RemoveOneClosedNearlyStraightPoint(List<BPoint> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var previous = points[(i - 1 + points.Count) % points.Count];
            var current = points[i];
            var next = points[(i + 1) % points.Count];
            if (!CanRemovePoint(previous, current, next)) continue;
            points.RemoveAt(i);
            return true;
        }

        return false;
    }

    private static bool CanRemovePoint(BPoint previous, BPoint current, BPoint next)
    {
        return current.DistanceToLine2(previous, next) < SimplifyPointTolerance2;
    }

    private static AnchorReference[] GenerateAnchors(List<BPoint> points, bool connected)
    {   
        // If connected, the end point of the last segment has be removed.
        var anchors = new Anchor[points.Count];
        anchors[0] = new Anchor(points[0], new BPoint(-1, -1), new BPoint(1, 1));
        for (int i = 1; i < points.Count; i++)
        {
            var lastPos = points[i - 1];
            var thisPos = points[i];
            var lastHandleOffset = (thisPos - lastPos) / 3;
            var thisHandleOffset = (lastPos - thisPos) / 2;
            anchors[i - 1].PNextOffset = lastHandleOffset;
            anchors[i] = new Anchor(thisPos, thisHandleOffset, new BPoint(1, 1));
        }
        if (connected)
        {
            var lastPos = points[^1];
            var thisPos = points[0];
            var lastHandleOffset = (thisPos - lastPos) / 3;
            var thisHandleOffset = (lastPos - thisPos) / 3;
            anchors[^1].PNextOffset = lastHandleOffset;
            anchors[0].PLastOffset = thisHandleOffset;
        }
        return [.. anchors.Select(a => new AnchorReference(a))];
    }

    private static SegmentReference[] GenerateSegments(AnchorReference[] anchors, bool connected)
    {
        var ret = new SegmentReference[connected? anchors.Length : anchors.Length - 1];
        for (int i = 1; i < anchors.Length; i++)
        {
            ret[i - 1] = new SegmentReference(anchors[i - 1], anchors[i]);
        }
        if (connected) ret[^1] = new SegmentReference(anchors[^1], anchors[0]);
        return ret;
    }
}
