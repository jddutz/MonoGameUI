using System;

namespace MonoGameUI.Elements;


/// <summary>
///  Event handler for when the state of a element changes.
/// </summary>
/// <param name="sender">The element that raised the event.</param>
/// <param name="args">The event arguments.</param>
public delegate bool StateChangedEventHandler(object sender, StateChangedEventArgs args);