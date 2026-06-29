using Graph.Command;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Selection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TrackEditor.Commands;

internal class SegmentsReverseCommand() : EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "ABCDE";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        var segments = args.Selectables
            .OfType<MyTracableSegment>()
            .Select(a => a.SegmentReference);
        if (!SegmentReverseTransform.CheckSegments(segments, args.Graph.Graph))
        {
            MessageBox.Show("These segments cannot be reversed!"); return;
        }
        args.Graph.Execute(new SegmentReverseTransform(segments, args.Graph.Graph));
    }


}
