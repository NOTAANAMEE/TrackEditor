using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Basic;

internal class AnchorSegment(Anchor from, Anchor to)
{
    public Anchor From => from;
    public Anchor To => to;

    public BBezierSegment Segment => new(from.Position, from.PNext, to.PLast, to.Position);
}
