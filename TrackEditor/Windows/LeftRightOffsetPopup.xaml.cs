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
            var txt = OffsetText.Text;
            if (!double.TryParse(txt, out double offset))
            {
                MessageBox.Show($"{txt} is not a valid offset value!");
                return;
            }
            var param = new CenterlineOffsetCommandParameter()
            {
                Left = LeftCheckBox.IsChecked ?? false,
                Right = RightCheckBox.IsChecked ?? false,
                Offset = offset
            };
            _editor?.RunCommand(
                CenterlineOffsetCommand.CommandName, param);
            Close();
        }
    }
}
