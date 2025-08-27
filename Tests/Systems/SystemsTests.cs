using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUI.Components;
using MonoGameUI.Core;
using MonoGameUI.Systems;
using Xunit;

namespace MonoGameUI.Tests.Systems;

public class RenderSystemTests
{
    [Fact]
    public void RenderSystem_HasCorrectPriority()
    {
        var renderSystem = new RenderSystem();

        Assert.Equal(200, renderSystem.Priority);
    }

    [Fact]
    public void RenderSystem_InitializeRequiresGraphicsDeviceAndSpriteBatch()
    {
        var renderSystem = new RenderSystem();

        Assert.Throws<ArgumentNullException>(() => renderSystem.Initialize(null!, null!));
    }

    [Fact]
    public void RenderSystem_UpdateDoesNotThrowWhenNotInitialized()
    {
        var renderSystem = new RenderSystem();
        var world = new UIWorld();
        world.AddSystem(renderSystem);

        // Should not throw when not initialized
        var exception = Record.Exception(() => renderSystem.Update(0.016f));
        Assert.Null(exception);
    }

    [Fact]
    public void RenderSystem_CollectsRenderableEntities()
    {
        var renderSystem = new RenderSystem();
        var world = new UIWorld();
        world.AddSystem(renderSystem);

        // Create entity with renderable and transform components
        var entity = new UIEntity("TestEntity");
        entity.AddComponent<TransformComponent>();
        entity.AddComponent<RenderableComponent>();
        world.AddEntity(entity);

        // Mark as dirty to ensure it gets collected
        entity.MarkDirty(DirtyFlags.Render);

        // Should not throw - system handles missing graphics dependencies gracefully
        var exception = Record.Exception(() => renderSystem.Update(0.016f));
        Assert.Null(exception);
    }

    [Fact]
    public void RenderSystem_OnDetachedCleansUpResources()
    {
        var renderSystem = new RenderSystem();
        var world = new UIWorld();
        world.AddSystem(renderSystem);

        // Should not throw when cleaning up
        world.RemoveSystem(renderSystem);

        Assert.Null(renderSystem.World);
    }
}

public class InputSystemTests
{
    [Fact]
    public void InputSystem_HasCorrectPriority()
    {
        var inputSystem = new InputSystem();

        Assert.Equal(50, inputSystem.Priority);
    }

    [Fact]
    public void InputSystem_UpdateDoesNotThrowWithNoEntities()
    {
        var inputSystem = new InputSystem();
        var world = new UIWorld();
        world.AddSystem(inputSystem);

        var exception = Record.Exception(() => inputSystem.Update(0.016f));
        Assert.Null(exception);
    }

    [Fact]
    public void InputSystem_CollectsInputEntities()
    {
        var inputSystem = new InputSystem();
        var world = new UIWorld();
        world.AddSystem(inputSystem);

        // Create entity with input and transform components
        var entity = new UIEntity("TestEntity");
        entity.AddComponent<TransformComponent>();
        entity.AddComponent<InputComponent>();
        world.AddEntity(entity);

        // Should not throw
        var exception = Record.Exception(() => inputSystem.Update(0.016f));
        Assert.Null(exception);
    }

    [Fact]
    public void InputSystem_SetFocusChangesEntityFocus()
    {
        var inputSystem = new InputSystem();
        var world = new UIWorld();
        world.AddSystem(inputSystem);

        // Create entity that can receive focus
        var entity = new UIEntity("TestEntity");
        var input = entity.AddComponent<InputComponent>();
        input.CanReceiveFocus = true;
        world.AddEntity(entity);

        // Set focus
        inputSystem.SetFocus(entity);

        Assert.Equal(entity, inputSystem.FocusedEntity);
        Assert.True(input.HasFocus);
    }

    [Fact]
    public void InputSystem_SetFocusToNullClearsFocus()
    {
        var inputSystem = new InputSystem();
        var world = new UIWorld();
        world.AddSystem(inputSystem);

        // Create entity with focus
        var entity = new UIEntity("TestEntity");
        var input = entity.AddComponent<InputComponent>();
        input.CanReceiveFocus = true;
        world.AddEntity(entity);

        inputSystem.SetFocus(entity);
        Assert.True(input.HasFocus);

        // Clear focus
        inputSystem.SetFocus(null);

        Assert.Null(inputSystem.FocusedEntity);
        Assert.False(input.HasFocus);
    }

    [Fact]
    public void InputSystem_SetFocusToEntityThatCannotReceiveFocusDoesNotSetFocus()
    {
        var inputSystem = new InputSystem();
        var world = new UIWorld();
        world.AddSystem(inputSystem);

        // Create entity that cannot receive focus
        var entity = new UIEntity("TestEntity");
        var input = entity.AddComponent<InputComponent>();
        input.CanReceiveFocus = false;
        world.AddEntity(entity);

        // Try to set focus
        inputSystem.SetFocus(entity);

        Assert.Null(inputSystem.FocusedEntity);
        Assert.False(input.HasFocus);
    }

    [Fact]
    public void InputSystem_MovingFocusBetweenEntitiesUpdatesCorrectly()
    {
        var inputSystem = new InputSystem();
        var world = new UIWorld();
        world.AddSystem(inputSystem);

        // Create two entities that can receive focus
        var entity1 = new UIEntity("Entity1");
        var input1 = entity1.AddComponent<InputComponent>();
        input1.CanReceiveFocus = true;
        world.AddEntity(entity1);

        var entity2 = new UIEntity("Entity2");
        var input2 = entity2.AddComponent<InputComponent>();
        input2.CanReceiveFocus = true;
        world.AddEntity(entity2);

        // Set focus to first entity
        inputSystem.SetFocus(entity1);
        Assert.True(input1.HasFocus);
        Assert.False(input2.HasFocus);

        // Move focus to second entity
        inputSystem.SetFocus(entity2);
        Assert.False(input1.HasFocus);
        Assert.True(input2.HasFocus);
        Assert.Equal(entity2, inputSystem.FocusedEntity);
    }
}
