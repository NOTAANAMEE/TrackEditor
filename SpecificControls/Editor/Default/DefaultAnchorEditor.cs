using Graph.Graph.Reference;
using SpecificControls.Selection;
using System;
 using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SpecificControls.Editor.Default;

public class DefaultAnchorEditor() : EditorState(EditorModes.AnchorEdit)
{
    private static Dictionary<string, EditorStateCommand> _commands = [];

    public static void AddCommand(EditorStateCommand command)
    {
        _commands.Add(command.SignalName, command);
    }

    public static void RemoveCommand(EditorStateCommand command)
    {
        _commands.Remove(command.SignalName);
    }

    private class InsertPointModeCommand()
        : EditorStateCommand("DefaultAnchorEditor.InsertPointModeCommand")
    {
        public override void Signal(EditorState state, EditorSignalArgs args)
        {
            if (state is not DefaultAnchorEditor editor) return;
            editor.InsertPointMode = true;
        }
    }

    static DefaultAnchorEditor()
    {
        AddCommand(new InsertPointModeCommand());
    }

    public bool InsertPointMode { get; set; }


    public override void HandleMouseClick(object? sender, EditorClickArgs arg) 
    {
        if (!InsertPointMode) return;
        // TODO - Here to insert new point to the segment
        var command = AnchorInsertHelper.GetCommand(arg.Graph.Graph, arg.WorldPosition);
        arg.Graph.Execute(command);
        InsertPointMode = false;
    }

    public override void HandleSignal(object? sender, EditorSignalArgs args) 
    {
        if (!_commands.TryGetValue(args.SignalName, out var command)) return;
        command.Signal(this, args);
    }

    public override void HandleKey(object? sender, EditorKeyArgs args)
    {
        if (args.KeyEventArgs.Key != Key.Delete) return;
        if (!args.Selectables.Any()) return;
        HashSet<AnchorReference> anchors =  [..args.Selectables
            .OfType<MyTracablePoint>()
            .Select(a => a._anchor)];
        var transform = AnchorRemoveSpliceHelper
            .GetRemoveTransform(args.Graph.Graph, anchors);
        args.Graph.Execute(transform);
    }
}
