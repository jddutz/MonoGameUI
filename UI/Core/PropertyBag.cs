using System;
using System.Collections.Generic;

namespace MonoGameUI.Core;

/// <summary>
/// A dynamic property bag for storing arbitrary key-value pairs.
/// Supports change notifications and type-safe access.
/// </summary>
public class PropertyBag
{
    private readonly Dictionary<string, object?> _properties = new();

    /// <summary>
    /// Event raised when a property value changes.
    /// </summary>
    public event Action<string, object?, object?>? PropertyChanged;

    /// <summary>
    /// Get all property names.
    /// </summary>
    public IEnumerable<string> PropertyNames => _properties.Keys;

    /// <summary>
    /// Get a property value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>The property value, or null if not found.</returns>
    public object? Get(string name)
    {
        return _properties.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>
    /// Get a property value with a specific type.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="name">The property name.</param>
    /// <returns>The property value, or default(T) if not found or wrong type.</returns>
    public T? Get<T>(string name)
    {
        var value = Get(name);
        return value is T typedValue ? typedValue : default(T);
    }

    /// <summary>
    /// Get a property value with a default fallback.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="name">The property name.</param>
    /// <param name="defaultValue">The default value if property doesn't exist.</param>
    /// <returns>The property value or the default value.</returns>
    public T Get<T>(string name, T defaultValue)
    {
        var value = Get(name);
        return value is T typedValue ? typedValue : defaultValue;
    }

    /// <summary>
    /// Set a property value.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    public void Set(string name, object? value)
    {
        var oldValue = Get(name);
        if (!Equals(oldValue, value))
        {
            _properties[name] = value;
            PropertyChanged?.Invoke(name, oldValue, value);
        }
    }

    /// <summary>
    /// Check if a property exists.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>True if the property exists.</returns>
    public bool Has(string name)
    {
        return _properties.ContainsKey(name);
    }

    /// <summary>
    /// Remove a property.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>True if the property was removed.</returns>
    public bool Remove(string name)
    {
        if (_properties.TryGetValue(name, out var oldValue))
        {
            _properties.Remove(name);
            PropertyChanged?.Invoke(name, oldValue, null);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clear all properties.
    /// </summary>
    public void Clear()
    {
        var oldProperties = new Dictionary<string, object?>(_properties);
        _properties.Clear();

        foreach (var kvp in oldProperties)
        {
            PropertyChanged?.Invoke(kvp.Key, kvp.Value, null);
        }
    }

    /// <summary>
    /// Get the number of properties.
    /// </summary>
    public int Count => _properties.Count;
}
