using Microsoft.Xna.Framework.Input;

namespace MonoGameUI;

public class KeyboardInputController
{
    /// <summary>
    /// Stores the current keyboard state.
    /// </summary>
    public KeyboardState CurrentState { get; private set; }

    /// <summary>
    /// Stores the previous state to detect keyboard input events.
    /// </summary>
    public KeyboardState PreviousState { get; private set; }
}