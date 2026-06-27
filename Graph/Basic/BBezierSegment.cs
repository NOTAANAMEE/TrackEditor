using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Basic;

public readonly struct BBezierSegment(BPoint p0, BPoint p1, BPoint p2, BPoint p3)
{
    private static readonly double epsilon = 0.5;

    private static readonly double epsilon2 = epsilon * epsilon;

    private static readonly double tolerance = 0.2;

    private static readonly double tolerance2 = tolerance * tolerance;

    public BPoint P0 => p0;

    public BPoint P1 => p1;

    public BPoint P2 => p2;

    public BPoint P3 => p3;

    public void Flattern(List<BBezierSegment> segments)
    {
        if (IsAlmostLine())
        {
            segments.Add(this);
            return;
        }
        if ((P3 - P0).Length2 < tolerance2) // If the curve is very short, we can consider it as a line segment.
        {
            segments.Add(this);
            return;
        }
        Split(0.5, out var left, out var right);
        left.Flattern(segments);
        right.Flattern(segments);
    }

    public void Split(double t, out BBezierSegment left, out BBezierSegment right)
    {
        static BPoint Lerp(BPoint a, BPoint b, double t)
            => a + (b - a) * t;
        var p01 = Lerp(P0, P1, t);
        var p12 = Lerp(P1, P2, t);
        var p23 = Lerp(P2, P3, t);
        var p012 = Lerp(p01, p12, t);
        var p123 = Lerp(p12, p23, t);
        var p0123 = Lerp(p012, p123, t);
        left = new BBezierSegment(P0, p01, p012, p0123);
        right = new BBezierSegment(p0123, p123, p23, P3);
    }

    private bool IsAlmostLine()
        => P1.DistanceToLine2(P0, P3) < epsilon2 && P2.DistanceToLine2(P0, P3) < epsilon2;


    public BPoint GetPoint(double t)
    {
        var x = 1 - t;
        return (x * x * x) * P0 + 3 * (x * x) * t * P1 + 3 * x * (t * t) * P2 + (t * t * t) * P3;
    }

    public BBezierSegment Rotate(double radians)
        => Rotate(radians, new (0, 0));

    public BBezierSegment Rotate(double radians, BPoint center)
        => new(P0.Rotate(radians, center), P1.Rotate(radians, center), P2.Rotate(radians, center), P3.Rotate(radians, center));

    #region OPERATORS
    public static BBezierSegment operator +(BPoint a, BBezierSegment b)
        => b + a;

    public static BBezierSegment operator +(BBezierSegment a, BPoint b)
        => new(a.P0 + b, a.P1 + b, a.P2 + b, a.P3 + b);

    public static BBezierSegment operator -(BBezierSegment a, BPoint b)
        => new(a.P0 - b, a.P1 - b, a.P2 - b, a.P3 - b);

    public static BBezierSegment operator *(BBezierSegment a, double b)
        => new(a.P0 * b, a.P1 * b, a.P2 * b, a.P3 * b);

    public static BBezierSegment operator *(double a, BBezierSegment b)
        => new(b.P0 * a, b.P1 * a, b.P2 * a, b.P3 * a);
    #endregion

    public BPoint GetDerivative(double t)
    {
        double u = 1 - t;

        return
            (P1 - P0) * (3 * u * u) +
            (P2 - P1) * (6 * u * t) +
            (P3 - P2) * (3 * t * t);
    }

    public BPoint GetSecondDerivative(double t)
    {
        double u = 1 - t;
        return
            (P2 - 2 * P1 + P0) * (6 * u) +
            (P3 - 2 * P2 + P1) * (6 * t);
    }

    public double DistanceToPoint2(BPoint point, out double t)
    {
        // Use the Newton-Raphson method to find the closest point on the curve
        // to the given point.
        const int samples = 32;

        t = 0;
        double best = double.PositiveInfinity;

        for (int i = 0; i <= samples; i++)
        {
            double ti = (double)i / samples;
            double d2 = (GetPoint(ti) - point).Length2;

            if (d2 < best)
            {
                best = d2;
                t = ti;
            }
        }

        for (int i = 0; i < 10; i++)
        {
            var p = GetPoint(t);
            var d = p - point;
            var dp = GetDerivative(t);
            var ddp = GetSecondDerivative(t);
            var numerator = d.X * dp.X + d.Y * dp.Y;
            var denominator = dp.X * dp.X + dp.Y * dp.Y + d.X * ddp.X + d.Y * ddp.Y;
            if (denominator == 0) break;
            var dt = -numerator / denominator;
            t += dt;
            if (Math.Abs(dt) < epsilon) break;
        }
        var closestPoint = GetPoint(t);
        return (point - closestPoint).Length2;
    }
}
