using Controls;
using Graph.Basic;
using Graph.Graph.Reference;
using Rasterization;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Graph;
using SpecificControls.Selection;
using System.Windows;
using System.Windows.Media;

namespace TrackEditor.Commands;

public class RasterizeCommand() : EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "SegmentCommand:RasterizeCommand";

    private static readonly string NoSelectableMessage = "";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        var lines = args.Selectables;
        if (!lines.Any())
        {
            MessageBox.Show(NoSelectableMessage);
            return;
        }
        if (args.Parameters == null ||
            args.Parameters is not Color color) return;
        var pointsAndSegments = InitGraphAndMap(lines, out var map);
        foreach (var line in lines)
        {
            if (line is not MyTracableSegment segment) continue;
            SetMap(segment.SegmentReference, pointsAndSegments, map);
        }
        var info = Rasterize(pointsAndSegments, color);
        MainEditor.Instance.CellInfos.Add(info);
    }

    private static CellInfo Rasterize(PointsAndSegments graph, Color color)
    {
        var left = graph.LeftFloor;
        var top = graph.TopCeil;
        var width = graph.RightCeil - graph.LeftFloor;
        var height = graph.TopCeil - graph.BottomFloor;
        var cells = new bool[width, height];
        for (var i = 0; i < height; i++)
        {
            RasterizationTask.Rasterize(graph, i, graph.BottomFloor, graph.LeftFloor, width, cells);
        }
        return new()
        {
            Cells = cells,
            TopLeft = new BPoint(left, top).ToPoint(),
            FlipY = true,
            R = color.R,
            G = color.G,
            B = color.B,
            A = 255
        };
    }

    private static void SetMap(
        SegmentReference reference, PointsAndSegments graph,
        Dictionary<AnchorReference, int> map)
    {
        var list = new List<BBezierSegment>();
        reference.Segment.Flattern(list);
        var lastInd = map[reference.From];
        for (int i = 0; i < list.Count - 1; i++)
        {
            var curSegment = list[i];
            var curInd = graph.Add(curSegment.P3);
            graph.Add(lastInd, curInd);
            lastInd = curInd;
        }
        var toInd = map[reference.To];
        graph.Add(lastInd, toInd);
    }

    private PointsAndSegments InitGraphAndMap(IEnumerable<Selectable> lines, 
        out Dictionary<AnchorReference, int> map)
    {
        var ret = new PointsAndSegments();
        map = [];
        foreach (var line in lines)
        {
            if (line is not MyTracableSegment segment) continue;
            var p0 = segment.SegmentReference.From;
            var p1 = segment.SegmentReference.To;
            if (!map.ContainsKey(p0)) map[p0] = ret.Add(p0.Position);
            if (!map.ContainsKey(p1)) map[p1] = ret.Add(p1.Position);
        }
        return ret;
    }
}
