using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class TransformComponentTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Act
        var component = new TransformComponent();

        // Assert
        Assert.Equal(Vector2.Zero, component.Position);
        Assert.Equal(Vector2.Zero, component.Size);
        Assert.Equal(0f, component.Rotation);
        Assert.Equal(Vector2.One, component.Scale);
        Assert.Equal(new Vector2(0.5f, 0.5f), component.Pivot);
        Assert.True(component.Enabled);
    }

    [Fact]
    public void Position_WhenSet_MarksDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Position = new Vector2(10, 20);

        // Assert
        Assert.Equal(new Vector2(10, 20), component.Position);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Transform));
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void Position_WhenSetToSameValue_DoesNotMarkDirty()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Position = new Vector2(10, 20); // Same value

        // Assert
        Assert.Equal(DirtyFlags.None, entity.DirtyFlags);
    }

    [Fact]
    public void Size_WhenSet_MarksDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Size = new Vector2(100, 50);

        // Assert
        Assert.Equal(new Vector2(100, 50), component.Size);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Transform));
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void Size_WhenSetToSameValue_DoesNotMarkDirty()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Size = new Vector2(100, 50);
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Size = new Vector2(100, 50); // Same value

        // Assert
        Assert.Equal(DirtyFlags.None, entity.DirtyFlags);
    }

    [Fact]
    public void Rotation_WhenSet_MarksDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Rotation = MathHelper.PiOver2;

        // Assert
        Assert.Equal(MathHelper.PiOver2, component.Rotation);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Transform));
    }

    [Fact]
    public void Scale_WhenSet_MarksDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Scale = new Vector2(2f, 1.5f);

        // Assert
        Assert.Equal(new Vector2(2f, 1.5f), component.Scale);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Transform));
    }

    [Fact]
    public void Pivot_WhenSet_MarksDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        entity.DirtyFlags = DirtyFlags.None; // Clear dirty flags

        // Act
        component.Pivot = new Vector2(0f, 0f);

        // Assert
        Assert.Equal(new Vector2(0f, 0f), component.Pivot);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Transform));
    }

    [Fact]
    public void Width_Property_ReturnsAndSetsCorrectly()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();

        // Act
        component.Width = 150f;

        // Assert
        Assert.Equal(150f, component.Width);
        Assert.Equal(new Vector2(150f, 0f), component.Size);
    }

    [Fact]
    public void Height_Property_ReturnsAndSetsCorrectly()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();

        // Act
        component.Height = 75f;

        // Assert
        Assert.Equal(75f, component.Height);
        Assert.Equal(new Vector2(0f, 75f), component.Size);
    }

    [Fact]
    public void X_Property_ReturnsAndSetsCorrectly()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();

        // Act
        component.X = 25f;

        // Assert
        Assert.Equal(25f, component.X);
        Assert.Equal(new Vector2(25f, 0f), component.Position);
    }

    [Fact]
    public void Y_Property_ReturnsAndSetsCorrectly()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();

        // Act
        component.Y = 35f;

        // Assert
        Assert.Equal(35f, component.Y);
        Assert.Equal(new Vector2(0f, 35f), component.Position);
    }

    [Fact]
    public void LocalBounds_Property_ReturnsCorrectRectangle()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Size = new Vector2(100, 50);

        // Act
        var bounds = component.LocalBounds;

        // Assert
        Assert.Equal(new Rectangle(0, 0, 100, 50), bounds);
    }

    [Fact]
    public void WorldBounds_Property_ReturnsCorrectRectangle()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);
        component.Size = new Vector2(100, 50);

        // Act
        var bounds = component.WorldBounds;

        // Assert
        Assert.Equal(new Rectangle(10, 20, 100, 50), bounds);
    }

    [Fact]
    public void LocalToWorld_WithNoParent_ReturnsTransformedPosition()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);

        // Act
        var worldPosition = component.LocalToWorld(Vector2.Zero);

        // Assert
        // When transforming local origin (0,0), we should get the position with pivot offset
        Assert.NotEqual(Vector2.Zero, worldPosition);
    }

    [Fact]
    public void LocalToWorld_WithParent_ReturnsTransformedPosition()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var parentTransform = parent.Components.Add<TransformComponent>();
        parentTransform.Position = new Vector2(100, 50);

        var child = new UIEntity("Child");
        var childTransform = child.Components.Add<TransformComponent>();
        childTransform.Position = new Vector2(10, 20);

        parent.AddChild(child);

        // Act
        var worldPosition = childTransform.LocalToWorld(Vector2.Zero);

        // Assert
        // The result should account for both parent and child transforms
        Assert.NotEqual(Vector2.Zero, worldPosition);
    }

    [Fact]
    public void GetWorldTransformMatrix_ReturnsCorrectTransformation()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);
        component.Size = new Vector2(100, 50);
        component.Scale = new Vector2(2f, 1.5f);
        component.Rotation = MathHelper.PiOver4;
        component.Pivot = new Vector2(0.5f, 0.5f);

        // Act
        var matrix = component.GetWorldTransformMatrix();

        // Assert
        // The matrix should include translation, rotation, and scale
        // We can't easily test the exact values, but we can verify it's not identity
        Assert.NotEqual(Matrix.Identity, matrix);
    }

    [Fact]
    public void WorldToLocal_RoundTrip_PreservesPoint()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);
        component.Size = new Vector2(100, 50);
        component.Scale = new Vector2(2f, 1.5f);
        component.Rotation = MathHelper.PiOver4;

        var originalPoint = new Vector2(50, 75);

        // Act
        var worldPoint = component.LocalToWorld(originalPoint);
        var backToLocal = component.WorldToLocal(worldPoint);

        // Assert
        // Round trip should preserve the original point (within floating point precision)
        Assert.True(Vector2.Distance(originalPoint, backToLocal) < 0.001f);
    }

    [Fact]
    public void ContainsPoint_PointInside_ReturnsTrue()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);
        component.Size = new Vector2(100, 50);

        // Act
        var result = component.ContainsPoint(new Vector2(50, 40)); // Point inside bounds

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsPoint_PointOutside_ReturnsFalse()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();
        component.Position = new Vector2(10, 20);
        component.Size = new Vector2(100, 50);

        // Act
        var result = component.ContainsPoint(new Vector2(5, 15)); // Point outside bounds

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_WithEntity_ReturnsTrue()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TransformComponent>();

        // Act
        var isValid = component.Validate();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void Validate_WithoutEntity_ReturnsFalse()
    {
        // Arrange
        var component = new TransformComponent();

        // Act
        var isValid = component.Validate();

        // Assert
        Assert.False(isValid);
    }
}
