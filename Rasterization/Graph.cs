using Graph.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rasterization;

public class PointsAndSegments
{
    public record SegmentInfo(int SegmentID, double Coord);

    private readonly List<BPoint> _points = [];

    private readonly List<(int, int)> _segments = [];

    public double Left { get; private set; }

    public double Right { get; private set; }

    public double Top { get; private set; }
    
    public double Bottom { get; private set; }

    public int LeftFloor => (int)Math.Floor(Left);

    public int RightCeil => (int)Math.Ceiling(Right);

    public int BottomFloor => (int)Math.Floor(Bottom);

    public int TopCeil => (int)Math.Ceiling(Top);

    public int Add(BPoint point)
    {
        _points.Add(point);
        var x = point.X;
        var y = point.Y;
        if (x < Left) Left = x;
        if (y > Top) Top = y;
        if (x > Right) Right = x;
        if (y < Bottom) Bottom = y;
        return _points.Count - 1;
    }

    public BPoint GetPoint(int index) => _points[index];

    public void Add(int p1, int p2)
    {
        if (p1 == p2) throw new InvalidOperationException();
        if (p1 >= _points.Count || p2 >= _points.Count) throw new InvalidOperationException();
        _segments.Add((p1, p2));
    }

    private static bool IntersectsHorizontal(BPoint p1, BPoint p2, double y, out double x)
    {
        x = double.NaN;
        double y1 = p1.Y, y2 = p2.Y;
        double maxY = Math.Max(y1, y2), minY = Math.Min(y1, y2);
        if (!(minY < y && y <= maxY)) return false;// Automatically remove horizontal line
        double delX = p2.X - p1.X, delY = p2.Y - p1.Y;
        x = (y - y1) / delY * delX + p1.X;
        return true;
    }

    public IEnumerable<SegmentInfo> IntersectsHorizontal(double y)
    {
        var list = new List<SegmentInfo>();
        int id = 0;
        foreach (var segment in _segments)
        {
            (int i1, int i2) = segment;
            var curId = id++;
            if (!IntersectsHorizontal(_points[i1], _points[i2], y, out var x)) continue;
            list.Add(new(curId, x));
        }
        return list.OrderBy(a => a.Coord);
    }
}
