using Graph.Basic;
using Graph.Graph;
using Graph.Graph.Reference;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Command;

public class AnchorInsertOnSegmentTransform: CommandBase
{
    private readonly SegmentReference _segment;

    private readonly MyProperty _segmentProperty;

    private readonly MyProperty _anchorProperty = new();

    private readonly MyProperty _nextProperty = new();

    private readonly SegmentReference _last;

    private readonly SegmentReference _next;

    private readonly BPoint _fromPNext;

    private readonly BPoint _toPLast;

    private readonly BBezierSegment _left;

    private readonly BBezierSegment _right;

    public AnchorReference Reference { get; }

    public AnchorInsertOnSegmentTransform(SegmentReference segment, double t, BezierGraph graph)
    {
        if (t <= 0 || t >= 1)
            throw new ArgumentOutOfRangeException(nameof(t), "Insert position must be inside the segment.");

        _segment = segment;
        _segmentProperty = graph[segment];
        _fromPNext = segment.From.PNext;
        _toPLast = segment.To.PLast;

        segment.Segment.Split(t, out _left, out _right);
        Reference = new AnchorReference(_left.P3, _left.P2, _right.P1);
        _last = new SegmentReference(segment.From, Reference);
        _next = new SegmentReference(Reference, segment.To);
    }

    public override void Execute(BezierGraph graph)
    {
        graph.RemoveSegment(_segment);
        graph.ChangePosition([
            new PositionChanger(_segment.From, PositionType.PNext, _left.P1),
            new PositionChanger(_segment.To, PositionType.PLast, _right.P2)
        ]);
        graph.AddAnchorWithProperty(Reference, _anchorProperty);
        graph.AddSegmentWithProperty(_last, _segmentProperty);
        graph.AddSegmentWithProperty(_next, _nextProperty);
    }

    public override void Undo(BezierGraph graph)
    {
        graph.RemoveAnchor(Reference);
        graph.ChangePosition([
            new PositionChanger(_segment.From, PositionType.PNext, _fromPNext),
            new PositionChanger(_segment.To, PositionType.PLast, _toPLast)
        ]);
        graph.AddSegmentWithProperty(_segment, _segmentProperty);
    }
}

public class ComplexTransform(List<CommandBase> commands) : CommandBase
{
    private List<CommandBase> _commands = commands;

    public ComplexTransform() : this([]) { }

    public void Add(CommandBase command)
    {
        _commands.Add(command);
    }


    public override void Execute(BezierGraph graph)
    {
        foreach (var command in _commands) 
            command.Execute(graph);
    }

    public override void Undo(BezierGraph graph)
    {
        foreach(var command in _commands.Reverse<CommandBase>())
            command.Undo(graph);
    }
}
