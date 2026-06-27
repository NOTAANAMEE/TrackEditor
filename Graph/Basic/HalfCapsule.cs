using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Basic;

public readonly struct HalfCapsule(BPoint p1, BPoint p2, double lineWidth, bool left)
{
    public readonly BPoint Start => p1;

    public readonly BPoint End => p2;

    public readonly double LineWidth => lineWidth;

    public readonly bool Left => left;

    private readonly double lineWidth2 = lineWidth * lineWidth;

    public bool Contains(BPoint point)
    {
        if (IsLeft(point) != Left) return false;
        var ab = End - Start;
        var ap = point - Start;
        var abLen2 = ab.Length2;
        if (abLen2 == 0) return ap.Length2 <= lineWidth2; // circle case

        var t = (ap.X * ab.X + ap.Y * ab.Y) / abLen2;
        // When the projection falls outside the segment, we clamp it to the nearest endpoint
        t = Math.Clamp(t, 0, 1);
        var closest = Start + ab * t;
        var diff = point - closest;
        return diff.Length2 <= lineWidth2;
    }



    public void Rasterize(ICollection<BPoint> collection)
    {
        int top = (int)Math.Ceiling(Math.Max(Start.Y, End.Y) + LineWidth);
        int bottom = (int)Math.Floor(Math.Min(Start.Y, End.Y) - LineWidth);
        int left = (int)Math.Floor(Math.Min(Start.X, End.X) - LineWidth);
        int right = (int)Math.Ceiling(Math.Max(Start.X, End.X) + LineWidth);
        for (int y = bottom; y <= top; y++)
        {
            for (int x = left; x <= right; x++)
            {
                var p = new BPoint(x + 0.5, y + 0.5);
                if (Contains(p)) collection.Add(new(x, y));
            }
        }
    }

    private bool IsLeft(BPoint p)
    {
        var a = End - Start;
        var b = p - Start;
        var axb = a.X * b.Y - a.Y * b.X;
        if (axb < 0) return false;
        else return true;
    }
}
