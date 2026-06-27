using SpecificControls.Graph;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SpecificControls.Other;

/// <summary>
/// Interaction logic for LayerSelector.xaml
/// </summary>
public partial class LayerSelector : UserControl
{
    public ObservableCollection<IInfo>? CellInfoCollection
    {
        get => (ObservableCollection<IInfo>?)GetValue(CellInfoCollectionProperty);
        set => SetValue(CellInfoCollectionProperty, value);
    }

    public IInfo? SelectedCell
    {
        get => (IInfo?)GetValue(SelectedCellProperty);
        set => SetValue(SelectedCellProperty, value);
    }

    public LayerSelector()
    {
        InitializeComponent();
        CellInfoCollection = [];
    }

    private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (List.SelectedItems.Count == 0)
        {
            SelectedCell = null;
            Btn0.IsEnabled = false;
            Btn1.IsEnabled = false;
            Btn2.IsEnabled = false;
            Btn3.IsEnabled = false;
        }
        else
        {
            Btn0.IsEnabled = true;
            Btn1.IsEnabled = true;
            Btn2.IsEnabled = true;
            Btn3.IsEnabled = true;
        }
    }

    private void Btn0_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedCell != null)
            CellInfoCollection?.Remove(SelectedCell);
    }

    private void Btn3_Click(object sender, RoutedEventArgs e)
    {
        List.SelectedIndex = -1;
    }

    private void Btn1_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedCell is null || CellInfoCollection is null) return;
        var ind = CellInfoCollection.IndexOf(SelectedCell);
        if (ind == 0) return;
        CellInfoCollection.Move(ind, ind - 1);
    }

    private void Btn2_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedCell is null || CellInfoCollection is null) return;
        var ind = CellInfoCollection.IndexOf(SelectedCell);
        if (ind == CellInfoCollection.Count - 1) return;
        CellInfoCollection.Move(ind, ind + 1);
    }
}
