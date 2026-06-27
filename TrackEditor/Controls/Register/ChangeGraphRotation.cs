using SpecificControls.Graph;
using System.Windows;

namespace TrackEditor.Controls;

public partial class ChangeGraphRotation
{
    public static readonly DependencyProperty SelectedGraphProperty
        = DependencyProperty.Register(
            nameof(SelectedGraph), typeof(GraphInfo), typeof(ChangeGraphRotation),
            new PropertyMetadata(null, SelectedGraphChange));

    private static void SelectedGraphChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ChangeGraphRotation graph) return;
        graph.SelectedGraphUpdate();
    }
}
