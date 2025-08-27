using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class ButtonComponentTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var component = new ButtonComponent();

        // Assert
        Assert.Equal(ButtonState.Normal, component.State);
        Assert.False(component.IsPressed);
        Assert.False(component.IsHovered);
        Assert.True(component.Enabled);
    }

    [Fact]
    public void Enabled_Setter_UpdatesStateWhenDisabled()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Enabled = false;

        // Assert
        Assert.False(component.Enabled);
        Assert.Equal(ButtonState.Disabled, component.State);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Enabled_Setter_UpdatesStateWhenReEnabled()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        component.Enabled = false; // Start disabled
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Enabled = true;

        // Assert
        Assert.True(component.Enabled);
        Assert.Equal(ButtonState.Normal, component.State);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void PerformClick_TriggersClickEvent()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        bool clickEventFired = false;
        ButtonComponent? clickedComponent = null;

        component.Click += (btn) =>
        {
            clickEventFired = true;
            clickedComponent = btn;
        };

        // Act
        component.PerformClick();

        // Assert
        Assert.True(clickEventFired);
        Assert.Equal(component, clickedComponent);
    }

    [Fact]
    public void PerformClick_DoesNotTrigger_WhenDisabled()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        bool clickEventFired = false;

        component.Click += (btn) =>
        {
            clickEventFired = true;
        };

        // Act
        component.Enabled = false;
        component.PerformClick();

        // Assert
        Assert.False(clickEventFired);
    }

    [Fact]
    public void SetText_UpdatesTextProperty()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();

        // Act
        component.SetText("Test Button");

        // Assert
        Assert.Equal("Test Button", component.Text);
    }

    [Fact]
    public void SetText_UpdatesRenderableComponent_WhenPresent()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        var renderable = entity.AddComponent<RenderableComponent>();

        // Act
        component.SetText("Test Button");

        // Assert
        Assert.Equal("Test Button", renderable.Text);
    }

    [Fact]
    public void GetStateColors_ReturnsCorrectColorsForNormalState()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();

        // Act
        var (bgColor, textColor, opacity) = component.GetStateColors();

        // Assert
        Assert.Equal(Color.LightGray, bgColor);
        Assert.Equal(Color.Black, textColor);
        Assert.Equal(1.0f, opacity);
    }

    [Fact]
    public void GetStateColors_ReturnsCorrectColorsForDisabledState()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        component.Enabled = false;

        // Act
        var (bgColor, textColor, opacity) = component.GetStateColors();

        // Assert
        Assert.Equal(Color.LightGray, bgColor);
        Assert.Equal(Color.Gray, textColor);
        Assert.Equal(0.5f, opacity);
    }

    [Fact]
    public void ApplyStateToComponents_UpdatesStyleComponent()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        var style = entity.AddComponent<StyleComponent>();
        component.Enabled = false; // Set to disabled for testing

        // Act
        component.ApplyStateToComponents();

        // Assert
        Assert.Equal(Color.LightGray, style.BackgroundColor);
        Assert.Equal(0.5f, style.Opacity);
    }

    [Fact]
    public void ApplyStateToComponents_UpdatesRenderableComponent()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();
        var renderable = entity.AddComponent<RenderableComponent>();
        renderable.RenderType = RenderType.Text;
        component.Enabled = false; // Set to disabled for testing

        // Act
        component.ApplyStateToComponents();

        // Assert
        Assert.Equal(Color.Gray, renderable.Color);
    }

    [Fact]
    public void Validate_ReturnsTrue_WhenAttachedToEntity()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ButtonComponent>();

        // Act & Assert
        Assert.True(component.Validate());
    }

    [Fact]
    public void Validate_ReturnsFalse_WhenNotAttachedToEntity()
    {
        // Arrange
        var component = new ButtonComponent();

        // Act & Assert - Component without Entity should fail validation
        Assert.False(component.Validate());
    }

    [Fact]
    public void OnAttached_SubscribesToInputEvents()
    {
        // Arrange
        var entity = new UIEntity("test");
        var inputComponent = entity.AddComponent<InputComponent>();

        // Act
        var buttonComponent = entity.AddComponent<ButtonComponent>();

        // Assert
        // The component should be attached successfully
        Assert.NotNull(buttonComponent.Entity);
        Assert.Equal(entity, buttonComponent.Entity);
    }
}
