using Controls.Selection;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls.Canvases;

public partial class PointCanvas : MyCanvas
{
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

    private SelectionManager<TracablePoint> _manager = new();

    public IReadOnlySet<TracablePoint> SelectedPoints => _manager.Selected;

    public ObservableCollection<TracablePoint> PointList
    {
        get => (ObservableCollection<TracablePoint>)GetValue(PointListProperty);
        private set => SetValue(PointListProperty, value);
    }

    public event SelectedUpdateEventHandler SelectedUpdate
    {
        add => AddHandler(SelectedUpdateEvent, value);
        remove => RemoveHandler(SelectedUpdateEvent, value);
    }

    /// <summary>
    /// This action is for UI. X and Y delta are screen positions
    /// </summary>
    protected Action<double, double>? PreviewPositionChange;

    public PointCanvas() : base()
    {
        PointList = [];
        PointList.CollectionChanged += PointList_CollectionChanged;
        SelectedRect += PointCanvas_SelectedRect;
        OnClick += (s, e) => ClearSelectedThumbs();
        _manager.SelectionChanged += Manager_SelectionChanged;
        SizeChanged += (s, e) => ForcePositionUpdate();
        IsVisibleChanged += (s, e) => ForcePositionUpdate();
    }


    private void ForcePositionUpdate()
    {
        foreach (var thumb in Children.OfType<MyThumb>())
        {
            Thumb_WorldPositionChanged(thumb, thumb._point.WorldPosition);
        }
    }

    private void RemoveThumbs(IList? points)
    {
        if (points == null) return;
        foreach (var point in points)
        {
            if (point is not TracablePoint tracablePoint) continue;
            RemoveThumb(tracablePoint);
        }
    }

    protected void RemoveThumb(TracablePoint point)
    {
        if (point._parent == null) return;
        var thumb = point._parent;
        CleanUpThumb(thumb);
        _manager.Remove(point);
        Children.Remove(thumb);
    }

    private void RemoveAllThumbs()
    {
        var thumbsToRemove = new List<MyThumb>();
        foreach (var control in Children)
        {
            if (control is not MyThumb thumb) continue;
            CleanUpThumb(thumb);
            thumbsToRemove.Add(thumb);
        }
        thumbsToRemove.ForEach(Children.Remove);
        _manager.Clear();
    }

    private void AddThumbs(IList? points)
    {
        if (points == null) return;
        foreach (var element in points)
        {
            if (element is not TracablePoint point) continue;
            AddThumb(point);
        }
    }

    protected void AddThumb(TracablePoint point)
    {
        var thumb = new MyThumb(point)
        {
            Width = 5,
            Height = 5,
            Background = Brushes.Red,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };
        thumb.Click                += Thumb_Click;
        thumb.DragDelta            += Thumb_DragDelta;
        thumb.WorldPositionChanged += Thumb_WorldPositionChanged;
        thumb.DragCompleted        += Thumb_DragCompleted;
        thumb.DragStarted          += Thumb_DragStarted;
        Children.Add(thumb);
        Thumb_WorldPositionChanged(thumb, thumb._point.WorldPosition);
    }

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        if (sender is not MyThumb thumb) return;
        if (!thumb._point.CanBeSelected)// No selection change
        {
            var x = GetLeft(thumb) + e.HorizontalOffset;
            var y = GetTop(thumb) + e.VerticalOffset;
            SetLeft(thumb, x);
            SetTop(thumb, y);
            return;
        }

