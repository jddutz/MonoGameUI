using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameUI.Components;
using MonoGameUI.Core;
using XnaButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace MonoGameUI.Systems;

/// <summary>
/// System that processes InputComponent events and manages input routing, hit testing, and focus management.
/// </summary>
public class InputSystem : UISystem
{
    private MouseState _previousMouseState;
    private MouseState _currentMouseState;
    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;

    private UIEntity? _focusedEntity;
    private UIEntity? _hoveredEntity;
    private UIEntity? _capturedEntity; // Entity that captured mouse for dragging
    private readonly List<UIEntity> _inputEntities = new();

    public override int Priority => 50; // Process input before layout (100) and rendering (200)

    /// <summary>
    /// The currently focused entity for keyboard input.
    /// </summary>
    public UIEntity? FocusedEntity => _focusedEntity;

    /// <summary>
    /// The entity currently under the mouse cursor.
    /// </summary>
    public UIEntity? HoveredEntity => _hoveredEntity;

    public override void Update(float deltaTime)
    {
        if (!Enabled) return;

        // Update input states
        UpdateInputStates();

        // Collect all input-enabled entities
        CollectInputEntities();

        // Process mouse input
        ProcessMouseInput();

        // Process keyboard input
        ProcessKeyboardInput();

        // Update previous states for next frame
        _previousMouseState = _currentMouseState;
        _previousKeyboardState = _currentKeyboardState;
    }

    private void UpdateInputStates()
    {
        _currentMouseState = Mouse.GetState();
        _currentKeyboardState = Keyboard.GetState();
    }

    private void CollectInputEntities()
    {
        _inputEntities.Clear();

        foreach (var entity in GetEntitiesWith<InputComponent, TransformComponent>())
        {
            var input = entity.GetComponent<InputComponent>()!;
            if (input.AcceptsMouseInput || input.AcceptsKeyboardInput)
            {
                _inputEntities.Add(entity);
            }
        }

        // Sort by input priority (highest first) and then by render depth
        _inputEntities.Sort((a, b) =>
        {
            var priorityA = a.GetComponent<InputComponent>()!.Priority;
            var priorityB = b.GetComponent<InputComponent>()!.Priority;

            int priorityComparison = priorityB.CompareTo(priorityA); // Higher priority first
            if (priorityComparison != 0) return priorityComparison;

            // If same priority, sort by depth (higher depth first - on top)
            var depthA = GetRenderDepth(a);
            var depthB = GetRenderDepth(b);
            return depthB.CompareTo(depthA);
        });
    }

    private void ProcessMouseInput()
    {
        var mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

        // Handle mouse movement and hover
        ProcessMouseMovement(mousePosition);

        // Handle mouse buttons
        ProcessMouseButtons(mousePosition);

        // Handle mouse wheel
        ProcessMouseWheel(mousePosition);
    }

    private void ProcessMouseMovement(Vector2 mousePosition)
    {
        UIEntity? newHoveredEntity = null;

        // Find the top-most entity under the mouse (if not captured)
        if (_capturedEntity == null)
        {
            foreach (var entity in _inputEntities)
            {
                if (!entity.GetComponent<InputComponent>()!.AcceptsMouseInput)
                    continue;

                if (IsPointInEntity(mousePosition, entity))
                {
                    newHoveredEntity = entity;
                    break; // Take the first (highest priority/depth) entity
                }
            }
        }
        else
        {
            // If mouse is captured, keep hover on the captured entity
            newHoveredEntity = _capturedEntity;
        }

        // Handle hover changes
        if (newHoveredEntity != _hoveredEntity)
        {
            // Mouse exit on previous entity
            if (_hoveredEntity != null)
            {
                var exitEvent = new MouseInputEvent
                {
                    Type = MouseEventType.Exit,
                    Position = mousePosition,
                    WorldPosition = mousePosition
                };
                SendMouseEvent(_hoveredEntity, exitEvent);
            }

            // Mouse enter on new entity
            if (newHoveredEntity != null)
            {
                var enterEvent = new MouseInputEvent
                {
                    Type = MouseEventType.Enter,
                    Position = mousePosition,
                    WorldPosition = mousePosition
                };
                SendMouseEvent(newHoveredEntity, enterEvent);
            }

            _hoveredEntity = newHoveredEntity;
        }

        // Send mouse move event to hovered entity
        if (_hoveredEntity != null && (_currentMouseState.X != _previousMouseState.X || _currentMouseState.Y != _previousMouseState.Y))
        {
            var moveEvent = new MouseInputEvent
            {
                Type = MouseEventType.Move,
                Position = mousePosition,
                WorldPosition = mousePosition
            };
            SendMouseEvent(_hoveredEntity, moveEvent);
        }
    }

    private void ProcessMouseButtons(Vector2 mousePosition)
    {
        // Check each mouse button
        ProcessMouseButton(mousePosition, 0, _currentMouseState.LeftButton, _previousMouseState.LeftButton);
        ProcessMouseButton(mousePosition, 1, _currentMouseState.RightButton, _previousMouseState.RightButton);
        ProcessMouseButton(mousePosition, 2, _currentMouseState.MiddleButton, _previousMouseState.MiddleButton);
    }

