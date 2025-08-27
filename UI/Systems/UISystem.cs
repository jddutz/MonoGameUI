using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameUI.Core;

namespace MonoGameUI.Systems;

/// <summary>
/// Base class for systems that process entities with specific component combinations.
/// </summary>
public abstract class UISystem
{
    /// <summary>
    /// The UI world this system belongs to.
    /// </summary>
    public UIWorld? World { get; internal set; }

    /// <summary>
    /// Whether this system is currently enabled and processing entities.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Priority for system execution order (lower numbers execute first).
    /// </summary>
    public virtual int Priority { get; set; } = 0;

    /// <summary>
    /// Called when the system is added to a world.
    /// </summary>
    public virtual void OnAttached() { }

    /// <summary>
    /// Called when the system is removed from a world.
    /// </summary>
    public virtual void OnDetached() { }

    /// <summary>
    /// Update the system logic.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update.</param>
    public virtual void Update(float deltaTime) { }

    /// <summary>
    /// Get all entities in the world that have the specified component types.
    /// </summary>
    /// <param name="componentTypes">Required component types.</param>
    /// <returns>Entities with all specified components.</returns>
    protected IEnumerable<UIEntity> GetEntitiesWith(params Type[] componentTypes)
    {
        if (World == null)
        {
            yield break;
        }

        foreach (var entity in World.Entities)
        {
            if (!entity.Enabled)
            {
                continue;
            }

            bool hasAllComponents = true;
            foreach (var type in componentTypes)
            {
                if (!HasComponent(entity, type))
                {
                    hasAllComponents = false;
                    break;
                }
            }

            if (hasAllComponents)
            {
                yield return entity;
            }
        }
    }

    /// <summary>
    /// Get all entities with the specified component type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>Entities with the specified component.</returns>
    protected IEnumerable<UIEntity> GetEntitiesWith<T>() where T : class, IComponent
    {
        return GetEntitiesWith(typeof(T));
    }

    /// <summary>
    /// Get all entities with the specified component types.
    /// </summary>
    /// <typeparam name="T1">First component type.</typeparam>
    /// <typeparam name="T2">Second component type.</typeparam>
    /// <returns>Entities with both specified components.</returns>
    protected IEnumerable<UIEntity> GetEntitiesWith<T1, T2>()
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        return GetEntitiesWith(typeof(T1), typeof(T2));
    }

    private static bool HasComponent(UIEntity entity, Type componentType)
    {
        foreach (var component in entity.Components)
        {
            if (componentType.IsAssignableFrom(component.GetType()))
            {
                return true;
            }
        }
        return false;
    }
}

/// <summary>
/// System that processes layout calculations for entities.
/// </summary>
public class LayoutSystem : UISystem
{
    public override int Priority => 100; // Layout should happen before rendering

    public override void Update(float deltaTime)
    {
        if (!Enabled) return;

        // Process all entities that need layout updates
        foreach (var entity in GetEntitiesWith<Components.LayoutComponent, Components.TransformComponent>())
        {
            if (!entity.IsDirty(DirtyFlags.Layout))
            {
                continue;
            }

            var layout = entity.GetComponent<Components.LayoutComponent>()!;
            var transform = entity.GetComponent<Components.TransformComponent>()!;

            // Calculate layout for this entity
            CalculateLayout(entity, layout, transform);

            // Clear layout dirty flag
            entity.DirtyFlags &= ~DirtyFlags.Layout;
        }
    }

    private void CalculateLayout(UIEntity entity, Components.LayoutComponent layout, Components.TransformComponent transform)
    {
        // Get parent bounds or use viewport
        Rectangle parentBounds;
        if (entity.Parent?.GetComponent<Components.TransformComponent>() is Components.TransformComponent parentTransform)
        {
            parentBounds = parentTransform.LocalBounds;
        }
        else
        {
            // Use world viewport bounds - for now just use a default
            parentBounds = new Microsoft.Xna.Framework.Rectangle(0, 0, 1920, 1080);
        }

        // Calculate desired size and arrange
        var desiredSize = layout.CalculateDesiredSize(new Microsoft.Xna.Framework.Vector2(parentBounds.Width, parentBounds.Height));
        layout.Arrange(parentBounds);
    }
}

/// <summary>
/// Simple system that validates all entities and their components.
/// </summary>
public class ValidationSystem : UISystem
{
    public override int Priority => 1000; // Validation should happen last

    public override void Update(float deltaTime)
    {
        if (!Enabled) return;

        foreach (var entity in World?.Entities ?? Array.Empty<UIEntity>())
        {
            if (!entity.Validate())
            {
                // Log validation error or handle invalid entity
                System.Diagnostics.Debug.WriteLine($"Validation failed for entity: {entity}");
            }
        }
    }
}

/// <summary>
/// Container for UI entities and systems, representing a complete UI scene or world.
/// </summary>
public class UIWorld
{
    private readonly List<UIEntity> _entities = new();
    private readonly List<UISystem> _systems = new();

    /// <summary>
    /// All entities in this world.
    /// </summary>
    public IReadOnlyList<UIEntity> Entities => _entities.AsReadOnly();

    /// <summary>
    /// All systems in this world.
    /// </summary>
    public IReadOnlyList<UISystem> Systems => _systems.AsReadOnly();

    /// <summary>
    /// Add an entity to the world.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void AddEntity(UIEntity entity)
    {
        if (!_entities.Contains(entity))
        {
            _entities.Add(entity);
        }
    }

    /// <summary>
    /// Remove an entity from the world.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>True if the entity was removed.</returns>
    public bool RemoveEntity(UIEntity entity)
    {
        return _entities.Remove(entity);
    }

    /// <summary>
    /// Add a system to the world.
    /// </summary>
    /// <param name="system">The system to add.</param>
    public void AddSystem(UISystem system)
    {
        if (!_systems.Contains(system))
        {
            _systems.Add(system);
            system.World = this;
            system.OnAttached();

            // Sort systems by priority
            _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }

    /// <summary>
    /// Remove a system from the world.
    /// </summary>
    /// <param name="system">The system to remove.</param>
    /// <returns>True if the system was removed.</returns>
    public bool RemoveSystem(UISystem system)
    {
        if (_systems.Remove(system))
        {
            system.OnDetached();
            system.World = null;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Update all systems in the world.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update.</param>
    public void Update(float deltaTime)
    {
        foreach (var system in _systems)
        {
            if (system.Enabled)
            {
                system.Update(deltaTime);
            }
        }
    }

    /// <summary>
    /// Find an entity by name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>The found entity, or null if not found.</returns>
    public UIEntity? FindEntity(string name)
    {
        return _entities.Find(e => e.Name == name);
    }

    /// <summary>
    /// Clear all entities and systems.
    /// </summary>
    public void Clear()
    {
        foreach (var system in _systems)
        {
            system.OnDetached();
            system.World = null;
        }
        _systems.Clear();
        _entities.Clear();
    }
}
