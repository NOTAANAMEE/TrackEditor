using Graph.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph;

public class GraphChanger(BezierGraph graph)
{
    public BezierGraph Graph { get; } = graph;

    private readonly Stack<CommandBase> _activeCommands = [];

    private readonly Stack<CommandBase> _redoCommands = [];

    public IEnumerable<CommandBase> ActiveCommands => _activeCommands;

    public IEnumerable<CommandBase> RedoCommands => _redoCommands;

    public void Undo()
    {
        if (_activeCommands.Count == 0) return;
        var cmd = _activeCommands.Pop();
        cmd.Undo(Graph);
        _redoCommands.Push(cmd);
    }

    public void Redo()
    {
        if (_redoCommands.Count == 0) return;
        var cmd = _redoCommands.Pop();
        cmd.Execute(Graph);
        _activeCommands.Push(cmd);
    }

    public void Execute(CommandBase cmd)
    {
        cmd.Execute(Graph);
        _activeCommands.Push(cmd);
        _redoCommands.Clear();
    }

}
