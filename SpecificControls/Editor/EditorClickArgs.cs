using Controls;
using Graph.Graph;
using System.Windows;

namespace SpecificControls.Editor;

public class EditorClickArgs(
    GraphChanger graph,
    IEnumerable<Selectable> selectables, 
    Point worldPos): 
    EditorArgs(graph, selectables)
{
    public readonly Point WorldPosition = worldPos;
}