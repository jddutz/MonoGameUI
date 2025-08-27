using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class RenderableComponentTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var component = new RenderableComponent();

        // Assert
        Assert.Equal(RenderType.None, component.RenderType);
        Assert.Equal(Color.White, component.Color);
        Assert.Equal(string.Empty, component.Text);
        Assert.Null(component.Texture);
        Assert.Null(component.Font);
    }

    [Fact]
    public void SetText_UpdatesPropertiesAndMarksDirty()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Text = "Hello World";
        component.Color = Color.Red;
        component.RenderType = RenderType.Text;

        // Assert
        Assert.Equal(RenderType.Text, component.RenderType);
        Assert.Equal("Hello World", component.Text);
        Assert.Equal(Color.Red, component.Color);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void SetSprite_UpdatesPropertiesAndMarksDirty()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act - For testing, we'll test the property setters directly since we can't create a real Texture2D
        component.RenderType = RenderType.Sprite;
        component.Color = Color.Blue;

        // Assert
        Assert.Equal(RenderType.Sprite, component.RenderType);
        Assert.Equal(Color.Blue, component.Color);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void SetSolidColor_UpdatesPropertiesAndMarksDirty()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.SetSolidColor(Color.Green);

        // Assert
        Assert.Equal(RenderType.SolidColor, component.RenderType);
        Assert.Equal(Color.Green, component.Color);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Text_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        component.Text = "Initial";
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Text = "Changed";

        // Assert
        Assert.Equal("Changed", component.Text);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Text_Setter_DoesNotMarkDirtyWhenSame()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        component.Text = "Same";
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Text = "Same";

        // Assert
        Assert.Equal("Same", component.Text);
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Color_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        component.Color = Color.Red;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Color = Color.Blue;

        // Assert
        Assert.Equal(Color.Blue, component.Color);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void RenderType_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();
        component.RenderType = RenderType.None;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.RenderType = RenderType.Text;

        // Assert
        Assert.Equal(RenderType.Text, component.RenderType);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Validate_ReturnsTrueForValidConfigurations()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();

        // Act & Assert - None type should always be valid when attached to entity
        component.RenderType = RenderType.None;
        Assert.True(component.Validate());

        // SolidColor should be valid without additional requirements
        component.RenderType = RenderType.SolidColor;
        Assert.True(component.Validate());
    }

    [Fact]
    public void Validate_ReturnsFalse_WhenNotAttachedToEntity()
    {
        // Arrange
        var component = new RenderableComponent();

        // Act & Assert - Component without Entity should fail validation
        Assert.False(component.Validate());
    }

    [Fact]
    public void Validate_ReturnsTrue_WhenAttachedToEntity()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<RenderableComponent>();

        // Act & Assert - Component with Entity should pass validation
        Assert.True(component.Validate());
    }
}
