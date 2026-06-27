using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Controls.Canvases;

public partial class MyThumb
{
    public static readonly DependencyProperty IsDraggingProperty =
        DependencyProperty.Register(
            nameof(IsDragging),
            typeof(bool),
            typeof(MyThumb));

    public static readonly RoutedEvent ClickEvent =
        EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(MyThumb));

    public static readonly RoutedEvent DragStartedEvent =
        EventManager.RegisterRoutedEvent(nameof(DragStarted), RoutingStrategy.Bubble,
            typeof(DragStartedEventHandler),
            typeof(MyThumb));

    public static readonly RoutedEvent DragDeltaEvent =
        EventManager.RegisterRoutedEvent(nameof(DragDelta), RoutingStrategy.Bubble,
            typeof(DragDeltaEventHandler),
            typeof(MyThumb));

    public static readonly RoutedEvent DragCompletedEvent =
        EventManager.RegisterRoutedEvent(nameof(DragCompleted), RoutingStrategy.Bubble,
            typeof(DragCompletedEventHandler),
            typeof(MyThumb));

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(MyThumb),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

}
