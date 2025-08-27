using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class GridItemComponentTests
{
    [Fact(DisplayName = "Constructor should initialize with default values")]
    public void ConstructorTest01()
    {
        // Arrange & Act
        var component = new GridItemComponent();

        // Assert
        Assert.Equal(-1, component.Column);
        Assert.Equal(-1, component.Row);
        Assert.Equal(1, component.ColumnSpan);
        Assert.Equal(1, component.RowSpan);
    }

    [Fact(DisplayName = "Column setter should mark dirty when changed")]
    public void ColumnTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Column = 2;

        // Assert
        Assert.Equal(2, component.Column);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "Column setter should not mark dirty when value is same")]
    public void ColumnTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.Column = 1;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Column = 1;

        // Assert
        Assert.Equal(1, component.Column);
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "Row setter should mark dirty when changed")]
    public void RowTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.Row = 3;

        // Assert
        Assert.Equal(3, component.Row);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ColumnSpan setter should mark dirty when changed")]
    public void ColumnSpanTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ColumnSpan = 2;

        // Assert
        Assert.Equal(2, component.ColumnSpan);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ColumnSpan setter should reject values less than 1")]
    public void ColumnSpanTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.ColumnSpan = 2;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ColumnSpan = 0;

        // Assert
        Assert.Equal(2, component.ColumnSpan); // Should remain unchanged
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "RowSpan setter should mark dirty when changed")]
    public void RowSpanTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.RowSpan = 3;

        // Assert
        Assert.Equal(3, component.RowSpan);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "RowSpan setter should reject values less than 1")]
    public void RowSpanTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.RowSpan = 3;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.RowSpan = 0;

        // Assert
        Assert.Equal(3, component.RowSpan); // Should remain unchanged
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetPosition should update both column and row")]
    public void SetPositionTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetPosition(2, 1);

        // Assert
        Assert.Equal(2, component.Column);
        Assert.Equal(1, component.Row);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetPosition should not mark dirty when values are same")]
    public void SetPositionTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.Column = 2;
        component.Row = 1;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetPosition(2, 1);

        // Assert
        Assert.Equal(2, component.Column);
        Assert.Equal(1, component.Row);
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetPosition should mark dirty when only column changes")]
    public void SetPositionTest03()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.Column = 1;
        component.Row = 2;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetPosition(3, 2);

        // Assert
        Assert.Equal(3, component.Column);
        Assert.Equal(2, component.Row);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetSpan should update both columnSpan and rowSpan")]
    public void SetSpanTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetSpan(2, 3);

        // Assert
        Assert.Equal(2, component.ColumnSpan);
        Assert.Equal(3, component.RowSpan);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetSpan should not mark dirty when values are same")]
    public void SetSpanTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.ColumnSpan = 2;
        component.RowSpan = 3;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetSpan(2, 3);

        // Assert
        Assert.Equal(2, component.ColumnSpan);
        Assert.Equal(3, component.RowSpan);
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetSpan should reject invalid span values")]
    public void SetSpanTest03()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.ColumnSpan = 2;
        component.RowSpan = 3;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetSpan(0, -1);

        // Assert
        Assert.Equal(2, component.ColumnSpan); // Should remain unchanged
        Assert.Equal(3, component.RowSpan); // Should remain unchanged
        Assert.False(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "SetSpan should accept valid span with one invalid value")]
    public void SetSpanTest04()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<GridItemComponent>();
        component.ColumnSpan = 1;
        component.RowSpan = 1;
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.SetSpan(3, 0); // columnSpan valid, rowSpan invalid

        // Assert
        Assert.Equal(3, component.ColumnSpan); // Should change
        Assert.Equal(1, component.RowSpan); // Should remain unchanged
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }
}
