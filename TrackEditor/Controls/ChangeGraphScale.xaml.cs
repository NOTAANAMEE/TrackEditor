using Graph.Command;
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

namespace TrackEditor.Controls
{
    /// <summary>
    /// Interaction logic for ChangeGraphScale.xaml
    /// </summary>
    public partial class ChangeGraphScale : UserControl
    {
        private bool _DisableUpdate;

        public GraphInfo? SelectedGraph
        {
            get => (GraphInfo?)GetValue(SelectedGraphProperty);
            set => SetValue(SelectedGraphProperty, value);
        }

        public ChangeGraphScale()
        {
            InitializeComponent();
        }

        private void SelectedGraphUpdate()
        {
            if (SelectedGraph == null)
            {
                ScaleTxt.IsEnabled = false;
            }
            else
            {
                ScaleTxt.IsEnabled = true;
                UpdateScaleValue();
                SelectedGraph.Graph.Graph.RotationChanged += (a, b) => UpdateScaleValue();
            }
        }

        private void UpdateScaleValue()
        {
            if (_DisableUpdate) return;
            ScaleTxt.Text = SelectedGraph?.Graph.Graph.Scale.ToString() ?? "1.0";
        }

        private void ScaleTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (!double.TryParse(ScaleTxt.Text, out var result))
            {
                MessageBox.Show("Invalid Value!");
                UpdateScaleValue();
                return;
            }
            if (SelectedGraph == null) return;
            _DisableUpdate = true;
            SelectedGraph.Graph.Execute(
                new Graph.Command.ScaleTransform(SelectedGraph.Graph.Graph, result));
            _DisableUpdate = false;
        }
    }
}
