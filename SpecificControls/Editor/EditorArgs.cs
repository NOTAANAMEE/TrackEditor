using Controls;
using Graph.Graph;

namespace SpecificControls.Editor;

public class EditorArgs(GraphChanger graph, IEnumerable<Selectable> selectables): EventArgs
{
    public readonly GraphChanger Graph = graph;

    public readonly IEnumerable<Selectable> Selectables = selectables;
}
