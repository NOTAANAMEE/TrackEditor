using Graph.Graph;
using SpecificControls.Graph;
using SpecificControls.Merge;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackEditor.Operations;

internal static class DuplicateOperation
{
    public static void DuplicateGraph(GraphInfo selected)
    {
        var newInfo = new GraphInfo(new(new()));
        var transform = GraphMergeHelper.MergeGraph(selected);
        transform.Execute(newInfo.Graph.Graph);
        MainEditor.Instance.GraphInfos.Add(newInfo);
        MainEditor.Instance.SelectedGraph = newInfo;
    }

    public static void DuplicateCell(CellInfo selected)
    {
        var newInfo = CellMergeHelper.MergeCells([selected]);
        MainEditor.Instance.CellInfos.Add(newInfo);
        MainEditor.Instance.SelectedCell = newInfo;
    }
}
