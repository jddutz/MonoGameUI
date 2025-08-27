using Microsoft.Xna.Framework;
using MonoGameUI.Core;

namespace MonoGameUI.Components;

/// <summary>
/// Button states for visual feedback and behavior.
/// </summary>
public enum ButtonState
{
    Normal,
    Hover,
    Pressed,
    Disabled
}

/// <summary>
/// Component that provides interactive button behavior.
/// Works in conjunction with InputComponent and RenderableComponent.
/// </summary>
public class ButtonComponent : Core.Component
{
    private ButtonState _state = ButtonState.Normal;
    private bool _isPressed = false;
    private bool _isHovered = false;

    /// <summary>
    /// Current visual and interaction state of the button.
    /// </summary>
    public ButtonState State
    {
        get => _state;
        private set
        {
            if (_state != value)
            {
                var oldState = _state;
                _state = value;
                StateChanged?.Invoke(oldState, value);
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Whether the button is currently being pressed.
    /// </summary>
    public bool IsPressed
    {
        get => _isPressed;
        private set
        {
            if (_isPressed != value)
            {
                _isPressed = value;
                UpdateState();
            }
        }
    }

    /// <summary>
    /// Whether the mouse is currently hovering over the button.
    /// </summary>
    public bool IsHovered
    {
        get => _isHovered;
        private set
        {
            if (_isHovered != value)
            {
                _isHovered = value;
                UpdateState();
            }
        }
    }

    /// <summary>
    /// Text to display on the button (if using text rendering).
    /// </summary>
    public string Text { get; set; } = string.Empty;

    #region Events

    /// <summary>
    /// Raised when the button is clicked.
    /// </summary>
    public event Action<ButtonComponent>? Click;

    /// <summary>
    /// Raised when the button state changes.
    /// </summary>
    public event Action<ButtonState, ButtonState>? StateChanged;

    /// <summary>
    /// Raised when the button is pressed down.
    /// </summary>
    public event Action<ButtonComponent>? MouseDown;

    /// <summary>
    /// Raised when the button is released.
    /// </summary>
    public event Action<ButtonComponent>? MouseUp;

    /// <summary>
    /// Raised when the mouse enters the button area.
    /// </summary>
    public event Action<ButtonComponent>? MouseEnter;

    /// <summary>
    /// Raised when the mouse exits the button area.
    /// </summary>
    public event Action<ButtonComponent>? MouseExit;

    #endregion

    /// <summary>
    /// Called when the component is attached to an entity.
    /// Sets up input event handlers.
    /// </summary>
    public override void OnAttached()
    {
        base.OnAttached();

        if (Entity?.GetComponent<InputComponent>() is InputComponent input)
        {
            SetupInputHandlers(input);
        }
    }

    /// <summary>
    /// Set up input event handlers with the InputComponent.
    /// </summary>
    private void SetupInputHandlers(InputComponent input)
    {
        input.MouseEnter += OnMouseEnter;
        input.MouseExit += OnMouseExit;
        input.MouseDown += OnMouseDown;
        input.MouseUp += OnMouseUp;
        input.MouseClick += OnMouseClick;
    }

    /// <summary>
    /// Clean up input event handlers.
    /// </summary>
    public override void OnDetached()
    {
        if (Entity?.GetComponent<InputComponent>() is InputComponent input)
        {
            input.MouseEnter -= OnMouseEnter;
            input.MouseExit -= OnMouseExit;
            input.MouseDown -= OnMouseDown;
            input.MouseUp -= OnMouseUp;
            input.MouseClick -= OnMouseClick;
        }

        base.OnDetached();
    }

    #region Input Event Handlers

    private void OnMouseEnter(MouseInputEvent e)
    {
        IsHovered = true;
        MouseEnter?.Invoke(this);
    }

    private void OnMouseExit(MouseInputEvent e)
    {
        IsHovered = false;
        IsPressed = false; // Release press if mouse leaves
        MouseExit?.Invoke(this);
    }

    private void OnMouseDown(MouseInputEvent e)
    {
        if (e.Button == 0) // Left mouse button
        {
            IsPressed = true;
            MouseDown?.Invoke(this);
            e.Handled = true;
        }
    }

    private void OnMouseUp(MouseInputEvent e)
    {
        if (e.Button == 0) // Left mouse button
        {
            IsPressed = false;
            MouseUp?.Invoke(this);
            e.Handled = true;
        }
    }

    private void OnMouseClick(MouseInputEvent e)
    {
        if (e.Button == 0) // Left mouse button
        {
            PerformClick();
            e.Handled = true;
        }
    }

    #endregion

    /// <summary>
    /// Programmatically trigger a button click.
    /// </summary>
    public void PerformClick()
    {
        if (Enabled && State != ButtonState.Disabled)
        {
            Click?.Invoke(this);
        }
    }

    /// <summary>
    /// Update the button state based on current interaction flags.
    /// </summary>
    private void UpdateState()
    {
        if (!Enabled)
        {
            State = ButtonState.Disabled;
        }
        else if (IsPressed)
        {
            State = ButtonState.Pressed;
        }
        else if (IsHovered)
        {
            State = ButtonState.Hover;
        }
        else
        {
            State = ButtonState.Normal;
        }
    }

    /// <summary>
    /// Enable or disable the button.
    /// </summary>
    public new bool Enabled
    {
        get => base.Enabled;
        set
        {
            if (base.Enabled != value)
            {
                base.Enabled = value;
                UpdateState();
            }
        }
    }

    /// <summary>
    /// Get style properties for the current button state.
    /// Can be used to update visual appearance based on state.
    /// </summary>
    public (Color backgroundColor, Color textColor, float opacity) GetStateColors()
    {
        return State switch
        {
            ButtonState.Normal => (Color.LightGray, Color.Black, 1.0f),
            ButtonState.Hover => (Color.Gray, Color.Black, 1.0f),
            ButtonState.Pressed => (Color.DarkGray, Color.Black, 1.0f),
            ButtonState.Disabled => (Color.LightGray, Color.Gray, 0.5f),
            _ => (Color.LightGray, Color.Black, 1.0f)
        };
    }

    /// <summary>
    /// Apply current state styling to associated components.
    /// </summary>
    public void ApplyStateToComponents()
    {
        var (bgColor, textColor, opacity) = GetStateColors();

        // Update StyleComponent if present
        if (Entity?.GetComponent<StyleComponent>() is StyleComponent style)
        {
            style.BackgroundColor = bgColor;
            style.Opacity = opacity;
        }

        // Update RenderableComponent if present
        if (Entity?.GetComponent<RenderableComponent>() is RenderableComponent renderable)
        {
            if (renderable.RenderType == RenderType.Text)
            {
                renderable.Color = textColor;
            }
        }
    }

    /// <summary>
    /// Set the button text and update the RenderableComponent.
    /// </summary>
    public void SetText(string text)
    {
        Text = text;

        if (Entity?.GetComponent<RenderableComponent>() is RenderableComponent renderable)
        {
            renderable.Text = text;
        }
    }
}
