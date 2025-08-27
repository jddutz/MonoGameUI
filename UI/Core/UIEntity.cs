using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameUI.Core;

/// <summary>
/// Represents a UI entity in the component-based architecture.
/// An entity is a container for components that provide specific functionality.
/// </summary>
public class UIEntity : IDirtyTrackable
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public EntityId Id { get; }

    /// <summary>
    /// Optional name for debugging and identification purposes.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parent entity in the hierarchy.
    /// </summary>
    public UIEntity? Parent { get; private set; }

    /// <summary>
    /// Child entities.
    /// </summary>
    public IReadOnlyList<UIEntity> Children => _children.AsReadOnly();
    private readonly List<UIEntity> _children = new();

    /// <summary>
    /// Collection of components attached to this entity.
    /// </summary>
    public ComponentCollection Components { get; }

    /// <summary>
    /// Property bag for storing arbitrary data.
    /// </summary>
    public PropertyBag Properties { get; }

    /// <summary>
    /// Whether this entity is currently enabled and active.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Current dirty flags indicating what needs to be updated.
    /// </summary>
    public DirtyFlags DirtyFlags { get; set; } = DirtyFlags.All;

    /// <summary>
    /// Event raised when the entity's dirty state changes.
    /// </summary>
    public event Action<UIEntity, DirtyFlags>? DirtyChanged;

    public UIEntity(string name = "")
    {
        Id = EntityId.New();
        Name = name;
        Components = new ComponentCollection(this);
        Properties = new PropertyBag();
    }

    #region Hierarchy Management

    /// <summary>
    /// Add a child entity.
    /// </summary>
    /// <param name="child">The child entity to add.</param>
    public void AddChild(UIEntity child)
    {
        if (child.Parent != null)
        {
            child.Parent.RemoveChild(child);
        }

        _children.Add(child);
        child.Parent = this;
        MarkDirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// Remove a child entity.
    /// </summary>
    /// <param name="child">The child entity to remove.</param>
    /// <returns>True if the child was removed.</returns>
    public bool RemoveChild(UIEntity child)
    {
        if (_children.Remove(child))
        {
            child.Parent = null;
            MarkDirty(DirtyFlags.Layout);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove all child entities.
    /// </summary>
    public void ClearChildren()
    {
        foreach (var child in _children)
        {
            child.Parent = null;
        }
        _children.Clear();
        MarkDirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// Find a child entity by name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <param name="recursive">Whether to search recursively through descendants.</param>
    /// <returns>The found entity, or null if not found.</returns>
    public UIEntity? FindChild(string name, bool recursive = false)
    {
        var child = _children.FirstOrDefault(c => c.Name == name);
        if (child != null || !recursive)
        {
            return child;
        }

        foreach (var c in _children)
        {
            var found = c.FindChild(name, true);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the root entity of this hierarchy.
    /// </summary>
    /// <returns>The root entity.</returns>
    public UIEntity GetRoot()
    {
        var current = this;
        while (current.Parent != null)
        {
            current = current.Parent;
        }
        return current;
    }

    #endregion

    #region Component Management

    /// <summary>
    /// Get a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The component instance, or null if not found.</returns>
    public T? GetComponent<T>() where T : class, IComponent
    {
        return Components.Get<T>();
    }

    /// <summary>
    /// Check if a component of the specified type exists.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>True if the component exists.</returns>
    public bool HasComponent<T>() where T : class, IComponent
    {
        return Components.Has<T>();
    }

    /// <summary>
    /// Add a component to this entity.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <param name="component">The component instance to add.</param>
    /// <returns>The added component.</returns>
    public T AddComponent<T>(T component) where T : class, IComponent
    {
        return Components.Add(component);
    }

    /// <summary>
    /// Add a new component of the specified type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The newly created component.</returns>
    public T AddComponent<T>() where T : class, IComponent, new()
    {
        return Components.Add<T>();
    }

    /// <summary>
    /// Remove a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>True if the component was removed.</returns>
    public bool RemoveComponent<T>() where T : class, IComponent
    {
        return Components.Remove<T>();
    }

    #endregion

    #region Dirty Tracking

    /// <summary>
    /// Mark specific aspects as dirty and needing updates.
    /// </summary>
    /// <param name="flags">The aspects to mark as dirty.</param>
    public void MarkDirty(DirtyFlags flags)
    {
        var oldFlags = DirtyFlags;
        DirtyFlags |= flags;

        if (oldFlags != DirtyFlags)
        {
            DirtyChanged?.Invoke(this, flags);
        }
    }

    /// <summary>
    /// Clear all dirty flags, indicating everything is up to date.
    /// </summary>
    public void ClearDirty()
    {
        DirtyFlags = DirtyFlags.None;
    }

    /// <summary>
    /// Check if any of the specified flags are dirty.
    /// </summary>
    /// <param name="flags">The flags to check.</param>
    /// <returns>True if any of the specified flags are dirty.</returns>
    public bool IsDirty(DirtyFlags flags)
    {
        return (DirtyFlags & flags) != DirtyFlags.None;
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validate this entity and all its components.
    /// </summary>
    /// <returns>True if the entity is in a valid state.</returns>
    public bool Validate()
    {
        if (!Enabled)
        {
            return true; // Disabled entities are considered valid
        }

        // Validate all components
        foreach (var component in Components)
        {
            if (component.Enabled && !component.Validate())
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    public override string ToString()
    {
        return string.IsNullOrEmpty(Name) ? $"Entity({Id})" : $"Entity({Id}, '{Name}')";
    }
}
