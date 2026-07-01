using Graph.Command;
using SpecificControls.Graph;
using SpecificControls.Merge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace TrackEditor.Operations;

internal static class GraphMergeOperation
{
    public static void MergeGraph(IList selected)
    {
        var _selected = MainEditor.Instance.SelectedGraph ?? throw new Exception();
        var cmds = selected
                .OfType<GraphInfo>()
                .Select(GraphMergeHelper.MergeGraph);
        var cmd = new ComplexTransform([.. cmds]);
        _selected.Graph.Execute(cmd);
    }
}
