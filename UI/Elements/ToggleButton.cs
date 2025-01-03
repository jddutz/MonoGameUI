using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

/// <summary>
/// A button that toggles between selected and normal states when clicked.
/// </summary>
public abstract class ToggleButton(UserInterface ui) : Button(ui)
{
    public bool IsSelected { get; set; } = false;

    public override void OnMouseClick()
    {
        if (CurrentState == ControlState.INACTIVE) return;

        IsSelected = !IsSelected;

        if (IsSelected)
        {
            CurrentState = ControlState.SELECTED;
        }
        else
        {
            CurrentState = ControlState.HOVER;
        }
    }

    public override void OnMouseExit()
    {
        if (CurrentState == ControlState.INACTIVE) return;

        if (IsSelected)
        {
            CurrentState = ControlState.SELECTED;
        }
        else
        {
            CurrentState = ControlState.NORMAL;
        }
    }
}