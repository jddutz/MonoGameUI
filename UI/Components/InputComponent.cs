using Microsoft.Xna.Framework;
using MonoGameUI.Core;

namespace MonoGameUI.Components;

/// <summary>
/// Priority levels for input handling.
/// </summary>
public enum InputPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}

/// <summary>
/// Types of mouse input events.
/// </summary>
public enum MouseEventType
{
    Enter,
    Exit,
    Move,
    Down,
    Up,
    Click,
    DoubleClick,
    Wheel
}

/// <summary>
/// Mouse input event data.
/// </summary>
public class MouseInputEvent
{
    public MouseEventType Type { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 WorldPosition { get; set; }
    public int Button { get; set; } // 0 = left, 1 = right, 2 = middle
    public int WheelDelta { get; set; }
    public bool Handled { get; set; }
}

/// <summary>
/// Keyboard input event data.
/// </summary>
public class KeyboardInputEvent
{
    public Microsoft.Xna.Framework.Input.Keys Key { get; set; }
    public char Character { get; set; }
    public bool IsKeyDown { get; set; }
    public bool IsRepeat { get; set; }
    public bool Handled { get; set; }
}

/// <summary>
/// Component responsible for handling user input events (mouse, keyboard, touch).
/// This replaces input functionality from the legacy Element system.
/// </summary>
public class InputComponent : Core.Component
{
    private bool _canReceiveFocus = true;
    private bool _hasFocus = false;
    private InputPriority _priority = InputPriority.Normal;
    private bool _acceptsKeyboardInput = false;
    private bool _acceptsMouseInput = true;

    /// <summary>
    /// Whether this element can receive keyboard focus.
    /// </summary>
    public bool CanReceiveFocus
    {
        get => _canReceiveFocus;
        set
        {
            if (_canReceiveFocus != value)
            {
                _canReceiveFocus = value;
                if (!value && _hasFocus)
                {
                    HasFocus = false;
                }
            }
        }
    }

    /// <summary>
    /// Whether this element currently has keyboard focus.
    /// </summary>
    public bool HasFocus
    {
        get => _hasFocus;
        set
        {
            if (_hasFocus != value && (_canReceiveFocus || !value))
            {
                _hasFocus = value;
                if (value)
                {
                    FocusGained?.Invoke();
                }
                else
                {
                    FocusLost?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Input priority for event handling order.
    /// </summary>
    public InputPriority Priority
    {
        get => _priority;
        set => _priority = value;
    }

    /// <summary>
    /// Whether this element accepts keyboard input.
    /// </summary>
    public bool AcceptsKeyboardInput
    {
        get => _acceptsKeyboardInput;
        set => _acceptsKeyboardInput = value;
    }

    /// <summary>
    /// Whether this element accepts mouse input.
    /// </summary>
    public bool AcceptsMouseInput
    {
        get => _acceptsMouseInput;
        set => _acceptsMouseInput = value;
    }

    #region Mouse Events

    /// <summary>
    /// Raised when the mouse enters the element bounds.
    /// </summary>
    public event Action<MouseInputEvent>? MouseEnter;

    /// <summary>
    /// Raised when the mouse exits the element bounds.
    /// </summary>
    public event Action<MouseInputEvent>? MouseExit;

    /// <summary>
    /// Raised when the mouse moves within the element bounds.
    /// </summary>
    public event Action<MouseInputEvent>? MouseMove;

    /// <summary>
    /// Raised when a mouse button is pressed down.
    /// </summary>
    public event Action<MouseInputEvent>? MouseDown;

    /// <summary>
    /// Raised when a mouse button is released.
    /// </summary>
    public event Action<MouseInputEvent>? MouseUp;

    /// <summary>
    /// Raised when a mouse button is clicked (down then up).
    /// </summary>
    public event Action<MouseInputEvent>? MouseClick;

    /// <summary>
    /// Raised when a mouse button is double-clicked.
    /// </summary>
    public event Action<MouseInputEvent>? MouseDoubleClick;

    /// <summary>
    /// Raised when the mouse wheel is scrolled.
    /// </summary>
    public event Action<MouseInputEvent>? MouseWheel;

    #endregion

    #region Keyboard Events

    /// <summary>
    /// Raised when the element gains keyboard focus.
    /// </summary>
    public event Action? FocusGained;

    /// <summary>
    /// Raised when the element loses keyboard focus.
    /// </summary>
    public event Action? FocusLost;

    /// <summary>
    /// Raised when a key is pressed down.
    /// </summary>
    public event Action<KeyboardInputEvent>? KeyDown;

    /// <summary>
    /// Raised when a key is released.
    /// </summary>
    public event Action<KeyboardInputEvent>? KeyUp;

    /// <summary>
    /// Raised when a character is typed.
    /// </summary>
    public event Action<KeyboardInputEvent>? CharacterTyped;

    #endregion

    #region Input Handling Methods

    /// <summary>
    /// Handle a mouse input event.
    /// Returns true if the event was handled and should not be passed to other elements.
    /// </summary>
    public bool HandleMouseInput(MouseInputEvent eventArgs)
    {
        if (!Enabled || !AcceptsMouseInput)
            return false;

        switch (eventArgs.Type)
        {
            case MouseEventType.Enter:
                MouseEnter?.Invoke(eventArgs);
                break;
            case MouseEventType.Exit:
                MouseExit?.Invoke(eventArgs);
                break;
            case MouseEventType.Move:
                MouseMove?.Invoke(eventArgs);
                break;
            case MouseEventType.Down:
                MouseDown?.Invoke(eventArgs);
                break;
            case MouseEventType.Up:
                MouseUp?.Invoke(eventArgs);
                break;
            case MouseEventType.Click:
                MouseClick?.Invoke(eventArgs);
                break;
            case MouseEventType.DoubleClick:
                MouseDoubleClick?.Invoke(eventArgs);
                break;
            case MouseEventType.Wheel:
                MouseWheel?.Invoke(eventArgs);
                break;
        }

        return eventArgs.Handled;
    }

    /// <summary>
    /// Handle a keyboard input event.
    /// Returns true if the event was handled and should not be passed to other elements.
    /// </summary>
    public bool HandleKeyboardInput(KeyboardInputEvent eventArgs)
    {
        if (!Enabled || !AcceptsKeyboardInput || !HasFocus)
            return false;

        if (eventArgs.IsKeyDown)
        {
            KeyDown?.Invoke(eventArgs);
        }
        else
        {
            KeyUp?.Invoke(eventArgs);
        }

        if (eventArgs.Character != '\0')
        {
            CharacterTyped?.Invoke(eventArgs);
        }

        return eventArgs.Handled;
    }

    /// <summary>
    /// Check if a world position is within this element's bounds.
    /// </summary>
    public bool ContainsPoint(Vector2 worldPosition)
    {
        var transform = Entity?.GetComponent<TransformComponent>();
        return transform?.ContainsPoint(worldPosition) ?? false;
    }

    /// <summary>
    /// Request keyboard focus for this element.
    /// </summary>
    public void RequestFocus()
    {
        if (CanReceiveFocus && Enabled)
        {
            // This would typically be handled by an InputSystem
            // For now, just set focus directly
            HasFocus = true;
        }
    }

    /// <summary>
    /// Release keyboard focus from this element.
    /// </summary>
    public void ReleaseFocus()
    {
        HasFocus = false;
    }

    #endregion

    /// <summary>
    /// Called when the component is detached from an entity.
    /// Releases focus if this component has it.
    /// </summary>
    public override void OnDetached()
    {
        ReleaseFocus();
        base.OnDetached();
    }
}
