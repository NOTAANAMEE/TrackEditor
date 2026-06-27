using SpecificControls.Editor;
using SpecificControls.Graph;
using System.Windows;

namespace SpecificControls;

public partial class BezierEditor
{
    public static readonly DependencyProperty TargetProperty
        = DependencyProperty.Register(
            nameof(SelectedGraph), typeof(GraphInfo), typeof(BezierEditor));

    public static readonly DependencyProperty EditorStateProperty
        = DependencyProperty.Register(
            nameof(EditorState), typeof(EditorState), typeof(BezierEditor),
            new PropertyMetadata(null, OnModePropertyChanged));

    private static void OnModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BezierEditor editor) return;
        editor.SetMode();
    }

    public static readonly DependencyProperty ScaleXProperty
        = DependencyProperty.Register(
            nameof(ScaleX), typeof(double), typeof(BezierEditor),
            new PropertyMetadata(1.0));

    public static readonly DependencyProperty ScaleYProperty
        = DependencyProperty.Register(
            nameof(ScaleY), typeof(double), typeof(BezierEditor),
            new PropertyMetadata(1.0));

    public static readonly DependencyProperty CenterProperty
        = DependencyProperty.Register(
            nameof(Center), typeof(Point), typeof(BezierEditor),
            new PropertyMetadata(new Point(0, 0)));
}
