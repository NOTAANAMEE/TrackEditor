using SpecificControls.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SpecificControls.Project;

public partial class TrackProjectDTO
{
    public static readonly DependencyProperty SelectedCellProperty 
        = DependencyProperty.Register(
            nameof(SelectedCell),
            typeof(CellInfo),
            typeof(TrackProjectDTO), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedGraphProperty
        = DependencyProperty.Register(
            nameof(SelectedGraph),
            typeof(GraphInfo2),
            typeof(TrackProjectDTO), new PropertyMetadata(null));

    public static readonly DependencyProperty FilePathProperty
        = DependencyProperty.Register(
            nameof(FilePath),
            typeof(string),
            typeof(TrackProjectDTO), new PropertyMetadata(null));
}
