using Graph.Command;
using SpecificControls.Graph;
using SpecificControls.Merge;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrackEditor.Windows
{
    /// <summary>
    /// Interaction logic for GraphMergePopup.xaml
    /// </summary>
    public partial class GraphMergePopup : Window
    {
        private GraphInfo _selected;

        public GraphMergePopup()
        {
            InitializeComponent();
            var graphInfos = MainEditor.Instance.GraphInfos;
            _selected = MainEditor.Instance.SelectedGraph ?? throw new Exception();
            ListBox.ItemsSource = graphInfos.Where(a => a != _selected);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var cmds = ListBox.SelectedItems
                .OfType<GraphInfo>()
                .Select(GraphMergeHelper.MergeGraph);
            var cmd = new ComplexTransform([.. cmds]);
            _selected.Graph.Execute(cmd);
            Close();
        }
    }
}
