namespace Controls.Selection;

public class SelectionManager<T> where T: Selectable
{
    private readonly HashSet<T> _selected = [];

    public IReadOnlySet<T> Selected => _selected;

    public event EventHandler? SelectionChanged;

    private bool _enableSelect = true;

    private bool _multiSelect = true;

    public bool MultiSelect
    {
        get => _multiSelect;
        set
        {
            var prev = _multiSelect;
            _multiSelect = value;
            // If value = false and prev = true
            if (!value && prev) Clear();
        }
    }

    public bool EnableSelect
    {
        get => _enableSelect;
        set
        {
            var prev = _enableSelect;
            _enableSelect = value;
            if (!value && prev) Clear();
        }
    }

    private bool AllowMultiSelect => _multiSelect && EnableSelect;

    public bool Remove(T selectable)
    {
        if (!_selected.Remove(selectable))
            return false; // If not exists, return
        selectable.IsSelected = false;
        InvokeUpdate();
        return true;
    }

    public void Remove(IEnumerable<T> selectables)
    {
        foreach (var item in selectables)
        {
            if (!_selected.Remove(item)) continue;
            item.IsSelected = false;
        }
        InvokeUpdate();
    }

    public void SetSingleSelected(T selectable)
    {
        if (!EnableSelect) return;
        // Remove all the selection
        // Then try to select
        foreach (var s in _selected) 
            s.IsSelected = false;
        _selected.Clear();

        if (selectable.CanBeSelected)
        {
            _selected.Add(selectable);
            selectable.IsSelected = true;
        }

        InvokeUpdate();
    }

    public void UpdateOneSelected(T selectable)
    {
        if (!AllowMultiSelect) return;
        if (!selectable.CanBeSelected) return;
        if (_selected.Count > 0)
        {
            var group = _selected.First().SelectionGroup;
            if (!CheckCanBeSelected(selectable, group)) return;
        }
        if (!_selected.Remove(selectable)) _selected.Add(selectable);
        selectable.IsSelected = !selectable.IsSelected;
        InvokeUpdate();
    }

    public void SetMultiSelected(IEnumerable<T> selected)
    {
        if (!AllowMultiSelect) return;
        foreach (var s in _selected)
            s.IsSelected = false;
        _selected.Clear();

        if (selected.Any())
        {
            var group = selected.First().SelectionGroup;
            foreach (var selectable in selected)
            {
                group ??= selectable.SelectionGroup;
                if (!CheckCanBeSelected(selectable, group)) continue;
                _selected.Add(selectable);
                selectable.IsSelected = true;
            }
        } 
        InvokeUpdate();
    }

    public void AddMultiSelected(IEnumerable<T> selected)
    {
        if (!AllowMultiSelect) return;
        if (selected.Any())
        {
            var group = selected.First().SelectionGroup;
            foreach (var selectable in selected)
            {
                group ??= selectable.SelectionGroup;
                if (!CheckCanBeSelected(selectable, group)) continue;
                _selected.Add(selectable);
                selectable.IsSelected = true;
            }
        }
        InvokeUpdate();
    }

    private static bool CheckCanBeSelected(T selectable, string? groupName)
    {
        return selectable.CanBeSelected && selectable.SelectionGroup == groupName;
    }

    private void InvokeUpdate()
    {
        SelectionChanged?.Invoke(this, new EventArgs());
    }

    public void Clear()
    {
        foreach (var s in _selected)
            s.IsSelected = false;
        _selected.Clear();
        InvokeUpdate();
    }
}
