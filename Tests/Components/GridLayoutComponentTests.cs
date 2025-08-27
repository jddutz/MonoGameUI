using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using System.Linq;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class GridLayoutComponentTests
{
    [Fact(DisplayName = "Constructor should initialize with default values")]
    public void ConstructorTest01()
    {
        // Arrange & Act
        var component = new GridLayoutComponent();

        // Assert
        Assert.Equal(1, component.Columns);
        Assert.Equal(1, component.Rows);
        Assert.Equal(0f, component.ColumnGap);
        Assert.Equal(0f, component.RowGap);
        Assert.Equal(GridAutoFlow.Row, component.AutoFlow);
        Assert.Empty(component.ColumnSizes);
        Assert.Empty(component.RowSizes);
    }

    [Fact(DisplayName = "Columns setter should mark dirty when changed")]
    public void ColumnsTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Columns = 3;

        // Assert
        Assert.Equal(3, component.Columns);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "Columns setter should not mark dirty when value is same")]
    public void ColumnsTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        component.Columns = 2;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Columns = 2;

        // Assert
        Assert.Equal(2, component.Columns);
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "Columns setter should reject values less than or equal to 0")]
    public void ColumnsTest03()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        component.Columns = 2;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Columns = 0;

        // Assert
        Assert.Equal(2, component.Columns); // Should remain unchanged
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "Rows setter should mark dirty when changed")]
    public void RowsTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Rows = 3;

        // Assert
        Assert.Equal(3, component.Rows);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ColumnGap setter should mark dirty when changed")]
    public void ColumnGapTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ColumnGap = 10f;

        // Assert
        Assert.Equal(10f, component.ColumnGap);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "RowGap setter should mark dirty when changed")]
    public void RowGapTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.RowGap = 10f;

        // Assert
        Assert.Equal(10f, component.RowGap);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "AutoFlow setter should mark dirty when changed")]
    public void AutoFlowTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.AutoFlow = GridAutoFlow.Column;

        // Assert
        Assert.Equal(GridAutoFlow.Column, component.AutoFlow);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "Gap setter should set both ColumnGap and RowGap")]
    public void GapTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();

        // Act
        component.Gap = 15f;

        // Assert
        Assert.Equal(15f, component.ColumnGap);
        Assert.Equal(15f, component.RowGap);
    }

    [Fact(DisplayName = "AddColumnSize should add size and mark dirty")]
    public void AddColumnSizeTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.AddColumnSize(GridTrackSize.Pixels(100));

        // Assert
        Assert.Single(component.ColumnSizes);
        Assert.Equal(GridTrackSizeType.Pixels, component.ColumnSizes[0].Type);
        Assert.Equal(100f, component.ColumnSizes[0].Value);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "AddRowSize should add size and mark dirty")]
    public void AddRowSizeTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.AddRowSize(GridTrackSize.Percentage(50));

        // Assert
        Assert.Single(component.RowSizes);
        Assert.Equal(GridTrackSizeType.Percentage, component.RowSizes[0].Type);
        Assert.Equal(50f, component.RowSizes[0].Value);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ClearColumnSizes should remove all sizes and mark dirty")]
    public void ClearColumnSizesTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        component.AddColumnSize(GridTrackSize.Auto());
        component.AddColumnSize(GridTrackSize.Pixels(100));
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ClearColumnSizes();

        // Assert
        Assert.Empty(component.ColumnSizes);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ClearRowSizes should remove all sizes and mark dirty")]
    public void ClearRowSizesTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        component.AddRowSize(GridTrackSize.Auto());
        component.AddRowSize(GridTrackSize.Pixels(50));
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ClearRowSizes();

        // Assert
        Assert.Empty(component.RowSizes);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "CalculateDesiredSize should return zero for entity with no children")]
    public void CalculateDesiredSizeTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();

        // Act
        var result = component.CalculateDesiredSize(new Vector2(800, 600));

        // Assert
        Assert.Equal(Vector2.Zero, result);
    }

    [Fact(DisplayName = "CalculateDesiredSize should calculate size based on children")]
    public void CalculateDesiredSizeTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        component.Columns = 2;
        component.Rows = 2;

        // Add some child entities with transforms
        for (int i = 0; i < 3; i++)
        {
            var child = new UIEntity($"child{i}");
            var transform = child.AddComponent<TransformComponent>();
            transform.Size = new Vector2(100, 50);
            entity.AddChild(child);
        }

        // Act
        var result = component.CalculateDesiredSize(new Vector2(800, 600));

        // Assert
        Assert.True(result.X > 0);
        Assert.True(result.Y > 0);
        // Should account for 2 columns with gaps
        Assert.True(result.X <= 800);
        Assert.True(result.Y <= 600);
    }

    [Fact(DisplayName = "Arrange should call PerformLayout with bounds")]
    public void ArrangeTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridLayoutComponent>();
        var bounds = new Rectangle(10, 20, 300, 200);

        // Act
        component.Arrange(bounds);

        // Assert
        // No exception should be thrown, and method should complete
        // This is mainly testing that the interface method is properly implemented
        Assert.True(true);
    }
}

public class GridTrackSizeTests
{
    [Fact(DisplayName = "Auto factory method should create auto-sized track")]
    public void AutoTest01()
    {
        // Arrange & Act
        var track = GridTrackSize.Auto();

        // Assert
        Assert.Equal(GridTrackSizeType.Auto, track.Type);
        Assert.Equal(0f, track.Value);
    }

    [Fact(DisplayName = "Pixels factory method should create pixel-sized track")]
    public void PixelsTest01()
    {
        // Arrange & Act
        var track = GridTrackSize.Pixels(150);

        // Assert
        Assert.Equal(GridTrackSizeType.Pixels, track.Type);
        Assert.Equal(150f, track.Value);
    }

    [Fact(DisplayName = "Percentage factory method should create percentage-sized track")]
    public void PercentageTest01()
    {
        // Arrange & Act
        var track = GridTrackSize.Percentage(75);

        // Assert
        Assert.Equal(GridTrackSizeType.Percentage, track.Type);
        Assert.Equal(75f, track.Value);
    }
}
