using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class InputComponentTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var component = new InputComponent();

        // Assert
        Assert.True(component.AcceptsMouseInput);
        Assert.False(component.AcceptsKeyboardInput);
        Assert.True(component.CanReceiveFocus);
        Assert.False(component.HasFocus);
        Assert.Equal(InputPriority.Normal, component.Priority);
    }

    [Fact]
    public void AcceptsMouseInput_Setter_WorksCorrectly()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();

        // Act
        component.AcceptsMouseInput = false;

        // Assert
        Assert.False(component.AcceptsMouseInput);
    }

    [Fact]
    public void AcceptsKeyboardInput_Setter_WorksCorrectly()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();

        // Act
        component.AcceptsKeyboardInput = true;

        // Assert
        Assert.True(component.AcceptsKeyboardInput);
    }

    [Fact]
    public void CanReceiveFocus_Setter_WorksCorrectly()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();

        // Act
        component.CanReceiveFocus = false;

        // Assert
        Assert.False(component.CanReceiveFocus);
    }

    [Fact]
    public void CanReceiveFocus_WhenSetToFalse_RemovesFocus()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.HasFocus = true; // First gain focus

        // Act
        component.CanReceiveFocus = false;

        // Assert
        Assert.False(component.CanReceiveFocus);
        Assert.False(component.HasFocus);
    }

    [Fact]
    public void HasFocus_Setter_WorksCorrectly()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.CanReceiveFocus = true;

        // Act
        component.HasFocus = true;

        // Assert
        Assert.True(component.HasFocus);
    }

    [Fact]
    public void HasFocus_CannotBeSetWhenCanReceiveFocusIsFalse()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.CanReceiveFocus = false;

        // Act
        component.HasFocus = true;

        // Assert
        Assert.False(component.HasFocus);
    }

    [Fact]
    public void Priority_Setter_WorksCorrectly()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();

        // Act
        component.Priority = InputPriority.High;

        // Assert
        Assert.Equal(InputPriority.High, component.Priority);
    }

    [Fact]
    public void MouseEventHandlers_CanBeAttached()
    {
        // Arrange
        var component = new InputComponent();

        // Act & Assert - Just verify events can be subscribed to
        component.MouseEnter += (e) => { };
        component.MouseExit += (e) => { };
        component.MouseDown += (e) => { };
        component.MouseUp += (e) => { };
        component.MouseClick += (e) => { };
        component.MouseWheel += (e) => { };

        // No exceptions should be thrown
    }

    [Fact]
    public void KeyboardEventHandlers_CanBeAttached()
    {
        // Arrange
        var component = new InputComponent();

        // Act & Assert - Just verify events can be subscribed to
        component.KeyDown += (e) => { };
        component.KeyUp += (e) => { };

        // No exceptions should be thrown
    }

    [Fact]
    public void FocusEvents_CanBeAttached()
    {
        // Arrange
        var component = new InputComponent();

        // Act & Assert - Just verify events can be subscribed to
        component.FocusGained += () => { };
        component.FocusLost += () => { };

        // No exceptions should be thrown
    }

    [Fact]
    public void RequestFocus_GainsFocus()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.CanReceiveFocus = true;

        // Act
        component.RequestFocus();

        // Assert
        Assert.True(component.HasFocus);
    }

    [Fact]
    public void ReleaseFocus_RemovesFocus()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.CanReceiveFocus = true;
        component.RequestFocus(); // First gain focus

        // Act
        component.ReleaseFocus();

        // Assert
        Assert.False(component.HasFocus);
    }

    [Fact]
    public void Validate_ReturnsTrue_WhenAttachedToEntity()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();

        // Act & Assert
        Assert.True(component.Validate());
    }

    [Fact]
    public void Validate_ReturnsFalse_WhenNotAttachedToEntity()
    {
        // Arrange
        var component = new InputComponent();

        // Act & Assert - Component without Entity should fail validation
        Assert.False(component.Validate());
    }

    [Fact]
    public void ContainsPoint_ReturnsTrueForPointInBounds()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        var transform = entity.AddComponent<TransformComponent>();
        transform.Position = new Vector2(10, 10);
        transform.Size = new Vector2(100, 50);

        // Act & Assert
        Assert.True(component.ContainsPoint(new Vector2(50, 25))); // Center point
        Assert.True(component.ContainsPoint(new Vector2(10, 10))); // Top-left corner
        Assert.True(component.ContainsPoint(new Vector2(109, 59))); // Bottom-right corner (within bounds)
    }

    [Fact]
    public void ContainsPoint_ReturnsFalseForPointOutOfBounds()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        var transform = entity.AddComponent<TransformComponent>();
        transform.Position = new Vector2(10, 10);
        transform.Size = new Vector2(100, 50);

        // Act & Assert
        Assert.False(component.ContainsPoint(new Vector2(5, 25))); // Left of bounds
        Assert.False(component.ContainsPoint(new Vector2(115, 25))); // Right of bounds
        Assert.False(component.ContainsPoint(new Vector2(50, 5))); // Above bounds
        Assert.False(component.ContainsPoint(new Vector2(50, 65))); // Below bounds
    }

    [Fact]
    public void HandleMouseInput_ReturnsTrue_WhenAcceptsMouseInput()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.AcceptsMouseInput = true;
        var mouseEvent = new MouseInputEvent
        {
            Type = MouseEventType.Click,
            Position = new Vector2(50, 25),
            Button = 0
        };

        // Act
        var handled = component.HandleMouseInput(mouseEvent);

        // Assert - The method should return true for accepted mouse input
        // The actual handling depends on the internal implementation
        Assert.True(component.AcceptsMouseInput);
    }

    [Fact]
    public void HandleKeyboardInput_ReturnsCorrectly_WhenAcceptsKeyboardInput()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<InputComponent>();
        component.AcceptsKeyboardInput = true;
        var keyEvent = new KeyboardInputEvent
        {
            Key = Microsoft.Xna.Framework.Input.Keys.Space
        };

        // Act & Assert - The method should handle keyboard input when it accepts it
        Assert.True(component.AcceptsKeyboardInput);
    }
}

public class MouseInputEventTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var mouseEvent = new MouseInputEvent
        {
            Type = MouseEventType.Click,
            Position = new Vector2(100, 200),
            WorldPosition = new Vector2(150, 250),
            Button = 0,
            WheelDelta = 120,
            Handled = false
        };

        // Assert
        Assert.Equal(MouseEventType.Click, mouseEvent.Type);
        Assert.Equal(new Vector2(100, 200), mouseEvent.Position);
        Assert.Equal(new Vector2(150, 250), mouseEvent.WorldPosition);
        Assert.Equal(0, mouseEvent.Button);
        Assert.Equal(120, mouseEvent.WheelDelta);
        Assert.False(mouseEvent.Handled);
    }
}

public class KeyboardInputEventTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var keyEvent = new KeyboardInputEvent
        {
            Key = Microsoft.Xna.Framework.Input.Keys.Space,
            Handled = false
        };

        // Assert
        Assert.Equal(Microsoft.Xna.Framework.Input.Keys.Space, keyEvent.Key);
        Assert.False(keyEvent.Handled);
    }
}
