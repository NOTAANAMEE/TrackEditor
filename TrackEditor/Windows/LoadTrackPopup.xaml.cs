using Newtonsoft.Json.Linq;
using SpecificControls.Graph;
using Graph.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrackEditor.Windowses
{
    /// <summary>
    /// Interaction logic for LoadTrackPopup.xaml
    /// </summary>
    public partial class LoadTrackPopup : Window
    {
        public string? Path;

        public LoadTrackPopup()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var initialScaleText = InitialScaleTxt.Text;
            var scaleXText = ScaleXTxt.Text;
            var scaleYText = ScaleYTxt.Text;
            if (Path == null)
            {
                MessageBox.Show("Invalid Path");
                return;
            }
            if (!double.TryParse(initialScaleText, out var initialScale))
            {
                MessageBox.Show("Initial Scaling is not a valid number");
                return;
            }
            if (!double.TryParse(scaleXText, out var scaleX))
            {
                MessageBox.Show("Scale X is not a valid number");
                return;
            }
            if (!double.TryParse(scaleYText, out var scaleY))
            {
                MessageBox.Show("Scale X is not a valid number");
                return;
            }
            var arr = JArray.Parse(File.ReadAllText(Path));
            var graph = new GraphChanger(
                TrackLoader.ReadGraph(arr, scaleX * initialScale, scaleY * initialScale));
            var info = new GraphInfo(graph);
            MainEditor.Instance.GraphInfos.Add(info);
            MainEditor.Instance.SelectedGraph = info;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
