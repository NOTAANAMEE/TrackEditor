using Controls.Selection;
using Graph.Basic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls.Canvases;

public partial class CurveCanvas : MyCanvas
{
    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

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

    public event SelectedUpdateEventHandler SelectedUpdate
    {
        add => AddHandler(SelectedUpdateEvent, value);
        remove => RemoveHandler(SelectedUpdateEvent, value);
    }

    public bool MultipleSelect
    {
        get => (bool)GetValue(MultipleSelectProperty);
        set => SetValue(MultipleSelectProperty, value);
    }

    public bool EnableSelect
    {
        get => (bool)GetValue(EnableSelectProperty);
        set => SetValue(EnableSelectProperty, value);
    }

    private readonly SelectionManager<TracableSegment> _manager = new();

    public IReadOnlySet<TracableSegment> SelectedSegments => _manager.Selected;

    public ObservableCollection<TracableSegment> SegmentList
    {
        get => (ObservableCollection<TracableSegment>)GetValue(SegmentListProperty);
        private set => SetValue(SegmentListProperty, value);
    }

    public CurveCanvas()
    {
        SegmentList = [];
        SegmentList.CollectionChanged += SegmentList_CollectionChanged;
        OnClick += CurveCanvas_OnClick;
        SelectedRect += CurveCanvas_SelectedRect;
        IsVisibleChanged += CurveCanvas_IsVisibleChanged;
        _manager.SelectionChanged += Manager_SelectionChanged;
    }

    private void SegmentList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset) _manager.Clear();

        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is TracableSegment segment)
                {
                    segment.ControlPointChanged -= Segment_ControlPointChanged;
                    _manager.Remove(segment);
                }
            }

        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is TracableSegment segment)
                    segment.ControlPointChanged += Segment_ControlPointChanged;
            }
        }

        InvalidateVisual();
    }

    private void Segment_ControlPointChanged()
    {
        InvalidateVisual();
    }

    private void Manager_SelectionChanged(object? sender, EventArgs e)
    {
        RaiseEvent(new SelectedUpdateEventArgs([.. _manager.Selected])
        {
            RoutedEvent = SelectedUpdateEvent
        });
        InvalidateVisual();
    }

    private void CurveCanvas_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _manager.Clear();
    }

    private void CurveCanvas_SelectedRect(object? sender, Rect e)
    {
        List<TracableSegment> segments = [];

        foreach (var reference in SegmentList)
        {
            var p1 = WorldToCanvasPoint(reference.P0);
            var p2 = WorldToCanvasPoint(reference.P3);
            if (e.Contains(p1) && e.Contains(p2)) segments.Add(reference);
        }
        _manager.SetMultiSelected(segments);
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        //dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

        foreach (var segment in SegmentList)
        {
            DrawSegment(dc, segment, new Pen(Brushes.Black, 1));
        }

        foreach (var segment in SelectedSegments)
        {
            DrawSegment(dc, segment, new Pen(Brushes.Orange, 1));
        }
    }

    private void DrawSegment(DrawingContext dc, TracableSegment segment, Pen pen)
    {
        var geo = new StreamGeometry();

        using (var ctx = geo.Open())
        {
            ctx.BeginFigure(WorldToCanvasPoint(segment.P0), false, false);

            ctx.BezierTo(
                WorldToCanvasPoint(segment.P1),
                WorldToCanvasPoint(segment.P2),
                WorldToCanvasPoint(segment.P3),
                true,
                false);
        }

        geo.Freeze();
        dc.DrawGeometry(null, pen, geo);
    }

    private void CurveCanvas_OnClick(object? sender, MouseEventArgs e)
    {
        var closestSegment = ClosestSegment(e.GetPosition(this));
        if (closestSegment == null)
        {
            ClearSelectedSegments();
            return;
        }
        var multi = MultipleSelect && (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
        if (multi)
        {
            UpdateSelectedSegment(closestSegment);
            if (!closestSegment.IsSelected)
                _manager.AddMultiSelected(GetSegments(closestSegment));
        }
            
            
        else if (closestSegment.IsSelected)
        {
            _manager.SetMultiSelected(GetSegments(closestSegment));
        }
        else SetSelectedSegment(closestSegment);
    }

    private TracableSegment? ClosestSegment(Point mousePos)
    {
        var worldPos = CanvasToWorldPoint(mousePos);
        var bPoint = worldPos.ToBPoint();
        TracableSegment? closestSegment = null;
        var distance = double.PositiveInfinity;
        var t = 0.0;
        foreach (var bezierSegment in SegmentList)
        {
            var d = CurveToPoint(bezierSegment.Segment, bPoint, out var t2);
            if (d >= distance) continue;
            distance = d;
            closestSegment = bezierSegment;
            t = t2;
        }
        if (distance > 2) return null;
        return closestSegment;
    }

    private static double CurveToPoint(BBezierSegment segment, BPoint point, out double t)
    {
        return segment.DistanceToPoint2(point, out t);
    }

    private void ClearSelectedSegments()
    {
        _manager.Clear();
    }

    private void SetSelectedSegment(TracableSegment segment)
    {
        _manager.SetSingleSelected(segment);
    }

    private void UpdateSelectedSegment(TracableSegment segment)
    {
        _manager.UpdateOneSelected(segment);
    }

    private Point WorldToCanvasPoint(Point worldPos)
    {
        var canvasCenterX = ActualWidth / 2;
        var canvasCenterY = ActualHeight / 2;
        var x = (worldPos.X - Center.X) * ScaleX + canvasCenterX;
        var y = (worldPos.Y - Center.Y) * ScaleY + canvasCenterY;
        return new Point(x, y);
    }

    private Point CanvasToWorldPoint(Point canvasPos)
    {
        var canvasCenterX = ActualWidth / 2;
        var canvasCenterY = ActualHeight / 2;
        var x = (canvasPos.X - canvasCenterX) / ScaleX + Center.X;
        var y = (canvasPos.Y - canvasCenterY) / ScaleY + Center.Y;
        return new Point(x, y);
    }

    protected virtual IEnumerable<TracableSegment> GetSegments(TracableSegment start)
    {
        return [start];
    }
}


