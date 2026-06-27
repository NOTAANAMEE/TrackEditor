using Controls;
using Graph.Graph;

namespace SpecificControls.Editor;

public class EditorSignalArgs
    (GraphChanger graph,
    IReadOnlySet<Selectable> selectables,
    string signalName, object? parameters): EditorArgs(graph, selectables)
{
    public readonly string SignalName = signalName;

    public object? Parameters => parameters;
}
