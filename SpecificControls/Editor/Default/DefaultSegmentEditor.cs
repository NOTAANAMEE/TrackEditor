using Graph.Command;
using SpecificControls.Selection;
using System.Windows.Input;

namespace SpecificControls.Editor.Default;

public class DefaultSegmentEditor() : EditorState(EditorModes.SegmentSelect)
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

    public override void HandleMouseClick(object? sender, EditorClickArgs arg)
    { }

    public override void HandleSignal(object? sender, EditorSignalArgs args)
    {
        if (!_commands.TryGetValue(args.SignalName, out var command)) return;
        command.Signal(this, args);
    }

    public override void HandleKey(object? sender, EditorKeyArgs args)
    {
        if (args.KeyEventArgs.Key != Key.Delete) return;
        var segments = args.Selectables
            .OfType<MyTracableSegment>()
            .Select(a => a.SegmentReference);
        var command = new SegmentsRemoveTransform(segments, args.Graph.Graph);
        args.Graph.Execute(command);
    }
}
