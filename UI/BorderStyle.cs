using Microsoft.Xna.Framework;

namespace MonoGameUI;

public class BorderStyle
{
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// The border thickness of the element, in the order of top, right, bottom, left.
    /// </summary>
    private int[] thickness = [2, 2, 2, 2];
    public int[] Thickness
    {
        get => thickness;
        set
        {
            if (value.Length == 4)
            {
                thickness = value;
            }
        }
    }

    /// <summary>
    /// Sets the border to a uniform thickness.
    /// </summary>
    /// <param name="all">Thickness of the border on all sides.</param>
    public void SetThickness(int all)
    {
        if (Thickness[0] > 0)
            Thickness[0] = all;

        if (Thickness[1] > 0)
            Thickness[1] = all;

        if (Thickness[2] > 0)
            Thickness[2] = all;

        if (Thickness[3] > 0)
            Thickness[3] = all;
    }

    /// <summary>
    /// Sets the border thickness.
    /// </summary>
    /// <param name="top">Thickness of the top border.</param>
    /// <param name="right">Thickness of the right border.</param>
    /// <param name="bottom">Thickness of the bottom border.</param>
    /// <param name="left">Thickness of the left border.</param>
    public void SetThickness(int top, int right, int bottom, int left)
    {
        Thickness[0] = top;
        Thickness[1] = right;
        Thickness[2] = bottom;
        Thickness[3] = left;
    }
}
