using SpecificControls.Graph;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpecificControls.Merge;

public static class CellMergeHelper
{
    public static CellInfo MergeCells(params CellInfo[] cells)
    {
        var info1 = cells.First();
        var left = cells.Min(a => (int)a.TopLeft.X);
        var top = cells.Min(a => (int)a.TopLeft.Y);
        var right = cells.Max(a => (int)(a.TopLeft.X + a.Width));
        var bottom = cells.Max(a => (int)(a.TopLeft.Y + a.Height));

        var visual = new DrawingVisual();

        using (var dc = visual.RenderOpen())
        {
            foreach (var cell in cells.Reverse())
            {
                var rect = new Rect(cell.TopLeft.X - left, cell.TopLeft.Y - top, cell.Width, cell.Height);
                dc.DrawImage(cell.Bitmap, rect);
            }
        }

        var bitmap = GenBitmap(right - left, bottom - top);
        bitmap.Render(visual);
        bitmap.Freeze();

        return new()
        {
            Bitmap = bitmap,
            TopLeft = new Point(left, top),
        };
    }

    private static RenderTargetBitmap GenBitmap(int width, int height)
    {
        var bitmap = new RenderTargetBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Pbgra32);
        return bitmap;
    }
}
