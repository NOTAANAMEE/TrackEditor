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
using TrackEditor.Operations;

namespace TrackEditor.Windows
{
    /// <summary>
    /// Interaction logic for GraphMergePopup.xaml
    /// </summary>
    public partial class GraphMergePopup : Window
    {

        public GraphMergePopup()
        {
            InitializeComponent();
            ListBox.ItemsSource = MainEditor.Instance.UnselectedGraphs;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GraphMergeOperation.MergeGraph(ListBox.SelectedItems);
            Close();
        }
    }
}
