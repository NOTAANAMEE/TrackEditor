using Graph.Graph;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SpecificControls.Editor.Default;
using SpecificControls.Graph;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TrackEditor.Commands;
using TrackEditor.Files;
using TrackEditor.Windows;
using TrackEditor.Windowses;

namespace TrackEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DefaultSegmentEditor _segmentEditor = new();

        DefaultAnchorEditor _anchorEditor = new();

        DefaultInvisibleEditor _invisibleEditor = new();

        public MainWindow()
        {
            InitializeComponent();
            DefaultSegmentEditor.AddCommand(new CenterlineOffsetCommand());
            DefaultSegmentEditor.AddCommand(new RasterizeCommand());
            DefaultSegmentEditor.AddCommand(new RasterizeLineCommand());
            DefaultSegmentEditor.AddCommand(new SegmentsReverseCommand());
            DefaultAnchorEditor.AddCommand(new TryAddSegmentCommand());
            Editor.EditorState = _segmentEditor;
        }

        private void Offset_Click(object sender, RoutedEventArgs e)
        {
            var window = new LeftRightOffsetPopup()
            {
                _editor = Editor
            };
            window.ShowDialog();
        }

        private void Rasterize_Click(object sender, RoutedEventArgs e)
        {
            var window = new RasterizePopup()
            {
                _editor = Editor
            };
            window.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (LayerShower.Visibility == Visibility.Visible) 
                LayerShower.Visibility = Visibility.Collapsed;
            else LayerShower.Visibility = Visibility.Visible;
        }

        private void Mode_Click(object sender, RoutedEventArgs e)
        {
            AnchorEditCheck.IsChecked = false;
            SegmentEditCheck.IsChecked = false;
            NoneEditCheck.IsChecked = false;
            if (sender == AnchorEditCheck)
            {
                AnchorEditCheck.IsChecked = true;
                Editor.EditorState = _anchorEditor;
            }
            else if (sender == SegmentEditCheck)
            {
                SegmentEditCheck.IsChecked = true;
                Editor.EditorState = _segmentEditor;
            }
            else
            {
                NoneEditCheck.IsChecked = true;
                Editor.EditorState = _invisibleEditor;
            }
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            LayerShower?.Visibility = Visibility.Visible;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            LayerShower.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Export Image",
                Filter = "PNG Image (*.png)|*.png|All files (*.*)|*.*",
                DefaultExt = ".png",
                FileName = "export.png"
            };
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                var infos = MainEditor.Instance.CellInfos;
                var cells = infos.Select(a => (CellInfo)a);
                ImageSaver.SaveImage(cells, path);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Import Json",
                Filter = "JSON File (*.json)|*.json",
            };
            if (dialog.ShowDialog() ?? false)
            {
                var path = dialog.FileName;
                var window = new LoadTrackPopup()
                {
                    Path = path
                };
                window.ShowDialog();
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Export Image",
                Filter = "PNG Image (*.png)|*.png|All files (*.*)|*.*",
                DefaultExt = ".png",
                FileName = "export.png"
            };
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                var info = MainEditor.Instance.SelectedCell;
                if (info == null)
                {
                    MessageBox.Show("No image is selected");
                    return;
                }
                ImageSaver.SaveImage([info], path);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Editor.RunCommand("DefaultAnchorEditor.InsertPointModeCommand");
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            var window = new RasterizePopup()
            {
                _editor = Editor,
                Command = RasterizeLineCommand.CommandName
            };
            window.ShowDialog();
        }

        private void UndoCmd(object sender, RoutedEventArgs e)
        {
            MainEditor.Instance.SelectedGraph?.Graph.Undo();
        }

        private void RedoCmd(object sender, RoutedEventArgs e)
        {
            MainEditor.Instance.SelectedGraph?.Graph.Redo();
        }

        private void SegmentsReverseCmd(object sender, RoutedEventArgs e)
        {
            Editor.RunCommand(SegmentsReverseCommand.CommandName);
        }

        private void ConnectPointCmd(object sender, RoutedEventArgs e)
        {
            Editor.RunCommand(TryAddSegmentCommand.CommandName);
        }

        private void AddPointCmd(object sender, RoutedEventArgs e)
        {
            Editor.RunCommand("DefaultAnchorEditor.AddPointModeCommand");
        }

        private void NewGraph(object sender, RoutedEventArgs e)
        {
            var info = new GraphInfo(new GraphChanger(new BezierGraph()));
            MainEditor.Instance.GraphInfos.Add(info);
            MainEditor.Instance.SelectedGraph = info;
        }

        private void MergeGraph(object sender, RoutedEventArgs e)
        {
            if (MainEditor.Instance.SelectedCell == null)
            {
                MessageBox.Show("No graph!");
                return;
            }
            var window = new GraphMergePopup();
            window.ShowDialog();
        }

        private void MergeCell(object sender, RoutedEventArgs e)
        {
            if (MainEditor.Instance.SelectedCell == null)
            {
                MessageBox.Show("Select a cell map before started!");
                return;
            }
            var window = new CellMergePopup();
            window.ShowDialog();
        }
    }
}
