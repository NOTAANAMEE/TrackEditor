namespace SpecificControls.Project.Commands;

public abstract class ProjectCommandBase
{
    public abstract void Execute(TrackProjectDTO track);

    public abstract void Undo(TrackProjectDTO track);
}
