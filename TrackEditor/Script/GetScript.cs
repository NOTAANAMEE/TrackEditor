using System;
using System.Collections.Generic;
using System.Text;

namespace TrackEditor.Script;

public class GetScript
{
    public static string Script = @"
import bpy
import json

EXPORT_WORLD_SPACE = True

def to_xy(v):
    return [v.x, v.y]

def transform_point(obj, point):
    if EXPORT_WORLD_SPACE:
        return obj.matrix_world @ point
    return point

paths = []

objects = bpy.context.selected_objects

if not objects:
    raise RuntimeError(""Please select at least one Curve object."")

for obj in objects:
    if obj.type != ""CURVE"":
        continue

    curve = obj.data

    for index, spline in enumerate(curve.splines):
        if spline.type != ""BEZIER"":
            continue

        points = []

        for bp in spline.bezier_points:
            points.append({
                ""position"": to_xy(transform_point(obj, bp.co)),
                ""left"": to_xy(transform_point(obj, bp.handle_left)),
                ""right"": to_xy(transform_point(obj, bp.handle_right))
            })

        paths.append({
            ""name"": f""{obj.name}.{index}"",
            ""closed"": spline.use_cyclic_u,
            ""points"": points
        })

if not paths:
    raise RuntimeError(""No Bezier paths found in selected Curve objects."")

result = {
    ""paths"": paths
}

path = bpy.path.abspath(""//curves.json"")

with open(path, ""w"", encoding=""utf-8"") as f:
    json.dump(result, f, indent=2)

print(f""Exported {len(paths)} paths:"", path)";

}
