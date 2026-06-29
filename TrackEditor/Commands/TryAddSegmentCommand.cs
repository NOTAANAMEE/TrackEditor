using Graph.Command;
using Graph.Graph;
using Graph.Graph.Reference;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Selection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TrackEditor.Commands;

public class TryAddSegmentCommand() : EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "AABBCCDDEE";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        var selectedAnchors = args.Selectables.OfType<MyTracablePoint>().Select(a => a.Anchor);
        if (selectedAnchors.Count() != 2)
        {
            MessageBox.Show("Select exact 2 anchors to connect!");
            return;
        }
        TryAddSegment(selectedAnchors.First(), selectedAnchors.Last(), args.Graph);
    }

    private static void TryAddSegment(
        AnchorReference anchor0, AnchorReference anchor1, GraphChanger changer)
    {
        var graph = changer.Graph;
        if (!graph.HasNextSegment(anchor0) && !graph.HasLastSegment(anchor1)) 
        {
            changer.Execute(new SegmentAddTransform(new(anchor0, anchor1)));
        }
        else if (!graph.HasNextSegment(anchor1) && !graph.HasLastSegment(anchor0))
        {
            changer.Execute(new SegmentAddTransform(new(anchor1, anchor0)));
        }
        else MessageBox.Show("The 2 anchors cannot be connected!");
    }
}
