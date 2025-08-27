using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class StyleComponentTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var component = new StyleComponent();

        // Assert
        Assert.Equal(Color.Transparent, component.BackgroundColor);
        Assert.Equal(Color.Black, component.BorderColor); // Default is Black, not Transparent
        Assert.Equal(Thickness.Zero, component.BorderThickness);
        Assert.Equal(Thickness.Zero, component.Margin);
        Assert.Equal(Thickness.Zero, component.Padding);
        Assert.True(component.Visible);
        Assert.Equal(1.0f, component.Opacity);
    }

    [Fact]
    public void BackgroundColor_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        component.BackgroundColor = Color.Red;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.BackgroundColor = Color.Blue;

        // Assert
        Assert.Equal(Color.Blue, component.BackgroundColor);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void BackgroundColor_Setter_DoesNotMarkDirtyWhenSame()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        component.BackgroundColor = Color.Red;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.BackgroundColor = Color.Red;

        // Assert
        Assert.Equal(Color.Red, component.BackgroundColor);
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void BorderColor_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        component.BorderColor = Color.Red;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.BorderColor = Color.Blue;

        // Assert
        Assert.Equal(Color.Blue, component.BorderColor);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void BorderThickness_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        var initialThickness = new Thickness(1);
        var newThickness = new Thickness(2);
        component.BorderThickness = initialThickness;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.BorderThickness = newThickness;

        // Assert
        Assert.Equal(newThickness, component.BorderThickness);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void Margin_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        var initialMargin = new Thickness(1);
        var newMargin = new Thickness(2);
        component.Margin = initialMargin;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Margin = newMargin;

        // Assert
        Assert.Equal(newMargin, component.Margin);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void Padding_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        var initialPadding = new Thickness(1);
        var newPadding = new Thickness(2);
        component.Padding = initialPadding;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Padding = newPadding;

        // Assert
        Assert.Equal(newPadding, component.Padding);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void Visible_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        component.Visible = true;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Visible = false;

        // Assert
        Assert.False(component.Visible);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Opacity_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();
        component.Opacity = 1.0f;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Opacity = 0.5f;

        // Assert
        Assert.Equal(0.5f, component.Opacity);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void Opacity_Setter_ClampsToValidRange()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();

        // Act & Assert - Test lower bound
        component.Opacity = -0.5f;
        Assert.Equal(0.0f, component.Opacity);

        // Act & Assert - Test upper bound
        component.Opacity = 1.5f;
        Assert.Equal(1.0f, component.Opacity);

        // Act & Assert - Test valid value
        component.Opacity = 0.7f;
        Assert.Equal(0.7f, component.Opacity);
    }

    [Fact]
    public void Validate_ReturnsTrue_WhenAttachedToEntity()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<StyleComponent>();

        // Act & Assert
        Assert.True(component.Validate());
    }

    [Fact]
    public void Validate_ReturnsFalse_WhenNotAttachedToEntity()
    {
        // Arrange
        var component = new StyleComponent();

        // Act & Assert - Component without Entity should fail validation
        Assert.False(component.Validate());
    }
}

public class ThicknessTests
{
    [Fact]
    public void Constructor_UniformThickness_SetsAllSides()
    {
        // Arrange & Act
        var thickness = new Thickness(5f);

        // Assert
        Assert.Equal(5f, thickness.Left);
        Assert.Equal(5f, thickness.Top);
        Assert.Equal(5f, thickness.Right);
        Assert.Equal(5f, thickness.Bottom);
    }

    [Fact]
    public void Constructor_HorizontalVertical_SetsCorrectSides()
    {
        // Arrange & Act
        var thickness = new Thickness(10f, 20f);

        // Assert
        Assert.Equal(10f, thickness.Left);
        Assert.Equal(20f, thickness.Top);
        Assert.Equal(10f, thickness.Right);
        Assert.Equal(20f, thickness.Bottom);
    }

    [Fact]
    public void Constructor_AllSides_SetsIndividualValues()
    {
        // Arrange & Act
        var thickness = new Thickness(1f, 2f, 3f, 4f);

        // Assert
        Assert.Equal(1f, thickness.Left);
        Assert.Equal(2f, thickness.Top);
        Assert.Equal(3f, thickness.Right);
        Assert.Equal(4f, thickness.Bottom);
    }

    [Fact]
    public void TotalWidth_CalculatesCorrectly()
    {
        // Arrange
        var thickness = new Thickness(2f, 3f, 4f, 5f);

        // Act & Assert
        Assert.Equal(6f, thickness.TotalWidth); // Left + Right = 2 + 4
    }

    [Fact]
    public void TotalHeight_CalculatesCorrectly()
    {
        // Arrange
        var thickness = new Thickness(2f, 3f, 4f, 5f);

        // Act & Assert
        Assert.Equal(8f, thickness.TotalHeight); // Top + Bottom = 3 + 5
    }

    [Fact]
    public void Zero_ReturnsAllZeros()
    {
        // Arrange & Act
        var zero = Thickness.Zero;

        // Assert
        Assert.Equal(0f, zero.Left);
        Assert.Equal(0f, zero.Top);
        Assert.Equal(0f, zero.Right);
        Assert.Equal(0f, zero.Bottom);
    }

    [Fact]
    public void ImplicitConversion_FromFloat_CreatesUniformThickness()
    {
        // Arrange & Act
        Thickness thickness = 7f;

        // Assert
        Assert.Equal(7f, thickness.Left);
        Assert.Equal(7f, thickness.Top);
        Assert.Equal(7f, thickness.Right);
        Assert.Equal(7f, thickness.Bottom);
    }
}
