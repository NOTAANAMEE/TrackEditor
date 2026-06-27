using Controls;
using Graph.Basic;
using SpecificControls.Graph;

namespace TrackEditor.Commands;

public class RasterizeLineHelper
{
    private static readonly double MaxDistance = 0.5 * 0.5;

    public static CellInfo RasterizeLines(IEnumerable<BBezierSegment> segments)
    {
        bool[,] cell;
        int left   = int.MaxValue, 
            right  = int.MinValue, 
            bottom = int.MaxValue, 
            top    = int.MinValue;
        foreach (var segment in segments)
        {
            BPoint P0 = segment.P0, P1 = segment.P1, P2 = segment.P2, P3 = segment.P3;
            left   = Math.Min(left, MinFloor(P0.X, P1.X, P2.X, P3.X));
            right  = Math.Max(right, MaxCeiling(P0.X, P1.X, P2.X, P3.X));
            bottom = Math.Min(bottom, MinFloor(P0.Y, P1.Y, P2.Y, P3.Y));
            top    = Math.Max(top, MaxCeiling(P0.Y, P1.Y, P2.Y, P3.Y));
        }
        cell = new bool[right - left, top - bottom];
        var cellInfo = new CellInfo() 
        { 
            Cells = cell,
            TopLeft = new BPoint(left, top).ToPoint(),
            FlipY = true,
            A = 255
        };
        foreach (var segment in segments)
        {
            RasterizeLine(cellInfo, segment);
        }
        return cellInfo;
    }

    private static void RasterizeLine(CellInfo cellInfo, BBezierSegment segment)
    {
        BPoint P0 = segment.P0, P1 = segment.P1, P2 = segment.P2, P3 = segment.P3;
        var left = MinFloor(P0.X, P1.X, P2.X, P3.X);
        var right = MaxCeiling(P0.X, P1.X, P2.X, P3.X);
        var bottom = MinFloor(P0.Y, P1.Y, P2.Y, P3.Y);
        var top = MaxCeiling(P0.Y, P1.Y, P2.Y, P3.Y);
        var topLeft = cellInfo.TopLeft.ToBPoint();
        for (int x = left; x < right; x++)
        {
            for (int y = bottom; y < top; y++)
            {
                var bPoint = new BPoint(x + 0.5, y + 0.5);
                var distance = segment.DistanceToPoint2(bPoint, out var _);
                
                if (distance >= MaxDistance) continue;
                var actualX = (int)(x - topLeft.X);
                var actualY = (int)(y - topLeft.Y + cellInfo.Height);
                cellInfo.Cells[actualX, actualY] = true;
            }
        }
    }

    private static int MaxCeiling(params double[] doubles)
    {
        var max = doubles[0];
        foreach (var value in doubles) 
            max = Math.Max(value, max);
        return (int)Math.Ceiling(max);
    }

    private static int MinFloor(params double[] doubles)
    {
        var min = doubles[0];
        foreach (var value in doubles)
            min = Math.Min(min, value);
        return (int)Math.Floor(min);
    }
}
