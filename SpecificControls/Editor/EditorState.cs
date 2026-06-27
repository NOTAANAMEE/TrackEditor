
namespace SpecificControls.Editor;

public abstract class EditorState
{
    public EditorMode Mode { get; }

    protected EditorState(EditorMode mode)
    {
        Mode = mode;
    }

    public virtual void HandleSelectionChange(object? sender, EditorArgs args) { }

    public virtual void HandleMouseClick(object? sender, EditorClickArgs args) { }

    public virtual void HandleSignal(object? sender, EditorSignalArgs args) { }

    public virtual void HandleKey(object? sender, EditorKeyArgs args) { }
}
