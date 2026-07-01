using SpecificControls.Graph;
using SpecificControls.Merge;
using System.Windows;
using System.Windows.Controls;
using TrackEditor.Operations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TrackEditor.Windows
{
    /// <summary>
    /// Interaction logic for CellMergePopup.xaml
    /// </summary>
    public partial class CellMergePopup : Window
    {
        public CellMergePopup()
        {
            InitializeComponent();
            ListBox.ItemsSource = MainEditor.Instance.UnselectedCells;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CellMergeOperation.MergeCells(ListBox.SelectedItems);
            Close();
        }
    }
}
