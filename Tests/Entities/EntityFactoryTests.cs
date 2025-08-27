using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Components;
using MonoGameUI.Core;
using MonoGameUI.Entities;
using Moq;
using Xunit;

namespace Tests.Entities;

/// <summary>
/// Tests for the EntityFactory class.
/// </summary>
public class EntityFactoryTests
{
    [Fact]
    public void CreateBasicEntity_Should_CreateEntityWithBasicComponents()
    {
        // Act
        var entity = EntityFactory.CreateBasicEntity("TestEntity");

        // Assert
        Assert.Equal("TestEntity", entity.Name);
        Assert.NotNull(entity.GetComponent<TransformComponent>());
        Assert.NotNull(entity.GetComponent<LayoutComponent>());
    }

    [Fact]
    public void CreateBasicEntity_Should_GenerateNameWhenEmpty()
    {
        // Act
        var entity = EntityFactory.CreateBasicEntity();

        // Assert
        Assert.NotNull(entity.Name);
        Assert.NotEmpty(entity.Name);
        Assert.StartsWith("Entity_", entity.Name);
    }

    [Fact]
    public void CreateButton_Should_CreateButtonWithAllComponents()
    {
        // Act
        var button = EntityFactory.CreateButton("Test Button");

        // Assert
        Assert.Contains("Button_Test Button", button.Name);

        // Verify all required components are present
        Assert.NotNull(button.GetComponent<TransformComponent>());
        Assert.NotNull(button.GetComponent<LayoutComponent>());
        Assert.NotNull(button.GetComponent<RenderableComponent>());
        Assert.NotNull(button.GetComponent<InputComponent>());
        Assert.NotNull(button.GetComponent<StyleComponent>());
        Assert.NotNull(button.GetComponent<ButtonComponent>());
    }

    [Fact]
    public void CreateButton_Should_ConfigureInputSettings()
    {
        // Act
        var button = EntityFactory.CreateButton();

        // Assert
        var input = button.GetComponent<InputComponent>()!;
        Assert.True(input.AcceptsMouseInput);
        Assert.True(input.CanReceiveFocus);
    }

    [Fact]
    public void CreateButton_Should_ConfigureDefaultStyling()
    {
        // Act
        var button = EntityFactory.CreateButton();

        // Assert
        var style = button.GetComponent<StyleComponent>()!;
        Assert.Equal(Color.LightGray, style.BackgroundColor);
        Assert.Equal(Color.DarkGray, style.BorderColor);
        Assert.Equal(new Thickness(1), style.BorderThickness);
        Assert.Equal(new Thickness(8, 4), style.Padding);
    }

    [Fact]
    public void CreateButton_Should_ConfigureFitContentLayout()
    {
        // Act
        var button = EntityFactory.CreateButton();

        // Assert
        var layout = button.GetComponent<LayoutComponent>()!;
        Assert.Equal(SizeMode.FitContent, layout.WidthMode);
        Assert.Equal(SizeMode.FitContent, layout.HeightMode);
    }

    [Fact]
    public void CreateTextLabel_Should_CreateLabelWithTextComponents()
    {
        // Act
        var label = EntityFactory.CreateTextLabel("Test Label", null, Color.Red);

        // Assert
        Assert.Contains("Label_Test Label", label.Name);

        var renderable = label.GetComponent<RenderableComponent>()!;
        Assert.Equal("Test Label", renderable.Text);
        Assert.Equal(Color.Red, renderable.Color);

        var layout = label.GetComponent<LayoutComponent>()!;
        Assert.Equal(SizeMode.FitContent, layout.WidthMode);
        Assert.Equal(SizeMode.FitContent, layout.HeightMode);
    }

