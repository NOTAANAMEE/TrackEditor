using SpecificControls;
using System.Windows;
using TrackEditor.Commands;
using TrackEditor.Operations;

namespace TrackEditor.Windowses
{
    /// <summary>
    /// Interaction logic for RasterizePopup.xaml
    /// </summary>
    public partial class RasterizePopup : Window
    {
        public BezierEditor? _editor;

        public string Command = RasterizeCommand.CommandName;

        public RasterizePopup()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ColorSelector.SelectedColor == null) return;
            RasterizeOperation.Rasterize(_editor, Command, ColorSelector.SelectedColor.Value);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
