using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls.Canvases;

public partial class MyCanvas : Canvas
{
    public bool MouseDragSelectEnabled
    {
        get => (bool)GetValue(MouseDragSelectEnabledProperty);
        set => SetValue(MouseDragSelectEnabledProperty, value);
    }

    public Brush SelectionBrush
    {
        get => (Brush)GetValue(SelectionBrushProperty);
        set
        {
            _pen = new(value, 1)
            {
                DashStyle = new DashStyle([4, 2], 0)
            };
            SetValue(SelectionBrushProperty, value);
        }
    }

    public Pen Pen { get => _pen; set => _pen = value; }

    private Pen _pen = new(Brushes.DeepSkyBlue, 1)
    {
        DashStyle = new DashStyle([4, 2], 0)
    };

    private static void OnSelectionBrushChanged(
           DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var canvas = (MyCanvas)d;

        canvas._pen = new Pen((Brush)e.NewValue, 1)
        {
            DashStyle = new DashStyle([4, 2], 0)
        };

        canvas._pen.Freeze();
    }

    public event EventHandler<Rect>? SelectedRect;

    public event EventHandler<MouseEventArgs>? OnClick;

    private Point _downPoint;

    private Point _currentPoint;

    private bool _isDragging = false;

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (!MouseDragSelectEnabled) return;
        _isDragging = true;
        _downPoint = e.GetPosition(this);
        _currentPoint = _downPoint;
        CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_isDragging && MouseDragSelectEnabled)
        {
            var mousePoint = e.GetPosition(this);
            _currentPoint = ClampToBounds(mousePoint);
            if (IsOutsideBounds(mousePoint))
            {
                EndSelection(false);
                return;
            }

            InvalidateVisual();
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (_isDragging)
            _currentPoint = ClampToBounds(e.GetPosition(this));

        EndSelection(true, e);
    }

    private void EndSelection(bool raiseClick, MouseEventArgs? e = null)
    {
        if (IsMouseCaptured)
            ReleaseMouseCapture();

        var distance = (_currentPoint - _downPoint).Length;
        if (_isDragging && distance >= 5)
        {
            if (MouseDragSelectEnabled)
            {
                SelectedRect?.Invoke(this, new Rect(_currentPoint, _downPoint));
                InvalidateVisual();
            }
        }
        else if (raiseClick && e != null)
        {
            OnClick?.Invoke(this, e);
        }

        _isDragging = false;
    }

    private bool IsOutsideBounds(Point point)
        => point.X < 0
            || point.Y < 0
            || point.X > ActualWidth
            || point.Y > ActualHeight;

    private Point ClampToBounds(Point point)
        => new(
            Math.Clamp(point.X, 0, ActualWidth),
            Math.Clamp(point.Y, 0, ActualHeight));

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var distance = (_currentPoint - _downPoint).Length;
        if (!MouseDragSelectEnabled || !_isDragging || distance < 5)
            return;

        dc.DrawRectangle(
            null,
            _pen,
            new Rect(_currentPoint, _downPoint));
    }
}
