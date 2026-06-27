using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Basic;

public readonly struct BPoint(double x, double y)
{
    public double X { get; } = x;

    public double Y { get; } = y;

    public double Length => Math.Sqrt(X * X + Y * Y);

    public double Length2 => X * X + Y * Y;

    #region OPERATORS

    public static bool operator ==(BPoint a, BPoint b)
        => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(BPoint a, BPoint b)
        => a.X != b.X || a.Y != b.Y;

    public BPoint Scale(double scale, BPoint center)
        => (this - center) * scale + center;

    public static BPoint operator + (BPoint a, BPoint b)
        => new (a.X + b.X, a.Y + b.Y);

    public static BPoint operator *(BPoint a, double scale)
        => new (a.X * scale, a.Y * scale);
    

    public static BPoint operator *(double scale, BPoint a)
        => new (a.X * scale, a.Y * scale);

    public static BPoint operator -(BPoint a)
        => new(-a.X, -a.Y);

    public static BPoint operator -(BPoint a, BPoint b)
        => new(a.X - b.X, a.Y - b.Y);

    public static BPoint operator /(BPoint a, double scalar)
        => new(a.X / scalar, a.Y / scalar);
    #endregion

    public readonly BPoint Rotate(double radians)
        => Rotate(radians, new BPoint(0, 0)); 

    public readonly BPoint Rotate(double radians, BPoint center)
    {
        var X2 = X - center.X;
        var Y2 = Y - center.Y;
        double cos = Math.Cos(radians);
        double sin = Math.Sin(radians);
        return new BPoint(X2 * cos - Y2 * sin, X2 * sin + Y2 * cos) + center;
    }

    public double DistanceToLine2(BPoint p1, BPoint p2)
    {
        var ab = p2 - p1;
        var ap = this - p1;
        var abLen2 = ab.Length2;
        if (abLen2 == 0) return ap.Length2; // line degenerates to a point
        var t = (ap.X * ab.X + ap.Y * ab.Y) / abLen2;
        var closest = p1 + ab * t;
        var diff = this - closest;
        return diff.Length2;
    }

    public readonly BPoint Normalize()
    {
        var l = Length;
        if (l == 0) return this;
        var x = X / l;
        var y = Y / l;
        return new(x, y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BPoint other) return false;
        return other == this;
    }

    public override int GetHashCode()
        => HashCode.Combine(X, Y);
    
}
