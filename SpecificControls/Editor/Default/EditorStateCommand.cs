using System;
using System.Collections.Generic;
using System.Text;

namespace SpecificControls.Editor.Default;

public abstract class EditorStateCommand(string signalName)
{
    public string SignalName => signalName;

    public abstract void Signal(EditorState state, EditorSignalArgs args);
}
