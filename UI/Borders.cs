using Microsoft.Xna.Framework;

namespace MonoGameUI;

public static class Borders
{
    public static Color DefaultBorderColor => Color.White;

    public static BorderStyle Default => new()
    {
        Color = DefaultBorderColor,
        Thickness = [2, 2, 2, 2]
    };
    public static BorderStyle Thin => new()
    {
        Color = DefaultBorderColor,
        Thickness = [1, 1, 1, 1]
    };

    public static BorderStyle Top => new()
    {
        Color = DefaultBorderColor,
        Thickness = [2, 0, 0, 0]
    };

    public static BorderStyle Right => new()
    {
        Color = DefaultBorderColor,
        Thickness = [0, 2, 0, 0]
    };

    public static BorderStyle Bottom => new()
    {
        Color = DefaultBorderColor,
        Thickness = [0, 0, 2, 0]
    };

    public static BorderStyle Left => new()
    {
        Color = DefaultBorderColor,
        Thickness = [0, 0, 0, 2]
    };

    public static BorderStyle TopRight => new()
    {
        Color = DefaultBorderColor,
        Thickness = [2, 2, 0, 0]
    };

    public static BorderStyle BottomRight => new()
    {
        Color = DefaultBorderColor,
        Thickness = [0, 2, 2, 0]
    };

    public static BorderStyle BottomLeft => new()
    {
        Color = DefaultBorderColor,
        Thickness = [0, 0, 2, 2]
    };

    public static BorderStyle TopLeft => new()
    {
        Color = DefaultBorderColor,
        Thickness = [2, 0, 0, 2]
    };

    public static BorderStyle Vertical => new()
    {
        Color = DefaultBorderColor,
        Thickness = [2, 0, 2, 0]
    };

    public static BorderStyle Horizontal => new()
    {
        Color = DefaultBorderColor,
        Thickness = [0, 2, 0, 2]
    };
}
