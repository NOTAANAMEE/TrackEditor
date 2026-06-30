using SpecificControls.Graph;
using SpecificControls.Merge;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TrackEditor.Windows
{
    /// <summary>
    /// Interaction logic for CellMergePopup.xaml
    /// </summary>
    public partial class CellMergePopup : Window
    {
        private CellInfo _selected;

        public CellMergePopup()
        {
            InitializeComponent();
            var cellInfos = MainEditor.Instance.CellInfos;
            _selected = MainEditor.Instance.SelectedCell ?? throw new Exception();
            ListBox.ItemsSource = cellInfos.Where(a => a != _selected);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var cell = _selected;
            CellInfo[] cells = [cell, .. ListBox.SelectedItems.OfType<CellInfo>()];
            var output = CellMergeHelper.MergeCells(cells);
            MainEditor.Instance.CellInfos.Add(output);
            MainEditor.Instance.SelectedCell = output;
            Close();
        }
    }
}
