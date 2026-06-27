using Graph.Graph;
using System.Windows;

namespace SpecificControls.Canvases;

public partial class MyPointCanvas
{
    public static readonly DependencyProperty TargetProperty
        = DependencyProperty.Register(
            nameof(Target), typeof(GraphChanger), typeof(MyPointCanvas),
            new PropertyMetadata(null, OnTargetPropertyChanged));

    public static readonly DependencyProperty ShowHandleProperty
        = DependencyProperty.Register(
            nameof(ShowHandle), typeof(bool), typeof(MyPointCanvas),
            new PropertyMetadata(false, OnShowHandlePropertyChanged));

    public static readonly DependencyProperty AnchorMovableProperty
        = DependencyProperty.Register(
            nameof(AnchorMovable), typeof(bool), typeof(MyPointCanvas));

    private static void OnShowHandlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MyPointCanvas canvas) return;
        if ((bool)e.NewValue) canvas.AddHandle();
        else canvas.RemoveHandle();
    }

    private static void OnTargetPropertyChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MyPointCanvas canvas) return;
        canvas.UpdateTarget();
    }
}