    [Fact]
    public void CreateImage_Should_CreateImageWithSpriteRendering()
    {
        // Act - Test without actual texture to avoid mocking issues
        var image = EntityFactory.CreateImage(null, Color.Blue);

        // Assert
        Assert.Equal("Image", image.Name);

        var renderable = image.GetComponent<RenderableComponent>()!;
        Assert.Equal(RenderType.Sprite, renderable.RenderType);
        Assert.Equal(Color.Blue, renderable.Color);

        var layout = image.GetComponent<LayoutComponent>()!;
        Assert.Equal(SizeMode.Fixed, layout.WidthMode);
        Assert.Equal(SizeMode.Fixed, layout.HeightMode);

        // When no texture provided, size should remain default
        var transform = image.GetComponent<TransformComponent>()!;
        Assert.Equal(Vector2.Zero, transform.Size);
    }

    [Fact]
    public void CreateContainer_Should_CreateContainerWithFillLayout()
    {
        // Act
        var container = EntityFactory.CreateContainer("TestContainer");

        // Assert
        Assert.Equal("TestContainer", container.Name);
        Assert.NotNull(container.GetComponent<StyleComponent>());

        var layout = container.GetComponent<LayoutComponent>()!;
        Assert.Equal(SizeMode.Fill, layout.WidthMode);
        Assert.Equal(SizeMode.Fill, layout.HeightMode);
    }

    [Fact]
    public void CreatePanel_Should_CreateStyledContainer()
    {
        // Act
        var panel = EntityFactory.CreatePanel(Color.Yellow, Color.Black, 2f);

        // Assert
        Assert.Equal("Panel", panel.Name);

        var style = panel.GetComponent<StyleComponent>()!;
        Assert.Equal(Color.Yellow, style.BackgroundColor);
        Assert.Equal(Color.Black, style.BorderColor);
        Assert.Equal(new Thickness(2f), style.BorderThickness);
        Assert.Equal(new Thickness(4), style.Padding);
    }

    [Fact]
    public void CreateHorizontalLayout_Should_ConfigureRowDirection()
    {
        // Act
        var layout = EntityFactory.CreateHorizontalLayout(10f);

        // Assert
        Assert.Equal("HorizontalLayout", layout.Name);

        var layoutComponent = layout.GetComponent<LayoutComponent>()!;
        Assert.Equal(FlexDirection.Row, layoutComponent.FlexDirection);
        Assert.Equal(10f, layoutComponent.Gap);
    }

    [Fact]
    public void CreateVerticalLayout_Should_ConfigureColumnDirection()
    {
        // Act
        var layout = EntityFactory.CreateVerticalLayout(5f);

        // Assert
        Assert.Equal("VerticalLayout", layout.Name);

        var layoutComponent = layout.GetComponent<LayoutComponent>()!;
        Assert.Equal(FlexDirection.Column, layoutComponent.FlexDirection);
        Assert.Equal(5f, layoutComponent.Gap);
    }

    [Fact]
    public void CreateImageButton_Should_CreateButtonWithSprite()
    {
        // Act
        var imageButton = EntityFactory.CreateImageButton(null, "Image Button");

        // Assert
        Assert.Contains("Button_Image Button", imageButton.Name);

        // Should have all button components
        Assert.NotNull(imageButton.GetComponent<ButtonComponent>());
        Assert.NotNull(imageButton.GetComponent<InputComponent>());

        var renderable = imageButton.GetComponent<RenderableComponent>()!;
        // When no texture provided, should still have text configured
        Assert.Equal("Image Button", renderable.Text);
    }

    [Fact]
    public void Configure_Should_SetTransformProperties()
    {
        // Arrange
        var entity = EntityFactory.CreateBasicEntity();
        var position = new Vector2(100, 200);
        var size = new Vector2(300, 400);

        // Act
        var result = entity.Configure(position, size, false);

        // Assert
        Assert.Same(entity, result); // Should return same instance for chaining

        var transform = entity.GetComponent<TransformComponent>()!;
        Assert.Equal(position, transform.Position);
        Assert.Equal(size, transform.Size);

        var style = entity.GetComponent<StyleComponent>();
        if (style != null)
        {
            Assert.False(style.Visible);
        }
    }

    [Fact]
    public void Configure_Should_HandleMissingComponents()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        // Don't add any components

        // Act & Assert - Should not throw
        var result = entity.Configure(Vector2.One, Vector2.One, false);
        Assert.Same(entity, result);
    }
}
