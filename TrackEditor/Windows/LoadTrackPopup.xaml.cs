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
using TrackEditor.Operations;

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
            var parseCheck = new ParseCheck()
                .CheckPath(Path, "Path", out string path)
                .ParseDouble(InitialScaleTxt.Text, "Initial Scale", out double initialScale)
                .ParseDouble(ScaleXTxt.Text, "Scale X", out double scaleX)
                .ParseDouble(ScaleYTxt.Text, "Scale Y", out double scaleY);
            if (!parseCheck.All)
            {
                MessageBox.Show(parseCheck.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadTrackOperation.LoadTrack(path, scaleX, scaleY);
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
