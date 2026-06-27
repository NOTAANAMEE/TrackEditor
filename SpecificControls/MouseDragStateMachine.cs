using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpecificControls;

public class MouseDragStateMachine
{
    MouseButton _button;

    Control _control;

    bool _isDragging;

    Point _start;

    Point _last;

    public MouseDragStateMachine(Control control, MouseButton mouse)
    {
        control.MouseDown += Control_MouseDown;
        control.MouseMove += Control_MouseMove;
        control.MouseUp += Control_MouseUp;
        _button = mouse;
        _control = control;
    }

    private void Control_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _control.ReleaseMouseCapture();
        _isDragging = false;
    }

    public Action<Vector>? DragMove;

    private void Control_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging) return;
        var current = e.GetPosition(_control);
        var delta = current - _last;
        _last = current;
        DragMove?.Invoke(delta);
        if (current.X > _control.Width || current.X < 0
            || current.Y > _control.Height || current.Y < 0)
        {
            _control.ReleaseMouseCapture();
            _isDragging = false;
        }
            
    }

    private void Control_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != _button) return;
        _isDragging = true;
        _last = e.GetPosition(_control);
        _start = _last;
        _control.CaptureMouse();
    }
}
