using System.Windows;
using System.Windows.Media;

namespace Controls.Canvases;

public partial class MyCanvas
{
    public static readonly DependencyProperty MouseDragSelectEnabledProperty
        = DependencyProperty.Register(
            nameof(MouseDragSelectEnabled),
            typeof(bool),
            typeof(MyCanvas),
            new PropertyMetadata(true));

    public static readonly DependencyProperty SelectionBrushProperty =
        DependencyProperty.Register(
            nameof(SelectionBrush), typeof(Brush), typeof(MyCanvas),
            new FrameworkPropertyMetadata(
                Brushes.DeepSkyBlue,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnSelectionBrushChanged));
}
