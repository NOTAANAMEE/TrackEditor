using Controls.Canvases;
using System.Windows;

namespace Controls;

public abstract class TracablePoint: Selectable
{
    public Action<double, double>? PositionChanged;

    internal MyThumb? _parent;

    public virtual double X { get; }

    public virtual double Y { get; }

    public bool Tracing = false;

    internal void SetParent(MyThumb parent)
    {
        _parent = parent;
    }

    public abstract Point WorldPosition { get; }

    public abstract void Move(Vector delta);

    public abstract void MoveMultiple(IEnumerable<TracablePoint> points, Vector delta);

    public virtual void OnChanging() { }

    public virtual void OnChanged() { }
}
