using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameUI;

public class MouseInputController
{
    /// <summary>
    /// Stores the current mouse state.
    /// </summary>
    public MouseState CurrentState { get; private set; }

    /// <summary>
    /// Stores the previous mouse state to detect mouse events.
    /// </summary>
    public MouseState PreviousState { get; private set; }

    public float MouseInputTimer { get; set; } = 0f;

    public float DoubleClickSensitivity = 0.25f;

    public Point DragStartPosition { get; private set; }


    // Flag indicating the mouse moved since the previous frame
    public bool MouseMoved => CurrentState.Position != PreviousState.Position;

    // Flags indicating changes to the state of the mouse buttons
    public bool ButtonDown => CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
    public bool ButtonUp => CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
    public bool RightButtonDown => CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
    public bool RightButtonUp => CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
    public bool MiddleButtonDown => CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
    public bool MiddleButtonUp => CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;

    // Flags indicating a mouse double-click event occurred
    public bool DoubleClick => ButtonDown && MouseInputTimer < DoubleClickSensitivity;
    public bool RightDoubleClick => RightButtonDown && MouseInputTimer < DoubleClickSensitivity;
    public bool MiddleDoubleClick => MiddleButtonDown && MouseInputTimer < DoubleClickSensitivity;

    // Flags indicating a mouse scroll event occurred
    public bool MouseScroll => CurrentState.ScrollWheelValue != PreviousState.ScrollWheelValue;
    public int ScrollDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    // Start dragging when the left mouse button is held down for longer than the double-click sensitivity
    public bool Dragging => CurrentState.LeftButton == ButtonState.Pressed && MouseInputTimer > DoubleClickSensitivity;

    /// <summary>
    /// Called once per frame to detect mouse input.
    /// </summary>
    /// <param name="dt">Time in seconds since the previous frame.</param>
    public virtual void Update(float dt)
    {
        // Update the mouse input timer
        // As a float, we don't need to consider overflow
        MouseInputTimer += dt;

        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();

        // On button down, initalize drag and restart the mouse input timer
        if (ButtonDown)
        {
            MouseInputTimer = 0f;
            DragStartPosition = CurrentState.Position;
        }

        if (RightButtonDown) MouseInputTimer = 0f;
        if (MiddleButtonDown) MouseInputTimer = 0f;
    }
}