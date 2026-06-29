using Controls;
using Controls.Canvases;
using Controls.Collection;
using Graph.Graph;
using Graph.Graph.Reference;
using SpecificControls.Selection;

namespace SpecificControls.Canvases;

public partial class MyCurveCanvas: CurveCanvas
{
    public GraphChanger? Target
    {
        get => (GraphChanger)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    private SyncMapCollection<SegmentReference, TracableSegment>? _syncMap;

    private void UpdateTarget()
    {
        _syncMap?.Dispose();
        if (Target is null) return;
        _syncMap = new(
            Target.Graph.NotifySegmentCollection,
            Target.Graph.SegmentCollection, SegmentList,
            a => new MyTracableSegment(a, Target),
            true)
        {
            CleanUp = CleanUpSegment
        };
        _syncMap.Rebuild();
    }

    private static void CleanUpSegment(TracableSegment segment)
    {
        if (segment is not MyTracableSegment mySegment) return;
        mySegment.Dispose();
    }

    protected override IEnumerable<TracableSegment> GetSegments(TracableSegment start)
    {
        if (start is not MyTracableSegment segment) return base.GetSegments(start);
        var reference = segment.SegmentReference;
        if (Target != null && _syncMap != null)
            return _syncMap.GetTargets(Target.Graph.GetSegmentsRelated(reference));
        return [start];
    }
}
