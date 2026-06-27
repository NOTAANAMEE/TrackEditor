using Graph.Command;
using Graph.Graph.Reference;
using SpecificControls.Editor;
using SpecificControls.Editor.Default;
using SpecificControls.Selection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace TrackEditor;

public struct CenterlineOffsetCommandParameter
{
    public bool Left;
    public bool Right;
    public double Offset;
}

public class CenterlineOffsetCommand() : EditorStateCommand(CommandName)
{
    public static readonly string CommandName = "AAABBBCCC";

    public override void Signal(EditorState state, EditorSignalArgs args)
    {
        
        if (!args.Selectables.Any()) return;
        if (args.Parameters is null ||
            args.Parameters is not CenterlineOffsetCommandParameter param) return;
        var offset = param.Offset;
        ComplexTransform transform = new();
        if (param.Left) transform.Add(Command(args, offset, LeftRightEnum.Left));
        if (param.Right) transform.Add(Command(args, offset, LeftRightEnum.Right));
        args.Graph.Execute(transform);
    }

    private static CenterlineOffsetFitTransform Command(
        EditorSignalArgs args, double offset, LeftRightEnum leftRight)
    {
        var list = new List<SegmentReference>();
        foreach (var seg in args.Selectables)
        {
            if (seg is MyTracableSegment segment) list.Add(segment.SegmentReference);
        }
        return new CenterlineOffsetFitTransform(list, offset, leftRight);
    }
}
