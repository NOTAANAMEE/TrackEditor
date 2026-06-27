using Graph.Basic;
using Graph.Graph.Reference;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph;

public enum PositionType
{
    Position,
    PLast,
    PNext
}

public record PositionChanger(
    AnchorReference reference, PositionType positionType, BPoint position);
