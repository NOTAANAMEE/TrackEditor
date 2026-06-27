using SpecificControls.Graph;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SpecificControls.Canvases;

public partial class BoolGridCanvas : Canvas
{
    public CellInfo? CellInfo
    {
        get => (CellInfo?)GetValue(CellInfoProperty);
        set => SetValue(CellInfoProperty, value);
    }

    private WriteableBitmap? _bitmap;

    public double ScaleX
    {
        get => (double)GetValue(ScaleXProperty);
        set => SetValue(ScaleXProperty, value);
    }

    public double ScaleY
    {
        get => (double)GetValue(ScaleYProperty);
        set => SetValue(ScaleYProperty, value);
    }

    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public BoolGridCanvas()
    {
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
    }

    private void SetBitmap()
    {
        if (CellInfo != null) _bitmap = CellInfo.ToBitmap();
        else _bitmap = null;
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        if (_bitmap == null || CellInfo == null) return;
        var width = CellInfo.Width;
        var height = CellInfo.Height;
        var topLeft = CellInfo.TopLeft;
        var p0 = WorldToCanvasPoint(topLeft);
        var p1 = WorldToCanvasPoint(new Point(topLeft.X + width, topLeft.Y + height));
        //dc.PushOpacity(Opacity);
        dc.DrawImage(_bitmap, new Rect(p0, p1));
    }

    private Point WorldToCanvasPoint(Point worldPos)
    {
        var canvasCenterX = ActualWidth / 2;
        var canvasCenterY = ActualHeight / 2;
        var x = (worldPos.X - Center.X) * ScaleX + canvasCenterX;
        var y = (worldPos.Y - Center.Y) * ScaleY + canvasCenterY;
        return new Point(x, y);
    }
}