    private void ProcessMouseButton(Vector2 mousePosition, int button, XnaButtonState current, XnaButtonState previous)
    {
        // Mouse button down
        if (current == XnaButtonState.Pressed && previous == XnaButtonState.Released)
        {
            var targetEntity = _hoveredEntity;
            if (targetEntity != null)
            {
                var downEvent = new MouseInputEvent
                {
                    Type = MouseEventType.Down,
                    Position = mousePosition,
                    WorldPosition = mousePosition,
                    Button = button
                };

                if (SendMouseEvent(targetEntity, downEvent) && !downEvent.Handled)
                {
                    // Capture mouse for dragging
                    _capturedEntity = targetEntity;

                    // Set focus if this entity can receive it
                    var input = targetEntity.GetComponent<InputComponent>()!;
                    if (input.CanReceiveFocus)
                    {
                        SetFocus(targetEntity);
                    }
                }
            }
        }

        // Mouse button up
        if (current == XnaButtonState.Released && previous == XnaButtonState.Pressed)
        {
            var targetEntity = _capturedEntity ?? _hoveredEntity;
            if (targetEntity != null)
            {
                var upEvent = new MouseInputEvent
                {
                    Type = MouseEventType.Up,
                    Position = mousePosition,
                    WorldPosition = mousePosition,
                    Button = button
                };

                SendMouseEvent(targetEntity, upEvent);

                // If this was a click (mouse up on same entity that had mouse down)
                if (_capturedEntity == targetEntity)
                {
                    var clickEvent = new MouseInputEvent
                    {
                        Type = MouseEventType.Click,
                        Position = mousePosition,
                        WorldPosition = mousePosition,
                        Button = button
                    };
                    SendMouseEvent(targetEntity, clickEvent);
                }
            }

            // Release capture
            _capturedEntity = null;
        }
    }

    private void ProcessMouseWheel(Vector2 mousePosition)
    {
        int wheelDelta = _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
        if (wheelDelta != 0 && _hoveredEntity != null)
        {
            var wheelEvent = new MouseInputEvent
            {
                Type = MouseEventType.Wheel,
                Position = mousePosition,
                WorldPosition = mousePosition,
                WheelDelta = wheelDelta
            };
            SendMouseEvent(_hoveredEntity, wheelEvent);
        }
    }

    private void ProcessKeyboardInput()
    {
        if (_focusedEntity == null) return;

        var input = _focusedEntity.GetComponent<InputComponent>()!;
        if (!input.AcceptsKeyboardInput) return;

        // Check for key presses
        var currentKeys = _currentKeyboardState.GetPressedKeys();
        var previousKeys = _previousKeyboardState.GetPressedKeys();

        // Find newly pressed keys
        var newKeys = currentKeys.Except(previousKeys);
        foreach (var key in newKeys)
        {
            var keyEvent = new KeyboardInputEvent
            {
                Key = key,
                IsKeyDown = true,
                IsRepeat = false,
                Character = ConvertKeyToChar(key)
            };
            SendKeyboardEvent(_focusedEntity, keyEvent);
        }

        // Find released keys
        var releasedKeys = previousKeys.Except(currentKeys);
        foreach (var key in releasedKeys)
        {
            var keyEvent = new KeyboardInputEvent
            {
                Key = key,
                IsKeyDown = false,
                IsRepeat = false,
                Character = ConvertKeyToChar(key)
            };
            SendKeyboardEvent(_focusedEntity, keyEvent);
        }
    }

    private bool IsPointInEntity(Vector2 point, UIEntity entity)
    {
        var transform = entity.GetComponent<TransformComponent>()!;
        var bounds = transform.WorldBounds;

        return point.X >= bounds.X && point.X < bounds.Right &&
               point.Y >= bounds.Y && point.Y < bounds.Bottom;
    }

    private bool SendMouseEvent(UIEntity entity, MouseInputEvent mouseEvent)
    {
        var input = entity.GetComponent<InputComponent>()!;

        // Use the HandleMouseInput method from InputComponent
        return input.HandleMouseInput(mouseEvent);
    }

    private void SendKeyboardEvent(UIEntity entity, KeyboardInputEvent keyEvent)
    {
        var input = entity.GetComponent<InputComponent>()!;

        // Use the HandleKeyboardInput method from InputComponent
        input.HandleKeyboardInput(keyEvent);
    }

    /// <summary>
    /// Set keyboard focus to the specified entity.
    /// </summary>
    /// <param name="entity">The entity to focus, or null to clear focus.</param>
    public void SetFocus(UIEntity? entity)
    {
        if (_focusedEntity == entity) return;

        // Remove focus from current entity
        if (_focusedEntity != null)
        {
            var oldInput = _focusedEntity.GetComponent<InputComponent>()!;
            oldInput.HasFocus = false;
            // Focus events are handled automatically by the HasFocus property setter
        }

        _focusedEntity = entity;

        // Set focus on new entity
        if (_focusedEntity != null)
        {
            var newInput = _focusedEntity.GetComponent<InputComponent>()!;
            if (newInput.CanReceiveFocus)
            {
                newInput.HasFocus = true;
                // Focus events are handled automatically by the HasFocus property setter
            }
            else
            {
                _focusedEntity = null;
            }
        }
    }

    private float GetRenderDepth(UIEntity entity)
    {
        // For now, calculate depth based on parent hierarchy
        // TODO: Add ZIndex property to StyleComponent if needed
        float depth = 0f;
        var current = entity;
        while (current.Parent != null)
        {
            depth += 0.01f;
            current = current.Parent;
        }

        return depth;
    }

    private char ConvertKeyToChar(Keys key)
    {
        // Basic key to character conversion
        // This is a simplified version - a full implementation would handle
        // shift states, international keyboards, etc.

        if (key >= Keys.A && key <= Keys.Z)
        {
            return (char)('a' + (key - Keys.A));
        }

        if (key >= Keys.D0 && key <= Keys.D9)
        {
            return (char)('0' + (key - Keys.D0));
        }

        switch (key)
        {
            case Keys.Space: return ' ';
            case Keys.Enter: return '\n';
            case Keys.Tab: return '\t';
            default: return '\0';
        }
    }
}
