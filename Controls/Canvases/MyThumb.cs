using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Controls.Canvases;

public partial class MyThumb : Control
{
    static MyThumb()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MyThumb), new FrameworkPropertyMetadata(typeof(MyThumb)));
    }

    private Point _downPoint;
    private Vector _deltaTotal;
    private bool _isPressed;

    internal TracablePoint _point;

    public MyThumb(TracablePoint point) : base()
    {
        _point = point;
        _point.PositionChanged += PositionChanged;
        _point.SelectedChanged += Point_SelectedChanged;
        _point.SetParent(this);
    }

    public void SetPoint(TracablePoint point)
    {
        CleanUp();
        _point = point;
        _point.PositionChanged += PositionChanged;
        _point.SelectedChanged += Point_SelectedChanged;
        _point.SetParent(this);
        PositionChanged(point.X, point.Y);
    }

    private void Point_SelectedChanged(object? sender, EventArgs e)
    {
        IsSelected = _point.IsSelected;
    }

    public bool IsDragging
    {
        get => (bool)GetValue(IsDraggingProperty);
        set => SetValue(IsDraggingProperty, value);
    }

    public bool CanBeSelected => _point.CanBeSelected;

    public Point WorldPoint => _point.WorldPosition;

    public EventHandler<Point>? WorldPositionChanged;

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private void PositionChanged(double x, double y)
    {
        WorldPositionChanged?.Invoke(this, new Point(x, y));
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
        _downPoint = e.GetPosition((IInputElement)Parent);
        _isPressed = true;
        IsDragging = false;
        CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        e.Handled = true;
        if (!_isPressed) return;
        if (OutsideParent(e))
            FinishDragOrClick(
            new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
        else MouseDragging(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        e.Handled = true;
        FinishDragOrClick(e);
    }

    private void FinishDragOrClick(MouseButtonEventArgs e)
    {
        if (IsDragging)
        {
            RaiseEvent(new DragCompletedEventArgs(_deltaTotal.X, _deltaTotal.Y, false)
            {
                RoutedEvent = DragCompletedEvent
            });
            _point.OnChanged();
        }
        else if (_isPressed)
        {
            RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = ClickEvent
            });
        }
        ReleaseMouseCapture();
        IsDragging = false;
        _isPressed = false;
    }

    private void MouseDragging(MouseEventArgs e)
    {
        var p = e.GetPosition((IInputElement)Parent);

        if (!IsDragging)
        {
            var delta = p - _downPoint;
            var distance = delta.Length;
            if (distance > 10)
            {
                _deltaTotal = delta;
                _downPoint = p;
                IsDragging = true;
                RaiseEvent(new DragStartedEventArgs(delta.X, delta.Y)
                {
                    RoutedEvent = DragStartedEvent
                });
                _point.OnChanging();
            }
        }
        else
        {
            var delta = p - _downPoint;
            _deltaTotal += delta;
            _downPoint = p;
            RaiseEvent(new DragDeltaEventArgs(delta.X, delta.Y)
            {
                RoutedEvent = DragDeltaEvent
            });
        }
    }

    private bool OutsideParent(MouseEventArgs e)
    {
        return Parent is IInputElement p && !p.IsMouseOver;
    }

    public void CleanUp()
    {
        _point.PositionChanged -= PositionChanged;
        _point.SelectedChanged -= Point_SelectedChanged;
    }

    #region a
    public event MouseButtonEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public event DragStartedEventHandler DragStarted
    {
        add => AddHandler(DragStartedEvent, value);
        remove => RemoveHandler(DragStartedEvent, value);
    }

    public event DragDeltaEventHandler DragDelta
    {
        add => AddHandler(DragDeltaEvent, value);
        remove => RemoveHandler(DragDeltaEvent, value);
    }

    public event DragCompletedEventHandler DragCompleted
    {
        add => AddHandler(DragCompletedEvent, value);
        remove => RemoveHandler(DragCompletedEvent, value);
    }
    #endregion
}
