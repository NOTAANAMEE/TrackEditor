using Graph.Graph;
using Newtonsoft.Json.Linq;
using SpecificControls.Graph;
using System.IO;

namespace TrackEditor.Operations;

internal static class LoadTrackOperation
{
    public static void LoadTrack(string path, double scaleX, double scaleY)
    {
        var arr = JArray.Parse(File.ReadAllText(path));
        var graph = new GraphChanger(
                TrackLoader.ReadGraph(arr, scaleX, scaleY));
        var info = new GraphInfo(graph);
        MainEditor.Instance.GraphInfos.Add(info);
        MainEditor.Instance.SelectedGraph = info;
    }
}
