using System;
using Microsoft.Xna.Framework.Input;

namespace MonoGameUI;

public class GamepadInputController
{
    /// <summary>
    /// Stores the current gamepad input state.
    /// </summary>
    public ButtonState CurrentState { get; private set; }

    /// <summary>
    /// Stores the previous state to detect gamepad input events.
    /// </summary>
    public ButtonState PreviousState { get; private set; }
}
