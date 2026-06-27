using SpecificControls.Graph;
using System.Collections.ObjectModel;
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
}
