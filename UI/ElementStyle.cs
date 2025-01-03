using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameUI.Elements;

namespace MonoGameUI;

public class ElementStyle
{
    public Color BackgroundColor { get; set; } = Color.Transparent;
    public Color ForegroundColor { get; set; } = Color.White;
    public BorderStyle BorderStyle { get; set; } = new();
}
