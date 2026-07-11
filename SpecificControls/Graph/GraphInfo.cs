using Graph.Graph;

namespace SpecificControls.Graph;

public class GraphInfo(GraphChanger graph, string name) : IInfo
{
    private static int UniversalId = 0;

    public GraphInfo(GraphChanger graph) : this(graph, $"Graph {UniversalId++}") { }

    public string Name => name;

    public GraphChanger Graph => graph;
}
