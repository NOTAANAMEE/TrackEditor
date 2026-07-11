using Graph.Graph;

namespace SpecificControls.Graph;

public class GraphInfo2(BezierGraph graph, string name) : IInfo
{
    private static int UniversalId = 0;

    public GraphInfo2(BezierGraph graph) : this(graph, $"Graph {UniversalId++}") { }

    public string Name => name;

    public BezierGraph Graph => graph;
}
