using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Controls.Canvases;

public partial class CurveCanvas
{
    public static readonly DependencyProperty ScaleXProperty
        = DependencyProperty.Register(
            nameof(ScaleX), typeof(double), typeof(CurveCanvas),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ScaleYProperty
        = DependencyProperty.Register(
            nameof(ScaleY), typeof(double), typeof(CurveCanvas),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CenterProperty
        = DependencyProperty.Register(
            nameof(Center), typeof(Point), typeof(CurveCanvas),
            new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SegmentListProperty
        = DependencyProperty.Register(
            nameof(SegmentList), typeof(ObservableCollection<TracableSegment>), typeof(CurveCanvas),
            new FrameworkPropertyMetadata(null));

    public static readonly RoutedEvent SelectedUpdateEvent
        = EventManager.RegisterRoutedEvent(
            nameof(SelectedUpdate), RoutingStrategy.Bubble,
            typeof(SelectedUpdateEventHandler), typeof(CurveCanvas));

    public static readonly DependencyProperty MultipleSelectProperty
        = DependencyProperty.Register(
            nameof(MultipleSelect),
            typeof(bool),
            typeof(CurveCanvas),
            new PropertyMetadata(true, OnMultipleSelectChanged));

    public static readonly DependencyProperty EnableSelectProperty
        = DependencyProperty.Register(
            nameof(EnableSelect),
            typeof(bool),
            typeof(CurveCanvas),
            new PropertyMetadata(true, OnEnableSelectChanged));

    private static void OnEnableSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CurveCanvas canvas) return;
        canvas._manager.EnableSelect = (bool)e.NewValue;
        canvas.MouseDragSelectEnabled = canvas.EnableSelect && canvas.MultipleSelect;
    }

    private static void OnMultipleSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CurveCanvas canvas) return;
        canvas._manager.MultiSelect = (bool)e.NewValue;
        canvas.MouseDragSelectEnabled = canvas.EnableSelect && canvas.MultipleSelect;
    }
}
