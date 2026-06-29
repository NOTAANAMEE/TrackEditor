using System.Collections.ObjectModel;
using System.Windows;

namespace Controls.Canvases;

public partial class PointCanvas
{
    public static readonly DependencyProperty ScaleXProperty
        = DependencyProperty.Register(
            nameof(ScaleX), typeof(double), typeof(PointCanvas),
            new PropertyMetadata(1.0, OnScaleOrCenterChanged));

    private static void OnScaleOrCenterChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not PointCanvas canvas) return;
        canvas.ForcePositionUpdate();
    }

    public static readonly DependencyProperty ScaleYProperty
        = DependencyProperty.Register(
            nameof(ScaleY), typeof(double), typeof(PointCanvas),
            new PropertyMetadata(1.0, OnScaleOrCenterChanged));

    public static readonly DependencyProperty CenterProperty
        = DependencyProperty.Register(
            nameof(Center), typeof(Point), typeof(PointCanvas),
            new PropertyMetadata(new Point(0, 0), OnScaleOrCenterChanged));

    public static readonly DependencyProperty PointListProperty
        = DependencyProperty.Register(
            nameof(PointList), typeof(ObservableCollection<TracablePoint>), typeof(PointCanvas),
            new PropertyMetadata(null));

    public static readonly RoutedEvent SelectedUpdateEvent
        = EventManager.RegisterRoutedEvent(
            nameof(SelectedUpdate), RoutingStrategy.Bubble,
            typeof(SelectedUpdateEventHandler), typeof(PointCanvas));

    public static readonly DependencyProperty MultipleSelectProperty
        = DependencyProperty.Register(
            nameof(MultipleSelect),
            typeof(bool),
            typeof(PointCanvas),
            new PropertyMetadata(true, OnMultipleSelectChanged));

    public static readonly DependencyProperty EnableSelectProperty
        = DependencyProperty.Register(
            nameof(EnableSelect),
            typeof(bool),
            typeof(PointCanvas),
            new PropertyMetadata(true, OnEnableSelectChanged));

    private static void OnEnableSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PointCanvas canvas) return;
        canvas._manager.EnableSelect = (bool)e.NewValue;
        canvas.MouseDragSelectEnabled = canvas.EnableSelect && canvas.MultipleSelect;
    }

    private static void OnMultipleSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PointCanvas canvas) return;
        canvas._manager.MultiSelect = (bool)e.NewValue;
        canvas.MouseDragSelectEnabled = canvas.EnableSelect && canvas.MultipleSelect;
    }
}
