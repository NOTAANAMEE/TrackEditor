using SpecificControls.Graph;
using System.Windows;

namespace SpecificControls.Merge;

public static class CellMergeHelper
{
    public static CellInfo Merge(CellInfo info1, CellInfo info2)
    {
        var left   = (int)double.Min(info1.TopLeft.X, info2.TopLeft.X);
        var top    = (int)double.Min(info1.TopLeft.Y, info2.TopLeft.Y);
        var right  = (int)double.Max(info1.TopLeft.X + info1.Width, info2.TopLeft.X + info2.Width);
        var bottom = (int)double.Max(info1.TopLeft.Y + info1.Height, info2.TopLeft.Y + info2.Height);
        var cell   = new bool[right - left, bottom - top];
        for (var x = left; x < right; x++)
        {
            for (var y = top; y < bottom; y++)
            {
                cell[x - left, y - top] = 
                    GetInfo(info1, new Point(x, y)) || GetInfo(info2, new Point(x, y));
            }
        }
        return new CellInfo() 
        { 
            Cells = cell, 
            FlipY = false, 
            TopLeft = new Point(left, top),
            R = info1.R,
            G = info1.G,
            B = info1.B,
            A = info1.A,
        };
    }

    public static CellInfo MergeCells(params CellInfo[] cells)
    {
        var info1  = cells.First();
        var left   = cells.Min(a => (int)a.TopLeft.X);
        var top    = cells.Min(a => (int)a.TopLeft.Y);
        var right  = cells.Max(a => (int)(a.TopLeft.X + a.Width));
        var bottom = cells.Max(a => (int)(a.TopLeft.Y + a.Height));
        var cell = new bool[right - left, bottom - top];
        for (var x = left; x < right; x++)
        {
            for (var y = top; y < bottom; y++)
            {
                cell[x - left, y - top] =
                    cells.Any(a => GetInfo(a, new Point(x, y)));
            }
        }
        return new CellInfo()
        {
            Cells = cell,
            FlipY = false,
            TopLeft = new Point(left, top),
            R = info1.R,
            G = info1.G,
            B = info1.B,
            A = info1.A,
        };
    }

    private static bool GetInfo(CellInfo info, Point point)
    {
        int x = (int)point.X;
        int y = (int)point.Y;
        var height = info.Height;
        var actualX = (int)(x - info.TopLeft.X);
        var actualY = (int)(y - info.TopLeft.Y);
        if (info.FlipY) actualY = height - actualY - 1;
        if (actualX < 0 || actualX >= info.Width
            || actualY < 0 || actualY >= height) return false;
        return info.Cells[actualX, actualY];
    }
}
