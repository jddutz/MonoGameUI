using System;

namespace MonoGameUI.Elements;

public enum LayoutMode
{
    // Aligned to the start (left or top unless layout direction is ReverseDirection)
    START,

    // Aligned to the end (right or bottom unless layout direction is ReverseDirection)
    END,

    // Centered, with fixed space between children
    CENTER,

    // Centered, with space between children distributed evenly
    // Layout spacing is ignored. Children may overlap.
    EQUAL
}
