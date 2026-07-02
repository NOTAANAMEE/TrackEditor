using Graph.Basic;
using Graph.Graph;
using Graph.Graph.Reference;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TrackEditor;

public static class TrackLoader
{
    public static IEnumerable<BezierGraph> ReadGraph(JObject obj, double scaleX = 1, double scaleY = 1)
    {
        var paths = obj.Value<JArray>("paths")?.OfType<JObject>() ?? [];

        var ret = new List<BezierGraph>(paths.Count());
        foreach (var path in paths)
        {
            var graph = new BezierGraph();
            var points = path["points"] as JArray
            ?? throw new ArgumentException("Track JSON object must contain a points array.");
            var closed = path["closed"]?.Value<bool>() ?? true;
            ReadGraph(graph, points, scaleX, scaleY, closed);
            ret.Add(graph);
        }
        return ret;
    }

    public static void ReadGraph(BezierGraph graph, JArray arr, double scaleX, double scaleY, bool closed)
    {
        var anchorReferences = ReadPointList(arr, scaleX, scaleY);
        foreach (var r in anchorReferences) graph.AddAnchor(r);
        var segmentCount = closed ? anchorReferences.Count : anchorReferences.Count - 1;
        for (int i = 0; i < segmentCount; i++)
        {
            var p0 = anchorReferences[i];
            var p1 = anchorReferences[(i + 1) % anchorReferences.Count];
            graph.AddSegment(new (p0, p1));
        }
    }

    public static List<AnchorReference> ReadPointList(JArray arr, double scaleX = 1, double scaleY = 1)
    {
        var AnchorList = new List<AnchorReference>();
        foreach (var item in arr)
        {
            if (item is JObject obj)
            {
                AnchorList.Add(ReadAnchor(obj, scaleX, scaleY));
            }
        }
        return AnchorList;
    }

    public static AnchorReference ReadAnchor(JObject obj, double scaleX = 1, double scaleY = 1)
    {
        var pos = ReadBPoint(obj["position"] as JArray ?? [], scaleX, scaleY);
        var left = ReadBPoint(obj["left"] as JArray ?? [], scaleX, scaleY);
        var right = ReadBPoint(obj["right"] as JArray ?? [], scaleX, scaleY);
        return new AnchorReference(pos, left, right);
    }

    public static BPoint ReadBPoint(JArray array, double scaleX = 1, double scaleY = 1)
    {
        return new BPoint(array[0].Value<double>() * scaleX, array[1].Value<double>() * scaleY);
    }
}
