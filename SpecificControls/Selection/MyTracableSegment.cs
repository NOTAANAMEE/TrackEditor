using Controls;
using Graph.Basic;
using Graph.Graph;
using Graph.Graph.Reference;
using System.Windows;

namespace SpecificControls.Selection;

public class MyTracableSegment : TracableSegment, IDisposable
{
    internal SegmentReference _segment;

    internal GraphChanger _graph;

    public SegmentReference SegmentReference => _segment;

    public override string? SelectionGroup => "SSSSS";

    public override Point P0 => _segment.From.Position.ToPoint();

    public override Point P1 => _segment.From.PNext.ToPoint();

    public override Point P2 => _segment.To.PLast.ToPoint();

    public override Point P3 => _segment.To.Position.ToPoint();

    public MyTracableSegment(SegmentReference segment, GraphChanger graph)
    {
        _segment = segment;
        _graph = graph;
        _graph.Graph.AnchorPositionChanged += Graph_AnchorPositionChanged;
        _graph.Graph.RotationChanged += Graph_RotationChanged;
        _graph.Graph.CenterChanged += Graph_CenterChanged;
        _graph.Graph.ScaleChanged += Graph_ScaleChanged;
    }

    private void Graph_ScaleChanged(object? sender, double e)
    {
        ControlPointChanged?.Invoke();
    }

    private void Graph_CenterChanged(object? sender, BPoint e)
    {
        ControlPointChanged?.Invoke();
    }

    private void Graph_RotationChanged(object? sender, double e)
    {
        ControlPointChanged?.Invoke();
    }

    private void Graph_AnchorPositionChanged(object? sender, HashSet<AnchorReference> e)
    {
        if (e.Contains(_segment.From) || e.Contains(_segment.To))
        {
            ControlPointChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _graph.Graph.AnchorPositionChanged -= Graph_AnchorPositionChanged;
        _graph.Graph.RotationChanged -= Graph_RotationChanged;
        _graph.Graph.CenterChanged -= Graph_CenterChanged;
        _graph.Graph.ScaleChanged -= Graph_ScaleChanged;
    }
}
