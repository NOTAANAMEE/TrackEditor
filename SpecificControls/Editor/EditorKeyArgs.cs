using Controls;
using Graph.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SpecificControls.Editor;

public class EditorKeyArgs(
    GraphChanger graph,
    IReadOnlySet<Selectable> selectables,
    KeyEventArgs keyEventArgs
    ): EditorArgs(graph, selectables)
{
    public KeyEventArgs KeyEventArgs => keyEventArgs;
}
