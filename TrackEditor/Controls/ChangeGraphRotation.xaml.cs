using Graph.Command;
using Newtonsoft.Json.Converters;
using SpecificControls.Graph;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrackEditor.Controls
{
    /// <summary>
    /// Interaction logic for ChangeGraphRotation.xaml
    /// </summary>
    public partial class ChangeGraphRotation : UserControl
    {
        private bool _textDisable;

        private bool _rotateDisable;

        public GraphInfo? SelectedGraph
        {
            get => (GraphInfo?)GetValue(SelectedGraphProperty);
            set => SetValue(SelectedGraphProperty, value);
        }
        
        public ChangeGraphRotation()
        {
            InitializeComponent();
        }

        private void SelectedGraphUpdate()
        {
            if (SelectedGraph == null)
            {
                RotationBar.IsEnabled = false;
                RotationTxt.IsEnabled = false;
            }
            else
            {
                RotationBar.IsEnabled = true;
                RotationTxt.IsEnabled = true;
                UpdateRotationValue();
                SelectedGraph.Graph.Graph.RotationChanged += (a, b) => UpdateRotationValue();
            }
        }

        private void UpdateRotationValue()
        {
            if (SelectedGraph == null) return;
            _rotateDisable = true;
            RotationBar.Value = double.RadiansToDegrees(SelectedGraph.Graph.Graph.Rotation);
            _rotateDisable = false;
        }

        private void Rotation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (!double.TryParse(RotationTxt.Text, out var result))
            {
                MessageBox.Show("Invalid Value!");
                RotationTxt.Text = RotationBar.Value.ToString();
                return;
            }
            _textDisable = true;
            RotationBar.Value = result;
            _textDisable = false;
        }

        private void RotationBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_textDisable) RotationTxt.Text = e.NewValue.ToString();
            if (_rotateDisable) return;
            var graph = SelectedGraph?.Graph;
            if (graph == null) return;
            graph.Execute(new RotationTransform(graph.Graph, 
                double.DegreesToRadians(RotationBar.Value)));
        }
    }
}
