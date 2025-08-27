using Microsoft.Xna.Framework;
using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Core;

public class UIEntityTests
{
    [Fact]
    public void Constructor_WithEmptyName_SetsDefaultValues()
    {
        // Act
        var entity = new UIEntity();

        // Assert
        Assert.NotEqual(EntityId.None, entity.Id);
        Assert.Equal(string.Empty, entity.Name);
        Assert.Null(entity.Parent);
        Assert.Empty(entity.Children);
        Assert.True(entity.Enabled);
        Assert.Equal(DirtyFlags.All, entity.DirtyFlags);
        Assert.NotNull(entity.Components);
        Assert.NotNull(entity.Properties);
    }

    [Fact]
    public void Constructor_WithName_SetsNameCorrectly()
    {
        // Arrange
        const string testName = "TestEntity";

        // Act
        var entity = new UIEntity(testName);

        // Assert
        Assert.Equal(testName, entity.Name);
    }

    [Fact]
    public void Id_IsUniqueForEachEntity()
    {
        // Act
        var entity1 = new UIEntity();
        var entity2 = new UIEntity();

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    [Theory]
    [InlineData("Entity1")]
    [InlineData("")]
    [InlineData("Some Long Entity Name")]
    public void Name_CanBeSetAndRetrieved(string name)
    {
        // Arrange
        var entity = new UIEntity();

        // Act
        entity.Name = name;

        // Assert
        Assert.Equal(name, entity.Name);
    }

    [Fact]
    public void Enabled_CanBeSetAndRetrieved()
    {
        // Arrange
        var entity = new UIEntity();
        Assert.True(entity.Enabled); // Default state

        // Act & Assert
        entity.Enabled = false;
        Assert.False(entity.Enabled);

        entity.Enabled = true;
        Assert.True(entity.Enabled);
    }

    #region Hierarchy Tests

    [Fact]
    public void AddChild_AddsChildToCollection()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");

        // Act
        parent.AddChild(child);

        // Assert
        Assert.Contains(child, parent.Children);
        Assert.Equal(parent, child.Parent);
    }

    [Fact]
    public void AddChild_MarksDirtyLayout()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");
        parent.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        parent.AddChild(child);

