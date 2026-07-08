using SpecificControls.Graph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TrackEditor.Files;

public static class ImageSaver
{
    public static void SaveImage(IEnumerable<CellInfo> cells, string imagePath)
    {
        if (!cells.Any()) return;
        var left = cells.Min(cell => cell.TopLeft.X);
        var top = cells.Min(cell => cell.TopLeft.Y);
        var bottom = cells.Max(cell => cell.TopLeft.Y + cell.Height);
        var right = cells.Max(cell => cell.TopLeft.X + cell.Width);

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            foreach (var cell in cells.Reverse())
            {
                var curLeft = cell.TopLeft.X - left;
                var curTop = cell.TopLeft.Y - top;
                dc.DrawImage(cell.Bitmap,
                    new Rect(curLeft, curTop, cell.Width, cell.Height));
            }
        }

        var result = new RenderTargetBitmap(
            (int)(right - left),
            (int)(bottom - top),
            96,
            96,
            PixelFormats.Pbgra32);

        result.Render(visual);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(result));

        using var stream = File.Create(imagePath);
        encoder.Save(stream);
    }
}
