using Graph.Graph;
using Graph.Graph.Reference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Controls.Selection;

public class Editor
{
    public event EventHandler<HashSet<AnchorReference>>? AnchorPositionChanged;

    protected BezierGraph? Graph { get; private set; }

    internal void SetGraph(BezierGraph graph)
    {
        Graph = new BezierGraph();
        Graph.AnchorPositionChanged += AnchorPositionChanged;
    }

    internal void RemoveGraph()
    {
        if (Graph == null) return;
        Graph.AnchorPositionChanged -= AnchorPositionChanged;
        Graph = null;
    }

    public void MovePoint(AnchorReference anchor, Vector delta)
    {

    }

    public void MovePointss(AnchorReference[] anchors, Vector delta) 
    {
        
    }
}
