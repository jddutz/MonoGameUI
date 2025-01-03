using System;

namespace MonoGameUI.Elements;

/// <summary>
/// Event arguments for when the state of a element changes.
/// </summary>
/// <param name="newState">int representing the new state of the element.</param>
/// <param name="oldState">int representing the old state of the element.</param>
public class StateChangedEventArgs(int newState, int oldState) : EventArgs
{
    /// <summary>
    /// The new state of the element.
    /// </summary>
    public readonly int NewState = newState;

    /// <summary>
    /// The old state of the element.
    /// </summary>
    public readonly int OldState = oldState;

    /// <summary>
    /// Set to true to prevent the state from changing.
    /// </summary>
    public bool Cancelled = false;
}