        // Assert
        Assert.True(parent.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void AddChild_MovesChildFromPreviousParent()
    {
        // Arrange
        var parent1 = new UIEntity("Parent1");
        var parent2 = new UIEntity("Parent2");
        var child = new UIEntity("Child");

        parent1.AddChild(child);
        Assert.Equal(parent1, child.Parent);
        Assert.Contains(child, parent1.Children);

        // Act
        parent2.AddChild(child);

        // Assert
        Assert.Equal(parent2, child.Parent);
        Assert.Contains(child, parent2.Children);
        Assert.DoesNotContain(child, parent1.Children);
    }

    [Fact]
    public void RemoveChild_RemovesChildFromCollection()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");
        parent.AddChild(child);

        // Act
        var result = parent.RemoveChild(child);

        // Assert
        Assert.True(result);
        Assert.DoesNotContain(child, parent.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void RemoveChild_ReturnsFalseForNonChild()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var notChild = new UIEntity("NotChild");

        // Act
        var result = parent.RemoveChild(notChild);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveChild_MarksDirtyLayout()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");
        parent.AddChild(child);
        parent.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        parent.RemoveChild(child);

        // Assert
        Assert.True(parent.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void ClearChildren_RemovesAllChildren()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child1 = new UIEntity("Child1");
        var child2 = new UIEntity("Child2");
        parent.AddChild(child1);
        parent.AddChild(child2);

        // Act
        parent.ClearChildren();

        // Assert
        Assert.Empty(parent.Children);
        Assert.Null(child1.Parent);
        Assert.Null(child2.Parent);
    }

    [Fact]
    public void ClearChildren_MarksDirtyLayout()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");
        parent.AddChild(child);
        parent.DirtyFlags = DirtyFlags.None; // Clear flags

        // Act
        parent.ClearChildren();

        // Assert
        Assert.True(parent.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact]
    public void FindChild_FindsDirectChild()
    {
        // Arrange
        var parent = new UIEntity("Parent");
        var child1 = new UIEntity("Child1");
        var child2 = new UIEntity("Child2");
        parent.AddChild(child1);
        parent.AddChild(child2);

        // Act
        var found = parent.FindChild("Child2");

        // Assert
        Assert.Equal(child2, found);
    }

    [Fact]
    public void FindChild_ReturnsNullForNonExistentChild()
    {
        // Arrange
        var parent = new UIEntity("Parent");

        // Act
        var found = parent.FindChild("NonExistent");

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void FindChild_FindsNestedChildWhenRecursive()
    {
        // Arrange
        var grandparent = new UIEntity("Grandparent");
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");

        grandparent.AddChild(parent);
        parent.AddChild(child);

        // Act
        var found = grandparent.FindChild("Child", recursive: true);

        // Assert
        Assert.Equal(child, found);
    }

    [Fact]
    public void FindChild_DoesNotFindNestedChildWhenNotRecursive()
    {
        // Arrange
        var grandparent = new UIEntity("Grandparent");
        var parent = new UIEntity("Parent");
        var child = new UIEntity("Child");

        grandparent.AddChild(parent);
        parent.AddChild(child);

        // Act
        var found = grandparent.FindChild("Child", recursive: false);

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void GetRoot_ReturnsEntityWhenNoParent()
    {
        // Arrange
        var entity = new UIEntity("Entity");

        // Act
        var root = entity.GetRoot();

        // Assert
        Assert.Equal(entity, root);
    }

    [Fact]
    public void GetRoot_ReturnsRootAncestor()
    {
        // Arrange
        var root = new UIEntity("Root");
        var middle = new UIEntity("Middle");
        var leaf = new UIEntity("Leaf");

        root.AddChild(middle);
        middle.AddChild(leaf);

        // Act
        var foundRoot = leaf.GetRoot();

        // Assert
        Assert.Equal(root, foundRoot);
    }

    #endregion

    #region Dirty Tracking Tests

    [Fact]
    public void MarkDirty_SetsDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity();
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        entity.MarkDirty(DirtyFlags.Transform);

        // Assert
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Transform));
    }

    [Fact]
    public void MarkDirty_RaisesDirtyChangedEvent()
    {
        // Arrange
        var entity = new UIEntity();
        entity.DirtyFlags = DirtyFlags.None; // Clear flags first
        var eventRaised = false;
        UIEntity? eventEntity = null;
        DirtyFlags eventFlags = DirtyFlags.None;

        entity.DirtyChanged += (e, f) =>
        {
            eventRaised = true;
            eventEntity = e;
            eventFlags = f;
        };

        // Act
        entity.MarkDirty(DirtyFlags.Render);

        // Assert
        Assert.True(eventRaised);
        Assert.Equal(entity, eventEntity);
        Assert.True(eventFlags.HasFlag(DirtyFlags.Render));
    }

    [Fact]
    public void ClearDirty_RemovesAllDirtyFlags()
    {
        // Arrange
        var entity = new UIEntity();
        entity.DirtyFlags = DirtyFlags.All;

        // Act
        entity.ClearDirty();

        // Assert
        Assert.Equal(DirtyFlags.None, entity.DirtyFlags);
    }

    [Fact]
    public void IsDirty_ReturnsTrueWhenFlagsMatch()
    {
        // Arrange
        var entity = new UIEntity();
        entity.DirtyFlags = DirtyFlags.Transform | DirtyFlags.Layout;

        // Act & Assert
        Assert.True(entity.IsDirty(DirtyFlags.Transform));
        Assert.True(entity.IsDirty(DirtyFlags.Layout));
        Assert.True(entity.IsDirty(DirtyFlags.Transform | DirtyFlags.Layout));
        Assert.False(entity.IsDirty(DirtyFlags.Render));
    }

    #endregion
}
