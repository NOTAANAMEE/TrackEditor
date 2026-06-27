using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Basic;

public static class StaticHelper
{
    public static List<BBezierSegment> Flattern(this List<BBezierSegment> segments)
    {
        var result = new List<BBezierSegment>();
        foreach (var segment in segments)
        {
            segment.Flattern(result);
        }
        return result;
    }

    public static List<Capsule> ToCapsule(this List<BBezierSegment> segments, double lineWidth)
    {
        var capsules = new List<Capsule>();
        foreach (var segment in segments)
        {
            capsules.Add(new Capsule(segment.P0, segment.P3, lineWidth / 2));
        }
        return capsules;
    }

    public static HashSet<BPoint> Rasterize(this List<Capsule> capsules)
    {
        var points = new HashSet<BPoint>();
        foreach (var capsule in capsules)
        {
            capsule.Rasterize(points);
        }
        return points;
    }
}