        if (!thumb._point.IsSelected) // Selection change
            SetSingleSelectedThumb(thumb);
        foreach (var point in SelectedPoints)
        {
            // A tricky way. Control changes before the actual data change
            var parent = point._parent;
            var x = GetLeft(parent) + e.HorizontalOffset;
            var y = GetTop(parent) + e.VerticalOffset;
            SetLeft(parent, x);
            SetTop(parent, y);
        }
        PreviewPositionChange?.Invoke(e.HorizontalOffset, e.VerticalOffset);
    }

    private void CleanUpThumb(MyThumb thumb)
    {
        thumb.CleanUp();
        thumb.Click -= Thumb_Click;
        thumb.DragDelta -= Thumb_DragDelta;
        thumb.WorldPositionChanged -= Thumb_WorldPositionChanged;
        thumb.DragCompleted -= Thumb_DragCompleted;
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (sender is not MyThumb thumb) return;
        var x = GetLeft(thumb);
        var y = GetTop(thumb);
        var p = thumb._point;
        if (!thumb.CanBeSelected)
        {
            thumb._point.Move(
                new(e.HorizontalChange / ScaleX,
                    e.VerticalChange / ScaleY));
            return;
        }
        SelectedPoints.First()
            .MoveMultiple(SelectedPoints,
                new(e.HorizontalChange / ScaleX,
                    e.VerticalChange / ScaleY));
    }

    private void Thumb_WorldPositionChanged(object? sender, Point e)
    {
        if (sender is not MyThumb thumb) return;
        var pos = WorldToCanvasPoint(e);
        if (thumb._point.Tracing)
        {
            Debug.Write($"Point:{e}, ");
            Debug.Write($"pos2: {thumb._point.WorldPosition}, pos3:");
            Debug.WriteLine(pos.ToString());
        }
        SetLeft(thumb, pos.X - thumb.Width / 2);
        SetTop(thumb, pos.Y - thumb.Height / 2);
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (sender is not MyThumb thumb) return;
        if (!thumb._point.CanBeSelected)// No selection change
        {
            var x = GetLeft(thumb) + e.HorizontalChange;
            var y = GetTop(thumb) + e.VerticalChange;
            SetLeft(thumb, x);
            SetTop(thumb, y);
            return;
        }
        foreach (var point in SelectedPoints)
        {
            // A tricky way. Control changes before the actual data change
            var parent = point._parent;
            var x = GetLeft(parent) + e.HorizontalChange;
            var y = GetTop(parent) + e.VerticalChange;
            SetLeft(parent, x);
            SetTop(parent, y);
        }
        PreviewPositionChange?.Invoke(e.HorizontalChange, e.VerticalChange);
    }

    protected void HelpMoveThumb(TracablePoint point, double deltaX, double deltaY)
    {
        var parent = point._parent;
        var x = GetLeft(parent) + deltaX;
        var y = GetTop(parent) + deltaY;
        SetLeft(parent, x);
        SetTop(parent, y);
    }

    #region SELECT

    private void Manager_SelectionChanged(object? sender, EventArgs e)
    {
        RaiseEvent(new SelectedUpdateEventArgs([.. SelectedPoints])
        {
            RoutedEvent = SelectedUpdateEvent
        });
    }

    private void PointCanvas_SelectedRect(object? sender, Rect e)
    {
        var selectedThumbs = new List<MyThumb>();
        foreach (var control in Children)
        {
            if (control is not MyThumb thumb) continue;

            if (e.Contains(GetLeft(thumb), GetTop(thumb)))
                selectedThumbs.Add(thumb);
        }
        SetSelectedThumbs(selectedThumbs);
    }

    private void PointList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                RemoveThumbs(e.OldItems);
                break;

            case NotifyCollectionChangedAction.Replace:
                RemoveThumbs(e.OldItems);
                AddThumbs(e.NewItems);
                break;

            case NotifyCollectionChangedAction.Add:
                AddThumbs(e.NewItems);
                break;

            case NotifyCollectionChangedAction.Reset:
                RemoveAllThumbs();
                AddThumbs(PointList);
                break;
        }
    }

    private void Thumb_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not MyThumb thumb) return;
        if (MultipleSelect && (Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            UpdateSelectedThumb(thumb);
        else
            SetSingleSelectedThumb(thumb);
    }

    private void SetSingleSelectedThumb(MyThumb thumb)
    {
        _manager.SetSingleSelected(thumb._point);
    }

    private void UpdateSelectedThumb(MyThumb thumb)
    {
        _manager.UpdateOneSelected(thumb._point);
    }

    private void SetSelectedThumbs(IEnumerable<MyThumb> thumbs)
    {
        _manager.SetMultiSelected(thumbs.Select(x => x._point));
    }

    protected void ClearSelectedThumbs()
    {
        _manager.Clear();
    }
    #endregion

    private Point WorldToCanvasPoint(Point worldPos)
    {
        var canvasCenterX = ActualWidth / 2;
        var canvasCenterY = ActualHeight / 2;
        var x = (worldPos.X - Center.X) * ScaleX + canvasCenterX;
        var y = (worldPos.Y - Center.Y) * ScaleY + canvasCenterY;
        return new Point(x, y);
    }

    private double WorldToCanvasX(double x)
    {
        var canvasCenterX = ActualWidth / 2;
        return (x - Center.X) * ScaleX + canvasCenterX;
    }

    private double WorldToCanvasY(double y)
    {
        var canvasCenterY = ActualHeight / 2;
        return (y - Center.Y) * ScaleY + canvasCenterY;
    }
}


