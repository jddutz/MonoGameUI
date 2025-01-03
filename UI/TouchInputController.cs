using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonoGameUI;

public class TouchInputController
{
    /// <summary>
    /// Stores the current touch state.
    /// </summary>
    protected TouchCollection TouchState { get; private set; }

    /// <summary>
    /// Stores the previous touch state to detect touch events.
    /// </summary>
    protected TouchCollection PreviousTouchState { get; private set; }

    protected float TouchInputTimer { get; set; } = 0f;

    public bool Dragging { get; protected set; } = false;

    public float DoubleClickSensitivity = 0.25f;

    public Point TouchDragStart { get; protected set; }

    public bool TouchMoved { get; protected set; } = false;
    public bool TouchDown { get; protected set; } = false;
    public bool TouchUp { get; protected set; } = false;
    public bool Tap { get; protected set; } = false;
    public bool DoubleTap { get; protected set; } = false;
}