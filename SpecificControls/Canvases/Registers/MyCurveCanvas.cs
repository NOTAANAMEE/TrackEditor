using Graph.Graph;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SpecificControls.Canvases;

public partial class MyCurveCanvas
{
    public static readonly DependencyProperty TargetProperty
        = DependencyProperty.Register(
            nameof(Target), typeof(GraphChanger), typeof(MyCurveCanvas),
            new PropertyMetadata(null, OnTargetPropertyChanged));

    private static void OnTargetPropertyChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MyCurveCanvas canvas) return;
        canvas.UpdateTarget();
    }
}
