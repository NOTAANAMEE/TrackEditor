using Graph.Graph;
using Graph.Graph.Reference;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Graph.Command;

public class AnchorAddTransform(AnchorReference reference): CommandBase
{
    private readonly MyProperty _property = new();

    public override void Execute(BezierGraph graph)
    {
        graph.AddAnchorWithProperty(reference, _property);
    }

    public override void Undo(BezierGraph graph)
    {
        graph.RemoveAnchor(reference);
    }
}

public class AnchorsAddTransform(IEnumerable<AnchorReference> references) : CommandBase
{
    private readonly MyProperty[] _properties = [.. references.Select(a => new MyProperty())];
    private readonly AnchorReference[] _references = [..references];

    public override void Execute(BezierGraph graph)
    {
        var i = 0;
        foreach (var reference in _references)
            graph.AddAnchorWithProperty(reference, _properties[i++]);
    }

    public override void Undo(BezierGraph graph)
    {
        foreach(AnchorReference reference in _references)
            graph.RemoveAnchor(reference);
    }
}

public class AnchorsAddTransformWithProperty(IEnumerable<AnchorReference> references,
    IEnumerable<MyProperty> properties) : CommandBase
{
    private readonly MyProperty[] _properties = [.. properties];
    private readonly AnchorReference[] _references = [.. references];

    public override void Execute(BezierGraph graph)
    {
        var i = 0;
        foreach (var reference in _references)
            graph.AddAnchorWithProperty(reference, _properties[i++]);
    }

    public override void Undo(BezierGraph graph)
    {
        foreach (AnchorReference reference in _references)
            graph.RemoveAnchor(reference);
    }
}


public class SimpleAnchorsRemoveTransform(
    IEnumerable<AnchorReference> anchors, BezierGraph graph): CommandBase
{
    private readonly AnchorReference[] _anchors = [..anchors];
    private readonly MyProperty[] _properties = [..anchors.Select(a => graph[a])];

    public override void Execute(BezierGraph graph)
    {
        foreach (var anchor in _anchors)
            graph.RemoveAnchor(anchor);
    }

    public override void Undo(BezierGraph graph)
    {
        int ind = 0;
        foreach (var anchor in _anchors)
            graph.AddAnchorWithProperty(anchor, _properties[ind++]);
    }
}

public class SegmentAddTransform(SegmentReference reference, MyProperty property): CommandBase
{
    private readonly MyProperty _property = property;

    public SegmentAddTransform(SegmentReference reference) : this(reference, new()){ }

    public override void Execute(BezierGraph graph)
    {
        graph.AddSegmentWithProperty(reference, _property);
    }

    public override void Undo(BezierGraph graph)
    {
        graph.RemoveSegment(reference);
    }
}

public class SegmentsAddTransform(IEnumerable<SegmentReference> references): CommandBase
{
    private readonly List<MyProperty> _properties = [.. references.Select(a => new MyProperty())];

    private readonly SegmentReference[] _segments = [.. references];

    public override void Execute(BezierGraph graph)
    {
        var i = 0;
        foreach (var reference in _segments)
            graph.AddSegmentWithProperty(reference, _properties[i++]);
    }

    public override void Undo(BezierGraph graph)
    {
        foreach (var reference in _segments)
            graph.RemoveSegment(reference);
    }
}

public class SegmentsAddPropertyTransform(
    IEnumerable<SegmentReference> references,
    IEnumerable<MyProperty> properties
    ) : CommandBase
{
    private readonly MyProperty[] _properties = [.. properties];

    private readonly SegmentReference[] _segments = [.. references];

    public override void Execute(BezierGraph graph)
    {
        var i = 0;
        foreach (var reference in _segments)
            graph.AddSegmentWithProperty(reference, _properties[i++]);
    }

    public override void Undo(BezierGraph graph)
    {
        foreach (var reference in _segments)
            graph.RemoveSegment(reference);
    }
}

public class SegmentRemoveTransform(SegmentReference reference, BezierGraph graph) : CommandBase
{
    private readonly MyProperty _property = graph[reference];

    public override void Execute(BezierGraph graph)
    {
        graph.RemoveSegment(reference);
    }

    public override void Undo(BezierGraph graph)
    {
        graph.AddSegmentWithProperty(reference, _property);
    }
}

public class SegmentsRemoveTransform(IEnumerable<SegmentReference> references, BezierGraph graph) : CommandBase
{
    private readonly MyProperty[] _property = [.. references.Select(a => graph[a])];
    private readonly SegmentReference[] _segments = [.. references];

    public override void Execute(BezierGraph graph)
    {
        foreach (var reference in _segments)
            graph.RemoveSegment(reference);
    }

    public override void Undo(BezierGraph graph)
    {
        int i = 0;
        foreach (var reference in _segments)
            graph.AddSegmentWithProperty(reference, _property[i++]);
    }
}
