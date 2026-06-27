using SpecificControls.Graph;
using System.Windows;

namespace SpecificControls.Canvases;

public partial class BoolGridCanvas
{
    public static readonly DependencyProperty CellInfoProperty
        = DependencyProperty.Register(
            nameof(CellInfo), typeof(CellInfo), typeof(BoolGridCanvas),
            new FrameworkPropertyMetadata(null, 
                FrameworkPropertyMetadataOptions.AffectsRender, CellInfoPropertyChanged));

    public static readonly DependencyProperty ScaleXProperty
        = DependencyProperty.Register(
            nameof(ScaleX), typeof(double), typeof(BoolGridCanvas),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ScaleYProperty
        = DependencyProperty.Register(
            nameof(ScaleY), typeof(double), typeof(BoolGridCanvas),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CenterProperty
        = DependencyProperty.Register(
            nameof(Center), typeof(Point), typeof(BoolGridCanvas),
            new FrameworkPropertyMetadata(new Point(0, 0),
                FrameworkPropertyMetadataOptions.AffectsRender
                ));

    private static void CellInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoolGridCanvas canvas) return;
        canvas.SetBitmap();
    }
}
