using System;

namespace MonoGameUI.Core;

/// <summary>
/// Flags indicating which aspects of an entity need to be updated.
/// Used for performance optimization to avoid unnecessary calculations.
/// </summary>
[Flags]
public enum DirtyFlags
{
    /// <summary>
    /// No updates needed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Transform (position, size, rotation, scale) has changed.
    /// </summary>
    Transform = 1 << 0,

    /// <summary>
    /// Layout calculations need to be performed.
    /// </summary>
    Layout = 1 << 1,

    /// <summary>
    /// Visual representation needs to be re-rendered.
    /// </summary>
    Render = 1 << 2,

    /// <summary>
    /// Input handling needs to be updated (hit testing, focus, etc.).
    /// </summary>
    Input = 1 << 3,

    /// <summary>
    /// Data bindings need to be refreshed.
    /// </summary>
    Data = 1 << 4,

    /// <summary>
    /// Animation state has changed.
    /// </summary>
    Animation = 1 << 5,

    /// <summary>
    /// Theme or styling has changed.
    /// </summary>
    Style = 1 << 6,

    /// <summary>
    /// All aspects need updating.
    /// </summary>
    All = Transform | Layout | Render | Input | Data | Animation | Style
}

/// <summary>
/// Interface for objects that support dirty tracking for performance optimization.
/// </summary>
public interface IDirtyTrackable
{
    /// <summary>
    /// Current dirty flags indicating what needs to be updated.
    /// </summary>
    DirtyFlags DirtyFlags { get; set; }

    /// <summary>
    /// Mark specific aspects as dirty and needing updates.
    /// </summary>
    /// <param name="flags">The aspects to mark as dirty.</param>
    void MarkDirty(DirtyFlags flags);

    /// <summary>
    /// Clear all dirty flags, indicating everything is up to date.
    /// </summary>
    void ClearDirty();

    /// <summary>
    /// Check if any of the specified flags are dirty.
    /// </summary>
    /// <param name="flags">The flags to check.</param>
    /// <returns>True if any of the specified flags are dirty.</returns>
    bool IsDirty(DirtyFlags flags);
}
