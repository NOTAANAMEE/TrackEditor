using Controls;
using Graph.Basic;
using Graph.Command;
using Graph.Graph;
using Graph.Graph.Reference;
using System.Security.Cryptography.Xml;
using System.Windows;

namespace SpecificControls.Selection;

public class MyTracablePoint : TracablePoint, IDisposable
{
    internal AnchorReference _anchor;

    internal GraphChanger _graph;

    private PositionType _type;

    private Func<bool>? _movable;

    public override string? SelectionGroup => GetSelectionGroup();

    public override Point WorldPosition => GetPosition();

    public AnchorReference Anchor => _anchor;

    public MyTracablePoint(
        AnchorReference anchor, GraphChanger graph, PositionType type,
        Func<bool> getMovable)
    {
        _anchor = anchor;
        _graph = graph;
        _graph.Graph.AnchorPositionChanged += Graph_AnchorPositionChanged;
        _graph.Graph.RotationChanged += Graph_RotationChanged;
        _graph.Graph.CenterChanged += Graph_CenterChanged;
        _graph.Graph.ScaleChanged += Graph_ScaleChanged;
        _type = type;
        _movable = getMovable;
    }

    private void Graph_ScaleChanged(object? sender, double e)
    {
        PositionChanged?.Invoke(WorldPosition.X, WorldPosition.Y);
    }

    private void Graph_CenterChanged(object? sender, BPoint e)
    {
        PositionChanged?.Invoke(WorldPosition.X, WorldPosition.Y);
    }

    private void Graph_RotationChanged(object? sender, double e)
    {
        PositionChanged?.Invoke(WorldPosition.X, WorldPosition.Y);
    }

    private void Graph_AnchorPositionChanged(object? sender, HashSet<AnchorReference> e)
    {
        if (e.Contains(_anchor))
            PositionChanged?.Invoke(WorldPosition.X, WorldPosition.Y);
    }

    private Point GetPosition()
    {
        return _type switch
        {
            PositionType.Position => _anchor.Position.ToPoint(),
            PositionType.PLast => _anchor.PLast.ToPoint(),
            _ => _anchor.PNext.ToPoint(),
        };
    }

    private string? GetSelectionGroup()
    {
        return _type switch
        {
            PositionType.Position => "MOVABLEANCHOR",
            _ => null,
        };
    }

    public override void Move(Vector delta)
    {
        if (_movable == null || !_movable()) return;
        var point = delta + GetPosition();
        var transform = new PointTransform();
        transform.Add(new PositionChanger(_anchor, _type, point.ToBPoint()));
        _graph.Execute(transform);
    }

    public override void MoveMultiple(IEnumerable<TracablePoint> points, Vector delta)
    {
        if (_movable == null || !_movable()) return;
        var d = ((Point)delta).ToBPoint();
        var transform = new PointTransform();
        foreach (var point in points)
        {
            if (point is not MyTracablePoint myPoint) continue;
            var pos = myPoint._anchor.Position;
            transform.Add(new PositionChanger(myPoint._anchor, myPoint._type, d + pos));
        }
        _graph.Execute(transform);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _graph.Graph.AnchorPositionChanged -= Graph_AnchorPositionChanged;
        _graph.Graph.RotationChanged -= Graph_RotationChanged;
        _graph.Graph.CenterChanged -= Graph_CenterChanged;
        _graph.Graph.ScaleChanged -= Graph_ScaleChanged;
        _movable = null;
    }
}
