using Controls;
using Graph.Command;
using Graph.Graph;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Selection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace TrackEditor.Commands;

internal class VectorizationCommand(): EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "SegmentCommand:VectorizationCommand";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        if (!args.Selectables.Any()) return;
        var transform = new PointTransform(args.Selectables.Count() * 2);
        foreach (var segment in args.Selectables.OfType<MyTracableSegment>())
        {
            var start = segment.P0.ToBPoint();
            var end = segment.P3.ToBPoint();
            var p1 = (end - start) / 3 + start;
            var p2 = (start - end) / 3 + end;
            transform.Add(new PositionChanger(
                segment.SegmentReference.From,PositionType.PNext,p1
            ));
            transform.Add(new PositionChanger(
                segment.SegmentReference.To, PositionType.PLast, p2
            ));
        }
        args.Graph.Execute(transform);
    }
}
