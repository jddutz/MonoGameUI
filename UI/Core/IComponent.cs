using System;
using Microsoft.Xna.Framework;

namespace MonoGameUI.Core;

/// <summary>
/// Base interface for all UI components.
/// Components provide specific functionality to UI entities through composition.
/// </summary>
public interface IComponent
{
    /// <summary>
    /// The entity that owns this component.
    /// </summary>
    UIEntity? Entity { get; set; }

    /// <summary>
    /// Whether this component is currently enabled and active.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Called when the component is added to an entity.
    /// </summary>
    void OnAttached();

    /// <summary>
    /// Called when the component is removed from an entity.
    /// </summary>
    void OnDetached();

    /// <summary>
    /// Called when the component needs to validate its current state.
    /// </summary>
    /// <returns>True if the component is in a valid state.</returns>
    bool Validate();
}

/// <summary>
/// Base implementation of IComponent providing common functionality.
/// </summary>
public abstract class Component : IComponent
{
    public UIEntity? Entity { get; set; }
    public bool Enabled { get; set; } = true;

    public virtual void OnAttached() { }
    public virtual void OnDetached()
    {
        Entity = null;
    }

    public virtual bool Validate() => Entity is not null;

    /// <summary>
    /// Marks the entity as dirty for the specified flags.
    /// </summary>
    protected void MarkDirty(DirtyFlags flags)
    {
        Entity?.MarkDirty(flags);
    }
}

/// <summary>
/// Component that requires initialization resources (textures, fonts, etc.).
/// </summary>
public interface IInitializableComponent : IComponent
{
    /// <summary>
    /// Whether the component has been successfully initialized.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initialize the component with required resources.
    /// </summary>
    /// <returns>True if initialization was successful.</returns>
    bool Initialize();

    /// <summary>
    /// Clean up resources when the component is no longer needed.
    /// </summary>
    void Cleanup();
}

/// <summary>
/// Component that can be updated each frame.
/// </summary>
public interface IUpdateableComponent : IComponent
{
    /// <summary>
    /// Update the component logic.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update.</param>
    void Update(float deltaTime);
}

/// <summary>
/// Component that participates in the layout system.
/// </summary>
public interface ILayoutComponent : IComponent
{
    /// <summary>
    /// Calculate the desired size for this component.
    /// </summary>
    /// <param name="availableSize">Available space for the component.</param>
    /// <returns>The desired size.</returns>
    Vector2 CalculateDesiredSize(Vector2 availableSize);

    /// <summary>
    /// Arrange the component within the given bounds.
    /// </summary>
    /// <param name="bounds">The final bounds for the component.</param>
    void Arrange(Rectangle bounds);
}
