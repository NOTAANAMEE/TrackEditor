using Controls.Selection;
using SpecificControls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using TrackEditor.Commands;

namespace TrackEditor.Operations;

internal static class RasterizeOperation
{
    public static void Rasterize(BezierEditor? editor, string cmdName, Color color)
    {
        editor?.RunCommand(cmdName, color);
    }
}
