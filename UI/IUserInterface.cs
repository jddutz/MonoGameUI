using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Elements;

namespace MonoGameUI;

public interface IUserInterface
{
    IUserInterfaceManager? Manager { get; set; }

    /// <summary>
    /// Gets the graphics device that the user interface is rendered with.
    /// </summary>
    GraphicsDevice GraphicsDevice { get; }

    /// <summary>
    /// Gets the viewport that the user interface is rendered to.
    /// </summary>
    Viewport Viewport { get; }

    /// <summary>
    /// Gets the content manager used to load assets required by the user interface.
    /// </summary>
    ContentManager Content { get; }

    /// <summary>
    /// Gets the theme that the user interface uses to stylize elements.
    /// </summary>
    ITheme Theme { get; }

    /// <summary>
    /// Collection of UI elements that make up the user interface.
    /// </summary>
    IEnumerable<Element> Elements { get; }

    /// <summary>
    /// Adds an element to the user interface.
    /// </summary>
    /// <param name="element">Element to add to the user interface.</param>
    void Add(Element element);

    /// <summary>
    /// Removes an element from the user interface.
    /// </summary>
    /// <param name="element">Element to remove from the user interface.</param>
    bool Remove(Element element);

    /// <summary>
    /// Removes all elements from the user interface.
    /// </summary>
    /// <param name="element">Element to remove from the user interface.</param>
    void Clear();

    /// <summary>
    /// Element that is currently receiving user input.
    /// </summary>
    Element? FocusedElement { get; }

    /// <summary>
    /// Changes the element that current has user input focus.
    /// </summary>
    /// <param name="element">Element to set focus to. If null, focus is removed from the current element.</param>
    bool SetFocus(Element element);

    /// <summary>
    /// Controller for handling mouse input events.
    /// </summary>
    MouseInputController MouseInput { get; }

    /// <summary>
    /// Controller for handling touch input events.
    /// </summary>
    TouchInputController TouchInput { get; }

    /// <summary>
    /// Controller for handling keyboard input events.
    /// </summary>
    KeyboardInputController KeyboardInput { get; }

    /// <summary>
    /// Controller for handling gamepad input events.
    /// </summary>
    GamepadInputController GamepadInput { get; }

    /// <summary>
    /// Called when the user interface is initialized.
    /// This method is only called once, when the user interface is first created.
    /// Override this method to add components and elements to the user interface.
    /// </summary>
    void Build();

    /// <summary>
    /// Called when the user interface is activated, before the first call to Update.
    /// Override this method to load resources such as textures and sounds.
    /// This method may be called multiple times if the user interface is reactivated.
    /// </summary>
    bool Activate();

    /// <summary>
    /// Called before the UI is unloaded by the user interface manager.
    /// Override this function to free up resources such as textures and sounds.
    /// </summary>
    void Unload();

    /// <summary>
    /// Resizes all child elements based on the size of the game window.
    /// Called when the user interface is first loaded, and whenever the game window is resized.
    /// </summary>
    void Resize();

    /// <summary>
    /// Updates the user interface. Called once per frame to update the state of UI elements.
    /// </summary>
    /// <param name="dt">Elapsed game time since the previous frame.</param>
    void Update(float dt);

    /// <summary>
    /// Draws all child elements of the user interface to the screen.
    /// </summary>
    void Draw(SpriteBatch batch);
}

