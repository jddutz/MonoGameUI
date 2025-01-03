using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Elements;

namespace MonoGameUI;

public interface IMouseInputEventHandler
{
    /// <summary>
    /// Handles mouse input events for the given mouse input.
    /// </summary>
    /// <param name="mouseInput">The mouse input controller.</param>
    /// <returns>True if the event was handled, false otherwise.</returns>
    bool HandleMouseEvents(MouseInputController mouseInput);
}