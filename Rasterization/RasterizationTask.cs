using System;
using System.Collections.Generic;
using System.Text;

namespace Rasterization;

public static class RasterizationTask
{
    public static void Rasterize(PointsAndSegments graph, int y, int actualy, int start, int width, bool[,] result)
    {
        var segs = graph.IntersectsHorizontal(y + actualy + 0.5);
        var segIenumerator = segs.GetEnumerator();
        var inside = false;
        var hasNext = segIenumerator.MoveNext();
        for (int i = 0; i < width; i++)
        {
            while (hasNext && segIenumerator.Current.Coord <= i + start + 0.5)
            {
                inside = !inside;
                hasNext = segIenumerator.MoveNext();
            }
            result[i, y] = inside;
        }
    }
}
