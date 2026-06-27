using SpecificControls.Canvases;
using SpecificControls.Graph;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SpecificControls.Other;

/// <summary>
/// Interaction logic for LayerShower.xaml
/// </summary>
public partial class LayerShower : UserControl
{
    public ObservableCollection<IInfo>? CellInfoCollection
    {
        get => (ObservableCollection<IInfo>?)GetValue(CellInfoCollectionProperty);
        set => SetValue(CellInfoCollectionProperty, value);
    }

    public CellInfo? SelectedCell
    {
        get => (CellInfo?)GetValue(SelectedCellProperty);
        set => SetValue(SelectedCellProperty, value);
    }

    public double ScaleX
    {
        get => (double)GetValue(ScaleXProperty);
        set => SetValue(ScaleXProperty, value);
    }

    public double ScaleY
    {
        get => (double)GetValue(ScaleYProperty);
        set => SetValue(ScaleYProperty, value);
    }

    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    private readonly Dictionary<CellInfo, BoolGridCanvas> _canvases = [];

    public LayerShower()
    {
        InitializeComponent();
    }

    private void CellInfoCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                HandleChange(e);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Reset:
                HandleReset();
                return;
        }
        UpdateLayerVisuals();
    }

    private void HandleChange(NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is not CellInfo cellInfo) continue;
                List.Children.Remove(_canvases[cellInfo]);
                _canvases.Remove(cellInfo);
            }
        }
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is not CellInfo cellInfo) continue;
                var canvas = InitCanvas(cellInfo);
                List.Children.Add(canvas);
                _canvases[cellInfo] = canvas;
            }
        }
    }

    private BoolGridCanvas InitCanvas(CellInfo info)
    {
        var canvas = new BoolGridCanvas
        {
            CellInfo = info
        };
        canvas.SetBinding(BoolGridCanvas.CenterProperty, 
            new Binding(nameof(Center)) { Source = this });
        canvas.SetBinding(BoolGridCanvas.ScaleXProperty,
            new Binding(nameof(ScaleX)) { Source = this });
        canvas.SetBinding(BoolGridCanvas.ScaleYProperty,
            new Binding(nameof(ScaleY)) { Source = this });
        return canvas;
    }

    private void HandleReset()
    {
        List.Children.Clear();
        _canvases.Clear();

        if (CellInfoCollection == null) return;

        foreach (var info in CellInfoCollection)
        {
            if (info is not CellInfo cellInfo) continue;
            var canvas = InitCanvas(cellInfo);
            List.Children.Add(canvas);
            _canvases[cellInfo] = canvas;
        }
        UpdateLayerVisuals();
    }

    private void UpdateLayerVisuals()
    {
        if (CellInfoCollection == null) return;

        var selected = SelectedCell;
        var hasSelection = selected != null && _canvases.ContainsKey(selected);
        for (int i = 0; i < CellInfoCollection.Count; i++)
        {
            var info = CellInfoCollection[i];
            if (info is not CellInfo cellInfo) continue;
            if (!_canvases.TryGetValue(cellInfo, out var canvas)) continue;

            Panel.SetZIndex(canvas, hasSelection && cellInfo == selected ? 1 : -i);
            canvas.Opacity = !hasSelection || cellInfo == selected ? 1 : 0.5;
        }
    }
}
