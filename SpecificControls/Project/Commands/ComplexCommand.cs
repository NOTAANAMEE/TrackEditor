using System;
using System.Collections.Generic;
using System.Text;

namespace SpecificControls.Project.Commands;

public class ComplexCommand : ProjectCommandBase
{
    private List<ProjectCommandBase> _commands = [];

    public void Add(ProjectCommandBase command) => _commands.Add(command);

    public override void Execute(TrackProjectDTO track)
    {
        foreach (var command in _commands) command.Execute(track);
    }

    public override void Undo(TrackProjectDTO track)
    {
        foreach (var command in _commands.Reverse<ProjectCommandBase>()) command.Undo(track);
    }
}
