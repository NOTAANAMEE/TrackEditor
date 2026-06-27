using Controls;
using Graph.Graph;
using SpecificControls.Editor;
using SpecificControls.Graph;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpecificControls
{
    /// <summary>
    /// Interaction logic for BezierEditor.xaml
    /// </summary>
    public partial class BezierEditor : UserControl
    {
        public GraphInfo? SelectedGraph
        {
            get => (GraphInfo)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        public double ScaleX
        {
            get => (double)GetValue(ScaleXProperty);
            set => SetValue(ScaleXProperty, value);
        }

        public double ScaleY
        {
            get => (double)GetValue(ScaleYProperty);
            set => SetValue(ScaleYProperty, value);
        }

        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public EditorState? EditorState
        {
            get => (EditorState)GetValue(EditorStateProperty);
            set => SetValue(EditorStateProperty, value);
        }

        private EditorMode Mode => EditorState?.Mode ?? EditorMode.None;

        private MouseDragStateMachine _machine;


        public BezierEditor()
        {
            InitializeComponent();
            _machine = new(this, MouseButton.Right);
            _machine.DragMove += MouseDragMove;
        }

        private void MouseDragMove(Vector vector)
        {
            var x = vector.X / ScaleX;
            var y = vector.Y / ScaleY;
            Center -= new Vector(x, y);
        }

        private void Canvas_OnClick(object? sender, MouseEventArgs e)
        {
            if (EditorState == null || SelectedGraph == null) return;
            var anchorSelect = EditorState.Mode.HasFlag(EditorMode.AnchorOriented);
            IEnumerable<Selectable> selectables;
            if (anchorSelect) selectables = PCanvas.SelectedPoints.Select(a => a);
            else selectables = CCanvas.SelectedSegments.Select(a => a);
            EditorState.HandleMouseClick(this, new EditorClickArgs(
                SelectedGraph.Graph, selectables, CanvasToWorldPoint(e.GetPosition(this)))
                );
        }

        private void SetMode()
        {
            var mode = Mode;
            SetOriented(mode);
            SetVisibility(mode);
            SetEnableSelect(mode);
            SetMultiSelect(mode);
            SetShowHandle(mode);
            SetAnchorMovable(mode);
        }

        private void SetOriented(EditorMode mode)
        {
            var _bool = mode.HasFlag(EditorMode.AnchorOriented);
            var pInd = _bool? 1 : 0;
            var cInd = 1 - pInd;
            Panel.SetZIndex(PCanvas, pInd);
            Panel.SetZIndex(CCanvas, cInd);
        }

        private void SetVisibility(EditorMode mode)
        {
            var showA = mode.HasFlag(EditorMode.ShowAnchors);
            var showS = mode.HasFlag(EditorMode.ShowSegments);
            PCanvas.Visibility = showA? Visibility.Visible : Visibility.Collapsed;
            CCanvas.Visibility = showS? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetMultiSelect(EditorMode mode)
        {
            PCanvas.MultipleSelect = mode.HasFlag(EditorMode.MultiSelectable);
            CCanvas.MultipleSelect = mode.HasFlag(EditorMode.MultiSelectable);
        }

        private void SetEnableSelect(EditorMode mode)
        {
            PCanvas.EnableSelect = mode.HasFlag(EditorMode.Selectable);
            CCanvas.EnableSelect = mode.HasFlag(EditorMode.Selectable);
        }

        private void SetShowHandle(EditorMode mode)
            => PCanvas.ShowHandle = mode.HasFlag(EditorMode.ShowHandles);

        private void SetAnchorMovable(EditorMode mode)
            => PCanvas.AnchorMovable = mode.HasFlag(EditorMode.Movable);

        private Point WorldToCanvasPoint(Point worldPos)
        {
            var canvasCenterX = ActualWidth / 2;
            var canvasCenterY = ActualHeight / 2;
            var x = (worldPos.X - Center.X) * ScaleX + canvasCenterX;
            var y = (worldPos.Y - Center.Y) * ScaleY + canvasCenterY;
            return new Point(x, y);
        }

        private Point CanvasToWorldPoint(Point canvasPos)
        {
            var canvasCenterX = ActualWidth / 2;
            var canvasCenterY = ActualHeight / 2;
            var x = (canvasPos.X - canvasCenterX) / ScaleX + Center.X;
            var y = (canvasPos.Y - canvasCenterY) / ScaleY + Center.Y;
            return new Point(x, y);
        }

        private void Canvas_SelectedUpdate(object sender, SelectedUpdateEventArgs e)
        {
            if (EditorState == null || SelectedGraph == null) return;
            EditorState?.HandleSelectionChange(sender, 
                new EditorArgs(SelectedGraph.Graph, e.Selected));
        }

        public void RunCommand(string signalName)
            => RunCommand(signalName, null);

        public void RunCommand(string signalName, object? parameter)
        {
            if (EditorState == null || SelectedGraph == null) return;
            if (EditorState.Mode.HasFlag(EditorMode.AnchorOriented))
            {
                EditorState?.HandleSignal(this, new(
                SelectedGraph.Graph, PCanvas.SelectedPoints.ToHashSet<Selectable>(), signalName,
                parameter));
            }
            else
            {
                EditorState?.HandleSignal(this, new(
                SelectedGraph.Graph, CCanvas.SelectedSegments.ToHashSet<Selectable>(), signalName,
                parameter));
            }
        }

        private void Root_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var factor = e.Delta > 0 ? 1.1 : 1 / 1.1;
            ScaleX *= factor;
            ScaleY *= factor;
        }

        private void Root_KeyDown(object sender, KeyEventArgs e)
        {
            if (EditorState == null || SelectedGraph == null) return;
            if (EditorState.Mode.HasFlag(EditorMode.AnchorOriented))
            {
                EditorState?.HandleKey(this, new(
                SelectedGraph.Graph, PCanvas.SelectedPoints.ToHashSet<Selectable>(), e));
            }
            else
            {
                EditorState?.HandleKey(this, new(
                SelectedGraph.Graph, CCanvas.SelectedSegments.ToHashSet<Selectable>(), e));
            }
        }

        private void Root_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            Keyboard.Focus(this);
        }
    }
}
