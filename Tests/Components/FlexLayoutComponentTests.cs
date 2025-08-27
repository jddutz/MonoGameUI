using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class FlexLayoutComponentTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var component = new FlexLayoutComponent();

        // Assert
        Assert.Equal(FlexDirection.Row, component.FlexDirection);
        Assert.Equal(JustifyContent.FlexStart, component.JustifyContent);
        Assert.Equal(AlignItems.Stretch, component.AlignItems);
        Assert.Equal(FlexWrap.NoWrap, component.FlexWrap);
        Assert.Equal(0f, component.Gap);
        Assert.Equal(0f, component.RowGap);
        Assert.Equal(0f, component.ColumnGap);
    }

    [Fact]
    public void FlexDirection_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.FlexDirection = FlexDirection.Row;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.FlexDirection = FlexDirection.Column;

        // Assert
        Assert.Equal(FlexDirection.Column, component.FlexDirection);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void JustifyContent_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.JustifyContent = JustifyContent.FlexStart;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.JustifyContent = JustifyContent.Center;

        // Assert
        Assert.Equal(JustifyContent.Center, component.JustifyContent);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void AlignItems_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.AlignItems = AlignItems.Stretch;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.AlignItems = AlignItems.Center;

        // Assert
        Assert.Equal(AlignItems.Center, component.AlignItems);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void Gap_Setter_MarksDirtyWhenChanged()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.Gap = 0f;
        entity.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        component.Gap = 10f;

        // Assert
        Assert.Equal(10f, component.Gap);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void IsMainAxisHorizontal_ReturnsTrueForRowDirections()
    {
        // Arrange
        var component = new FlexLayoutComponent();

        // Act & Assert
        component.FlexDirection = FlexDirection.Row;
        Assert.True(component.IsMainAxisHorizontal);

        component.FlexDirection = FlexDirection.RowReverse;
        Assert.True(component.IsMainAxisHorizontal);

        component.FlexDirection = FlexDirection.Column;
        Assert.False(component.IsMainAxisHorizontal);

        component.FlexDirection = FlexDirection.ColumnReverse;
        Assert.False(component.IsMainAxisHorizontal);
    }

    [Fact]
    public void IsReversed_ReturnsTrueForReverseDirections()
    {
        // Arrange
        var component = new FlexLayoutComponent();

        // Act & Assert
        component.FlexDirection = FlexDirection.Row;
        Assert.False(component.IsReversed);

        component.FlexDirection = FlexDirection.RowReverse;
        Assert.True(component.IsReversed);

        component.FlexDirection = FlexDirection.Column;
        Assert.False(component.IsReversed);

        component.FlexDirection = FlexDirection.ColumnReverse;
        Assert.True(component.IsReversed);
    }

    [Fact]
    public void CalculateDesiredSize_ReturnsZeroForNoChildren()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<FlexLayoutComponent>();

        // Act
        var desiredSize = component.CalculateDesiredSize(new Vector2(200, 200));

        // Assert
        Assert.Equal(Vector2.Zero, desiredSize);
    }

    [Fact]
    public void CalculateDesiredSize_ReturnsCorrectSizeForRowLayout()
    {
        // Arrange
        var entity = new UIEntity("container");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.FlexDirection = FlexDirection.Row;
        component.Gap = 10f;

        // Add child entities
        var child1 = new UIEntity("child1");
        var transform1 = child1.AddComponent<TransformComponent>();
        transform1.Size = new Vector2(50, 30);
        entity.AddChild(child1);

        var child2 = new UIEntity("child2");
        var transform2 = child2.AddComponent<TransformComponent>();
        transform2.Size = new Vector2(40, 20);
        entity.AddChild(child2);

        // Act
        var desiredSize = component.CalculateDesiredSize(new Vector2(200, 200));

        // Assert
        // Width: 50 + 40 + 10 (gap) = 100
        // Height: max(30, 20) = 30
        Assert.Equal(100f, desiredSize.X);
        Assert.Equal(30f, desiredSize.Y);
    }

    [Fact]
    public void CalculateDesiredSize_ReturnsCorrectSizeForColumnLayout()
    {
        // Arrange
        var entity = new UIEntity("container");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.FlexDirection = FlexDirection.Column;
        component.Gap = 5f;

        // Add child entities
        var child1 = new UIEntity("child1");
        var transform1 = child1.AddComponent<TransformComponent>();
        transform1.Size = new Vector2(50, 30);
        entity.AddChild(child1);

        var child2 = new UIEntity("child2");
        var transform2 = child2.AddComponent<TransformComponent>();
        transform2.Size = new Vector2(40, 20);
        entity.AddChild(child2);

        // Act
        var desiredSize = component.CalculateDesiredSize(new Vector2(200, 200));

        // Assert
        // Width: max(50, 40) = 50
        // Height: 30 + 20 + 5 (gap) = 55
        Assert.Equal(50f, desiredSize.X);
        Assert.Equal(55f, desiredSize.Y);
    }

    [Fact]
    public void Arrange_PositionsChildrenCorrectlyInRow()
    {
        // Arrange
        var entity = new UIEntity("container");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.FlexDirection = FlexDirection.Row;
        component.Gap = 10f;

        // Add child entities
        var child1 = new UIEntity("child1");
        var transform1 = child1.AddComponent<TransformComponent>();
        transform1.Size = new Vector2(50, 30);
        entity.AddChild(child1);

        var child2 = new UIEntity("child2");
        var transform2 = child2.AddComponent<TransformComponent>();
        transform2.Size = new Vector2(40, 20);
        entity.AddChild(child2);

        var containerRect = new Rectangle(0, 0, 200, 100);

        // Act
        component.Arrange(containerRect);

        // Assert
        // First child at (0, 0)
        Assert.Equal(0f, transform1.Position.X);
        Assert.Equal(0f, transform1.Position.Y);

        // Second child at (60, 0) - after first child (50) + gap (10)
        Assert.Equal(60f, transform2.Position.X);
        Assert.Equal(0f, transform2.Position.Y);
    }

    [Fact]
    public void Arrange_PositionsChildrenCorrectlyInColumn()
    {
        // Arrange
        var entity = new UIEntity("container");
        var component = entity.AddComponent<FlexLayoutComponent>();
        component.FlexDirection = FlexDirection.Column;
        component.Gap = 5f;

        // Add child entities
        var child1 = new UIEntity("child1");
        var transform1 = child1.AddComponent<TransformComponent>();
        transform1.Size = new Vector2(50, 30);
        entity.AddChild(child1);

        var child2 = new UIEntity("child2");
        var transform2 = child2.AddComponent<TransformComponent>();
        transform2.Size = new Vector2(40, 20);
        entity.AddChild(child2);

        var containerRect = new Rectangle(0, 0, 100, 200);

        // Act
        component.Arrange(containerRect);

        // Assert
        // First child at (0, 0)
        Assert.Equal(0f, transform1.Position.X);
        Assert.Equal(0f, transform1.Position.Y);

        // Second child at (0, 35) - after first child (30) + gap (5)
        Assert.Equal(0f, transform2.Position.X);
        Assert.Equal(35f, transform2.Position.Y);
    }

    [Fact]
    public void Validate_ReturnsTrue_WhenAttachedToEntity()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<FlexLayoutComponent>();

        // Act & Assert
        Assert.True(component.Validate());
    }

    [Fact]
    public void Validate_ReturnsFalse_WhenNotAttachedToEntity()
    {
        // Arrange
        var component = new FlexLayoutComponent();

        // Act & Assert
        Assert.False(component.Validate());
    }
}
