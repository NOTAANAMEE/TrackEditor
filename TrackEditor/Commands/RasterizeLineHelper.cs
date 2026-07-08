using Controls;
using Graph.Basic;
using SpecificControls.Graph;
using System.Windows.Media;

namespace TrackEditor.Commands;

public class RasterizeLineHelper
{
    private static readonly double MaxDistance = 0.5 * 0.5;

    public static CellInfo RasterizeLines(
        IEnumerable<BBezierSegment> segments,
        Color color)
    {
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
        var cells = new bool[right - left, top - bottom];
        
        foreach (var segment in segments)
            RasterizeLine(cells, new BPoint(left, top), segment);
        

        var cellInfo = new CellInfo()
        {
            Bitmap = CellInfo.ToBitmap(cells, true, color.R, color.G, color.B, color.A),
            TopLeft = new BPoint(left, top).ToPoint(),
        };
        return cellInfo;
    }

    private static void RasterizeLine(bool[,] cells, BPoint topLeft, BBezierSegment segment)
    {
        BPoint P0 = segment.P0, P1 = segment.P1, P2 = segment.P2, P3 = segment.P3;
        var left = MinFloor(P0.X, P1.X, P2.X, P3.X);
        var right = MaxCeiling(P0.X, P1.X, P2.X, P3.X);
        var bottom = MinFloor(P0.Y, P1.Y, P2.Y, P3.Y);
        var top = MaxCeiling(P0.Y, P1.Y, P2.Y, P3.Y);
        for (int x = left; x < right; x++)
        {
            for (int y = bottom; y < top; y++)
            {
                var bPoint = new BPoint(x + 0.5, y + 0.5);
                var distance = segment.DistanceToPoint2(bPoint, out var _);
                
                if (distance >= MaxDistance) continue;
                var actualX = (int)(x - topLeft.X);
                var actualY = (int)(y - topLeft.Y + cells.GetLength(1));
                cells[actualX, actualY] = true;
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
