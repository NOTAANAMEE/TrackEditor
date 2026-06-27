using SpecificControls.Graph;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpecificControls.Other;

public partial class LayerSelector
{
    public static readonly DependencyProperty CellInfoCollectionProperty
        = DependencyProperty.Register(
            nameof(CellInfoCollection), typeof(ObservableCollection<IInfo>),
            typeof(LayerSelector), new PropertyMetadata(null));



    public static readonly DependencyProperty SelectedCellProperty
        = DependencyProperty.Register(
            nameof(SelectedCell), typeof(IInfo), typeof(LayerSelector),
            new PropertyMetadata(null));
}
