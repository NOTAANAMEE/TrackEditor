using SpecificControls;
using TrackEditor.Commands;

namespace TrackEditor.Operations;

internal static class LeftRightOffsetOperation
{
    public static void ApplyOffset(BezierEditor? editor, double offset, bool left, bool right)
    {
        var param = new CenterlineOffsetCommandParameter()
        {
            Left = left,
            Right = right,
            Offset = offset
        };
        editor?.RunCommand(CenterlineOffsetCommand.CommandName, param);
    }
}
