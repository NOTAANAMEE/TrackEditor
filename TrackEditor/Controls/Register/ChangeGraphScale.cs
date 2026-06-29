using SpecificControls.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TrackEditor.Controls;

public partial class ChangeGraphScale
{
    public static readonly DependencyProperty SelectedGraphProperty
        = DependencyProperty.Register(
            nameof(SelectedGraph), typeof(GraphInfo), typeof(ChangeGraphScale),
            new PropertyMetadata(null, SelectedGraphChange));

    private static void SelectedGraphChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ChangeGraphScale graph) return;
        graph.SelectedGraphUpdate();
    }
}
