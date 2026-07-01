using SpecificControls;
using SpecificControls.Editor;
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
using TrackEditor.Commands;
using TrackEditor.Operations;

namespace TrackEditor.Windowses
{
    /// <summary>
    /// Interaction logic for LeftRightOffsetPopup.xaml
    /// </summary>
    public partial class LeftRightOffsetPopup : Window
    {
        public BezierEditor? _editor;

        public LeftRightOffsetPopup()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var parseCheck = new ParseCheck()
                .ParseDouble(OffsetText.Text, "Offset", out var offset);
            if (!parseCheck.All)
            {
                MessageBox.Show(parseCheck.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LeftRightOffsetOperation.ApplyOffset(
                _editor, offset, 
                LeftCheckBox.IsChecked ?? false, 
                RightCheckBox.IsChecked ?? false);
            Close();
        }
    }
}
