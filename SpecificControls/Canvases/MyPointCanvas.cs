using Controls;
using Controls.Canvases;
using Controls.Collection;
using Graph.Graph;
using Graph.Graph.Reference;
using SpecificControls.Selection;

namespace SpecificControls.Canvases;

public partial class MyPointCanvas: PointCanvas
{
    public GraphChanger? Target
    {
        get => (GraphChanger)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public bool ShowHandle
    {
        get => (bool)GetValue(ShowHandleProperty);
        set => SetValue(ShowHandleProperty, value);
    }

    public bool AnchorMovable
    {
        get => (bool) GetValue(AnchorMovableProperty);
        set => SetValue(AnchorMovableProperty, value);
    }

    private SyncMapCollection<AnchorReference, TracablePoint>? _syncMap;

    private MyTracablePoint? _pLeft;

    private MyTracablePoint? _pRight;

    private bool _handleAdded;

    public MyPointCanvas()
    {
        PreviewPositionChange += MoveHandlesWithAnchorPreview;
        SelectedUpdate += MyPointCanvas_SelectedUpdate;
    }

    private void MoveHandlesWithAnchorPreview(double arg1, double arg2)
    {
        if (_pLeft != null) HelpMoveThumb(_pLeft, arg1, arg2);
        if (_pRight != null) HelpMoveThumb(_pRight, arg1, arg2);
    }

    private void MyPointCanvas_SelectedUpdate(object sender, SelectedUpdateEventArgs e)
    {
        ClearHandle();
        if (e.Selected.Length != 1 ||
            e.Selected[0] is not MyTracablePoint point) return;
        InitHandle(point);
        if (!ShowHandle) return;
        AddHandle();
    }

    private void UpdateTarget()
    {
        _syncMap?.Dispose();
        if (Target is null) return;
        _syncMap = new(Target.Graph.NotifyAnchorCollection,
            Target.Graph.AnchorCollection, PointList,
            a => new MyTracablePoint(a, Target, PositionType.Position, GetMovable),
            true)
        {
            CleanUp = CleanUpAnchor
        };
        _syncMap.Rebuild();
    }

    private static void CleanUpAnchor(TracablePoint point)
    {
        if (point is not MyTracablePoint myPoint) return;
        myPoint.Dispose();
    }

    private void ClearHandle()
    {
        RemoveHandle();
        _pLeft?.Dispose();
        _pRight?.Dispose();
        _pLeft = null;
        _pRight = null;
    }

    private void InitHandle(MyTracablePoint point)
    {
        _pLeft = new MyTracablePoint(point._anchor, point._graph,
                                      PositionType.PLast, GetMovable);
        _pRight = new MyTracablePoint(point._anchor, point._graph,
                                      PositionType.PNext, GetMovable);
    }

    private void RemoveHandle()
    {
        if (!_handleAdded) return;
        if (_pLeft != null) RemoveThumb(_pLeft);
        if (_pRight != null) RemoveThumb(_pRight);
        _handleAdded = false;
    }

    private void AddHandle()
    {
        if (_handleAdded) return;
        if (_pLeft != null && _pRight != null)
        {
            AddThumb(_pLeft); AddThumb(_pRight);
            _handleAdded = true;
        }
    }

    private bool GetMovable() => AnchorMovable;
}
