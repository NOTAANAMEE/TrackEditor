using Controls;
using Graph.Basic;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Graph;
using SpecificControls.Selection;
using System.Windows.Media;

namespace TrackEditor.Commands;

public class RasterizeLineCommand() : EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "SegmentCommand:RasterizeLineCommand";

    private static readonly string NoSelectableMessage = "";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        if (!args.Selectables.Any()) return;
        if (args.Parameters == null ||
            args.Parameters is not Color color) return;
        var segments = args.Selectables
            .OfType<MyTracableSegment>()
            .Select(a => a.SegmentReference.Segment) ?? [];
        var cellInfo = RasterizeLineHelper.RasterizeLines(segments);
        cellInfo.R = color.R;
        cellInfo.G = color.G;
        cellInfo.B = color.B;
        MainEditor.Instance.CellInfos.Add(cellInfo);
    }

    
}
