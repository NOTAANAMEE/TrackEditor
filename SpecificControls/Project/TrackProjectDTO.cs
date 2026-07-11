using SpecificControls.Graph;
using SpecificControls.Project.Commands;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpecificControls.Project;

public partial class TrackProjectDTO: DependencyObject
{
    public EventHandler<bool>? IsDirtyChanged;

    public string? FilePath
    {
        get => (string?)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public ObservableCollection<CellInfo> Cells => []; // Will not be changed during the lifetime

    public ObservableCollection<GraphInfo2> Graphs => [];

    public CellInfo? SelectedCell
    {
        get => (CellInfo?) GetValue(SelectedCellProperty);
        set => SetValue(SelectedCellProperty, value);
    }

    public GraphInfo2 SelectedGraph
    {
        get => (GraphInfo2)GetValue(SelectedGraphProperty);
        set => SetValue(SelectedGraphProperty, value);
    }

    public bool IsDirty { get; private set; }

    private Stack<ProjectCommandBase> UndoStack = [];

    private Stack<ProjectCommandBase> RedoStack = [];

    public void ExecuteCommand(ProjectCommandBase command)
    {
        command.Execute(this);
        UndoStack.Push(command);
        RedoStack.Clear();
        if (!IsDirty)
        {
            IsDirty = true;
            IsDirtyChanged?.Invoke(this, true);
        }
    }

    public void UndoCommand()
    {
        if (UndoStack.Count == 0) return;
        var command = UndoStack.Pop();
        command.Undo(this);
        RedoStack.Push(command);
        if (IsDirty && UndoStack.Count == 0)
        {
            IsDirty = false;
            IsDirtyChanged?.Invoke(this, false);
        }
    }

    public void RedoCommand()
    {
        if (RedoStack.Count == 0) return;
        var command = RedoStack.Pop();
        command.Execute(this);
        UndoStack.Push(command);
        if (!IsDirty)
        {
            IsDirty = true;
            IsDirtyChanged?.Invoke(this, true);
        }
    }

    public void AddCell(CellInfo cell)
    {
        Cells.Add(cell);
    }

    public void RemoveCell(CellInfo cell)
    {
        Cells.Remove(cell);
    }

    public void AddGraph(GraphInfo2 graph)
    {
        Graphs.Add(graph);
    }

    public void RemoveGraph(GraphInfo2 graph)
    {
        Graphs.Remove(graph);
    }
}
