using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameUI.Core;

/// <summary>
/// Collection of components attached to a UI entity.
/// Provides fast lookup and type-safe access to components.
/// </summary>
public class ComponentCollection : IEnumerable<IComponent>
{
    private readonly Dictionary<Type, IComponent> _components = new();
    private readonly UIEntity _entity;

    internal ComponentCollection(UIEntity entity)
    {
        _entity = entity;
    }

    /// <summary>
    /// Get a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The component instance, or null if not found.</returns>
    public T? Get<T>() where T : class, IComponent
    {
        return _components.TryGetValue(typeof(T), out var component) ? component as T : null;
    }

    /// <summary>
    /// Check if a component of the specified type exists.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>True if the component exists.</returns>
    public bool Has<T>() where T : class, IComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Add a new component instance.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <param name="component">The component instance to add.</param>
    /// <returns>The added component.</returns>
    public T Add<T>(T component) where T : class, IComponent
    {
        var type = typeof(T);
        if (_components.ContainsKey(type))
        {
            throw new InvalidOperationException($"Component of type {type.Name} already exists on entity {_entity.Id}");
        }

        _components[type] = component;
        component.Entity = _entity;
        component.OnAttached();
        return component;
    }

    /// <summary>
    /// Add a new component of the specified type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The newly created component.</returns>
    public T Add<T>() where T : class, IComponent, new()
    {
        return Add(new T());
    }

    /// <summary>
    /// Remove a component of the specified type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>True if the component was removed.</returns>
    public bool Remove<T>() where T : class, IComponent
    {
        var type = typeof(T);
        if (_components.TryGetValue(type, out var component))
        {
            _components.Remove(type);
            component.OnDetached();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove the specified component instance.
    /// </summary>
    /// <param name="component">The component to remove.</param>
    /// <returns>True if the component was removed.</returns>
    public bool Remove(IComponent component)
    {
        var type = component.GetType();
        if (_components.TryGetValue(type, out var existing) && ReferenceEquals(existing, component))
        {
            _components.Remove(type);
            component.OnDetached();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clear all components.
    /// </summary>
    public void Clear()
    {
        foreach (var component in _components.Values)
        {
            component.OnDetached();
        }
        _components.Clear();
    }

    /// <summary>
    /// Get all components of a specific interface type.
    /// </summary>
    /// <typeparam name="T">The interface type.</typeparam>
    /// <returns>All components implementing the interface.</returns>
    public IEnumerable<T> GetAll<T>() where T : class
    {
        return _components.Values.OfType<T>();
    }

    /// <summary>
    /// Get the number of components.
    /// </summary>
    public int Count => _components.Count;

    public IEnumerator<IComponent> GetEnumerator() => _components.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
