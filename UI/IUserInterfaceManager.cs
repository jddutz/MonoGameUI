using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI;

public interface IUserInterfaceManager
{
    /// <summary>
    /// Viewport that the user interface is rendered to.
    /// </summary>
    GraphicsDevice GraphicsDevice { get; }

    /// <summary>
    /// Content manager used to load assets for the user interface.
    /// </summary>
    ContentManager Content { get; }

    /// <summary>
    /// Viewport that the user interface is rendered to.
    /// </summary>
    Viewport Viewport { get; }

    /// <summary>
    /// Theme that the user interface uses to style elements.
    /// </summary>
    ITheme Theme { get; }

    /// <summary>
    /// User interface that is currently active.
    /// </summary>
    IUserInterface? Current { get; }

    /// <summary>
    /// Collection of user interfaces that have been navigated away from.
    /// Used for backward navigation.
    /// </summary>
    Stack<IUserInterface> History { get; }

    /// <summary>
    /// Prepares to load the specified user interface.
    /// Loading is delayed to avoid errors incurred by modifying the UI
    /// in the middle of an update. See <see cref="Update(float)"/> .
    /// </summary>
    void Load(IUserInterface ui);

    /// <summary>
    /// Called once per frame. Updates the user interface.
    /// If a user interface change is pending, the current user interface is unloaded
    /// then the pending user interface is loaded.
    /// </summary>
    void Update(float dt);

    /// <summary>
    /// Navigates back to the previous user interface.
    /// </summary>
    void Back();
}
