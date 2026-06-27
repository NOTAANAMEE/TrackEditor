using System;
using System.Collections.Generic;
using System.Text;

namespace SpecificControls.Editor;

[Flags]
public enum EditorMode
{
    // 00000000 
    // 00000001
    // 00000010 
    // 00000011 Shows all
    // 0 0 0 0 0 0 00
    // The first bit: It is anchor oriented or segment oriented
    // The second bit: It can be selected or not
    // The third bit: If second bit, it can be multi selected or not
    // The fourth bit: If anchor oriented and second bit, shows handle or not (Handle is not selectable)
    // The fifth bit: If anchor oriented and second bit, it can be moved or not
    // The sixth bit: None
    // The last 2:
    //  00: Shows nothing
    //  01: Shows anchor layer
    //  10: Shows Segment layer
    //  11: Shows both

    None = 0,

    AnchorOriented = 1 << 0,
    Selectable = 1 << 1,
    MultiSelectable = 1 << 2,
    ShowHandles = 1 << 3,
    Movable = 1 << 4,

    ShowAnchors = 1 << 6,
    ShowSegments = 1 << 7,
}

public static class EditorModes
{
    public const EditorMode AnchorView =
        EditorMode.AnchorOriented
        | EditorMode.ShowAnchors;

    public const EditorMode AnchorSelect =
        EditorMode.AnchorOriented
        | EditorMode.Selectable
        | EditorMode.MultiSelectable
        | EditorMode.ShowAnchors;

    public const EditorMode AnchorEdit =
        EditorMode.AnchorOriented
        | EditorMode.Selectable
        | EditorMode.MultiSelectable
        | EditorMode.ShowHandles
        | EditorMode.Movable
        | EditorMode.ShowAnchors
        | EditorMode.ShowSegments;

    public const EditorMode SegmentView =
        EditorMode.ShowSegments;

    public const EditorMode SegmentSelect =
        EditorMode.Selectable
        | EditorMode.MultiSelectable
        | EditorMode.ShowSegments;
}
