using Graph.Basic;
using Graph.Graph;
using Graph.Graph.Reference;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackEditor;

public static class TrackLoader
{
    public static BezierGraph ReadGraph(JArray arr, double scaleX = 1, double scaleY = 1)
    {
        var anchorReferences = ReadPointList(arr, scaleX, scaleY);
        var graph = new BezierGraph();
        foreach (var r in anchorReferences) graph.AddAnchor(r);
        for (int i = 0; i < anchorReferences.Count; i++)
        {
            var p0 = anchorReferences[i];
            var p1 = anchorReferences[(i + 1) % anchorReferences.Count];
            graph.AddSegment(new (p0, p1));
        }
        return graph;
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
