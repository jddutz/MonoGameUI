using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameUI.Elements;

namespace MonoGameUI;

/// <summary>
/// Defines the appearance of UI elements.
/// A Theme is a collection of ElementStyles
/// and rules for applying them to UI elements.
/// </summary>
public class Theme : ITheme
{
    public ElementStyle DefaultElementStyle { get; set; } = new ElementStyle();
    public Dictionary<string, ElementStyle> ElementStyles { get; private set; } = [];
    public List<IThemeRule> Rules { get; private set; } = [];
}

/*
    public Dictionary<int, Color> BackgroundColors { get; set; } = new()
    {
        { ControlState.NORMAL, Color.Transparent },
        { ControlState.HOVER, new Color(42,42,42,255) },
        { ControlState.SELECTED, Color.Silver },
        { ControlState.INACTIVE, Color.Black }
    };

    public Dictionary<int, Color> ForegroundColors { get; set; } = new()
    {
        { ControlState.NORMAL, Color.White },
        { ControlState.HOVER, Color.White },
        { ControlState.SELECTED, new Color(85,85,85,255) },
        { ControlState.INACTIVE, new Color(85,85,85,255) }
    };

    public Dictionary<int, Color> BorderColors { get; set; } = new()
    {
        { ControlState.NORMAL, Color.White },
        { ControlState.HOVER, Color.White },
        { ControlState.SELECTED, new Color(85,85,85,255) },
        { ControlState.INACTIVE, new Color(85,85,85,255) }
    };
    */