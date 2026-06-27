using Graph.Basic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Graph.Graph;

internal class AnchorInfo(MyProperty property)
{
    [MemberNotNullWhen(true, nameof(NextProperty), nameof(NextAnchor))]
    public bool HasNext() => NextAnchor != null;

    [MemberNotNullWhen(true, nameof(LastAnchor))]
    public bool HasLast() => LastAnchor != null;

    public Anchor? NextAnchor { get; set; }
    
    public MyProperty? NextProperty;

    public Anchor? LastAnchor { get; set; }

    public MyProperty Property = property;
}
