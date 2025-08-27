using MonoGameUI.Core;
using Xunit;

namespace MonoGameUI.Tests.Core;

public class ComponentCollectionTests
{
    // Test component classes
    private class TestComponent : MonoGameUI.Core.Component
    {
        public string TestProperty { get; set; } = "Test";
        public bool OnAttachedCalled { get; private set; }
        public bool OnDetachedCalled { get; private set; }

        public override void OnAttached()
        {
            base.OnAttached();
            OnAttachedCalled = true;
        }

        public override void OnDetached()
        {
            base.OnDetached();
            OnDetachedCalled = true;
        }
    }

    private class AnotherTestComponent : MonoGameUI.Core.Component
    {
        public int Value { get; set; } = 42;
    }

    [Fact]
    public void Add_WithInstance_AddsComponentToCollection()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = new TestComponent();

        // Act
        var result = entity.Components.Add(component);

        // Assert
        Assert.Equal(component, result);
        Assert.True(entity.Components.Has<TestComponent>());
        Assert.Equal(component, entity.Components.Get<TestComponent>());
        Assert.Equal(entity, component.Entity);
        Assert.True(component.OnAttachedCalled);
    }

    [Fact]
    public void Add_WithGeneric_CreatesAndAddsComponent()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");

        // Act
        var result = entity.Components.Add<TestComponent>();

        // Assert
        Assert.NotNull(result);
        Assert.True(entity.Components.Has<TestComponent>());
        Assert.Equal(result, entity.Components.Get<TestComponent>());
        Assert.Equal(entity, result.Entity);
        Assert.True(result.OnAttachedCalled);
    }

    [Fact]
    public void Add_DuplicateType_ThrowsException()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        entity.Components.Add<TestComponent>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            entity.Components.Add<TestComponent>());

        Assert.Contains("already exists", exception.Message);
        Assert.Contains("TestComponent", exception.Message);
    }

    [Fact]
    public void Get_ExistingComponent_ReturnsComponent()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TestComponent>();

        // Act
        var result = entity.Components.Get<TestComponent>();

        // Assert
        Assert.Equal(component, result);
    }

    [Fact]
    public void Get_NonExistentComponent_ReturnsNull()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");

        // Act
        var result = entity.Components.Get<TestComponent>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Has_ExistingComponent_ReturnsTrue()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        entity.Components.Add<TestComponent>();

        // Act
        var result = entity.Components.Has<TestComponent>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Has_NonExistentComponent_ReturnsFalse()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");

        // Act
        var result = entity.Components.Has<TestComponent>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Remove_ByType_RemovesComponentAndCallsOnDetached()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TestComponent>();

        // Act
        var result = entity.Components.Remove<TestComponent>();

        // Assert
        Assert.True(result);
        Assert.False(entity.Components.Has<TestComponent>());
        Assert.Null(entity.Components.Get<TestComponent>());
        Assert.True(component.OnDetachedCalled);
    }

    [Fact]
    public void Remove_ByInstance_RemovesComponentAndCallsOnDetached()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component = entity.Components.Add<TestComponent>();

        // Act
        var result = entity.Components.Remove(component);

        // Assert
        Assert.True(result);
        Assert.False(entity.Components.Has<TestComponent>());
        Assert.Null(entity.Components.Get<TestComponent>());
        Assert.True(component.OnDetachedCalled);
    }

    [Fact]
    public void Remove_NonExistentComponent_ReturnsFalse()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");

        // Act
        var result = entity.Components.Remove<TestComponent>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Remove_DifferentInstance_ReturnsFalse()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        entity.Components.Add<TestComponent>();
        var differentComponent = new TestComponent();

        // Act
        var result = entity.Components.Remove(differentComponent);

        // Assert
        Assert.False(result);
        Assert.True(entity.Components.Has<TestComponent>());
    }

    [Fact]
    public void Clear_RemovesAllComponentsAndCallsOnDetached()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component1 = entity.Components.Add<TestComponent>();
        var component2 = entity.Components.Add<AnotherTestComponent>();

        // Act
        entity.Components.Clear();

        // Assert
        Assert.False(entity.Components.Has<TestComponent>());
        Assert.False(entity.Components.Has<AnotherTestComponent>());
        Assert.True(component1.OnDetachedCalled);
        // Note: AnotherTestComponent doesn't override OnDetached, so we can't test that
    }

    [Fact]
    public void Count_ReturnsCorrectNumber()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");

        // Act & Assert
        Assert.Equal(0, entity.Components.Count);

        entity.Components.Add<TestComponent>();
        Assert.Equal(1, entity.Components.Count);

        entity.Components.Add<AnotherTestComponent>();
        Assert.Equal(2, entity.Components.Count);

        entity.Components.Remove<TestComponent>();
        Assert.Equal(1, entity.Components.Count);
    }

    [Fact]
    public void Enumeration_IteratesOverAllComponents()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");
        var component1 = entity.Components.Add<TestComponent>();
        var component2 = entity.Components.Add<AnotherTestComponent>();

        // Act
        var components = new List<IComponent>();
        foreach (var component in entity.Components)
        {
            components.Add(component);
        }

        // Assert
        Assert.Equal(2, components.Count);
        Assert.Contains(component1, components);
        Assert.Contains(component2, components);
    }

    [Fact]
    public void MultipleComponentTypes_WorkIndependently()
    {
        // Arrange
        var entity = new UIEntity("TestEntity");

        // Act
        var testComponent = entity.Components.Add<TestComponent>();
        var anotherComponent = entity.Components.Add<AnotherTestComponent>();

        // Assert
        Assert.True(entity.Components.Has<TestComponent>());
        Assert.True(entity.Components.Has<AnotherTestComponent>());
        Assert.Equal(testComponent, entity.Components.Get<TestComponent>());
        Assert.Equal(anotherComponent, entity.Components.Get<AnotherTestComponent>());
        Assert.Equal(2, entity.Components.Count);
    }
}
