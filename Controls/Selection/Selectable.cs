using System.Windows;

namespace Controls;

public abstract class Selectable
{
    public virtual string? SelectionGroup { get; }

    public bool CanBeSelected => SelectionGroup != null;

    public event EventHandler? SelectedChanged;

    private bool _isSelected;

    public bool IsSelected 
    { 
        get => _isSelected; 
        internal set
        {
            if (_isSelected == value) return; 
            _isSelected = value;
            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

public class SelectedUpdateEventArgs(Selectable[] selected) : RoutedEventArgs()
{
    public Selectable[] Selected { get; } = selected;
}

public delegate void SelectedUpdateEventHandler(object sender, SelectedUpdateEventArgs e);
