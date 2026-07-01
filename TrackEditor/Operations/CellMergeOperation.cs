using SpecificControls.Graph;
using SpecificControls.Merge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace TrackEditor.Operations;

internal static class CellMergeOperation
{
    public static void MergeCells(IList selected)
    {
        var cell = MainEditor.Instance.SelectedCell ?? throw new Exception();
        CellInfo[] cells = [cell, .. selected.OfType<CellInfo>()];
        var output = CellMergeHelper.MergeCells(cells);
        MainEditor.Instance.CellInfos.Add(output);
        MainEditor.Instance.SelectedCell = output;
    }
}
