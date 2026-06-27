using Graph.Basic;
using Graph.Graph;

namespace Graph.Command;

public class CenterTransform(BezierGraph graph, BPoint value) : CommandBase
{
    private readonly BPoint _oldValue = graph.Center;

    private readonly BPoint _newValue = value;

    public override void Execute(BezierGraph graph)
    {
        graph.Center = _newValue;
    }

    public override void Undo(BezierGraph graph)
    {
        graph.Center = _oldValue;
    }
}

public class RotationTransform(BezierGraph graph, double value) : CommandBase
{
    private readonly double _oldValue = graph.Rotation;

    private readonly double _newValue = value;

    public override void Execute(BezierGraph graph)
    {
        graph.Rotation = _newValue;
    }

    public override void Undo(BezierGraph graph)
    {
        graph.Rotation = _oldValue;
    }
}

public class RadiusTransform(BezierGraph graph, double value) : CommandBase
{
    private readonly double _oldValue = graph.Radius;

    private readonly double _newValue = value;

    public override void Execute(BezierGraph graph)
    {
        graph.Radius = _newValue;
    }

    public override void Undo(BezierGraph graph)
    {
        graph.Radius = _oldValue;
    }
}

public class ScaleTransform(BezierGraph graph, double value) : CommandBase
{
    private readonly double _oldValue = graph.Scale;

    private readonly double _newValue = value;

    public override void Execute(BezierGraph graph)
    {
        graph.Scale = _newValue;
    }

    public override void Undo(BezierGraph graph)
    {
        graph.Scale = _oldValue;
    }
}

public class MyPropertyTransform(MyProperty property, string key, string value) : CommandBase
{
    private readonly MyProperty _property = property;

    private readonly string _key = key;

    private readonly string _oldValue = property[key];

    private readonly string _newValue = value;

    public override void Execute(BezierGraph graph)
    {
        _property[_key] = _newValue;
    }

    public override void Undo(BezierGraph graph)
    {
        _property[_key] = _oldValue;
    }
}
