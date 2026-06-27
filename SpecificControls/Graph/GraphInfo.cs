using Graph.Graph;

namespace SpecificControls.Graph;

public class GraphInfo(GraphChanger graph) : IInfo
{
    private static int UniversalId = 0;

    private readonly int _id = UniversalId++;

    public string Name => $"Graph {_id}";

    public  GraphChanger Graph => graph;
}
