using Graph.Graph;
using Newtonsoft.Json.Linq;
using SpecificControls.Graph;
using System.IO;

namespace TrackEditor.Operations;

internal static class LoadTrackOperation
{
    public static void LoadTrack(string path, double scaleX, double scaleY)
    {
        var obj = JObject.Parse(File.ReadAllText(path));
        var graphs = TrackLoader.ReadGraph(obj, scaleX, scaleY);

        var infos = graphs.Select(a => new GraphInfo(new(a)));
        foreach (var info in infos)
        {
            MainEditor.Instance.GraphInfos.Add(info);
        }
        if (infos.Any())
            MainEditor.Instance.SelectedGraph = infos.First();
    }
}
