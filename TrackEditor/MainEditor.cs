using SpecificControls.Graph;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace TrackEditor;

public partial class MainEditor: DependencyObject
{
    private static readonly Lazy<MainEditor> Editor = new(() => new MainEditor());
    
    public static MainEditor Instance => Editor.Value;

    public static readonly DependencyProperty SelectedGraphProperty
        = DependencyProperty.Register(
            nameof(SelectedGraph), typeof(GraphInfo), typeof(MainEditor),
            new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedCellProperty
        = DependencyProperty.Register(
            nameof(SelectedCell), typeof(CellInfo), typeof(MainEditor),
            new PropertyMetadata(null));

    public ObservableCollection<IInfo> GraphInfos { get; } = [];

    public GraphInfo? SelectedGraph
    {
        get => (GraphInfo?)GetValue(SelectedGraphProperty);
        set => SetValue(SelectedGraphProperty, value);
    }

    public ObservableCollection<IInfo> CellInfos { get; } = [];

    public CellInfo? SelectedCell
    {
        get => (CellInfo?)GetValue(SelectedCellProperty);
        set => SetValue(SelectedCellProperty, value);
    }

    public IEnumerable<IInfo> UnselectedGraphs => GraphInfos.Where(a => a != SelectedGraph);

    public IEnumerable<IInfo> UnselectedCells => CellInfos.Where(a => a != SelectedCell);

    public bool TryGetSelectedGraph([NotNullWhen(true)]out GraphInfo? graph)
    {
        graph = SelectedGraph;
        return graph is not null;
    }

    public bool TryGetSelectedCell([NotNullWhen(true)] out CellInfo? cell)
    {
        cell = SelectedCell;
        return cell is not null;
    }
}
