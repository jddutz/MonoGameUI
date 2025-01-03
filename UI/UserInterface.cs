using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Elements;

namespace MonoGameUI;

/// <summary>
/// Defines an interface through which the user interacts with the game.
/// Typically represents a single menu screen or game mode.
/// </summary>
public class UserInterface : IUserInterface
{
    /// <summary>
    /// Gets or sets a reference to the user interface manager
    /// used for navigation between different user interfaces.
    /// </summary>
    public IUserInterfaceManager? Manager { get; set; }

    /// <summary>
    /// Gets the graphics device that the user interface is rendered with.
    /// </summary>
    /// <returns>Graphics device used to render the user interface.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user interface manager has not been set.</exception>
    protected virtual GraphicsDevice GetGraphicsDevice()
    {
        if (Manager == null)
            throw new InvalidOperationException("User interface manager has not been set.");

        return Manager.GraphicsDevice;
    }

    /// <summary>
    /// Gets the graphics device that the user interface is rendered with.
    /// </summary>
    public GraphicsDevice GraphicsDevice => GetGraphicsDevice();

    /// <summary>
    /// Gets the viewport that the user interface is rendered to.
    /// </summary>
    protected virtual Viewport GetViewport()
    {
        if (Manager == null)
            throw new InvalidOperationException("User interface manager has not been set.");

        return Manager.Viewport;
    }

    /// <summary>
    /// Gets the viewport that the user interface is rendered to.
    /// </summary>
    public Viewport Viewport => GetViewport();

    /// <summary>
    /// Gets the content manager used to load assets required by the user interface.
    /// </summary>
    protected virtual ContentManager GetContentManager()
    {
        if (Manager == null)
            throw new InvalidOperationException("User interface manager has not been set.");

        return Manager.Content;
    }

    /// <summary>
    /// Gets the content manager used to load assets required by the user interface.
    /// </summary>
    public ContentManager Content => GetContentManager();

    /// <summary>
    /// Gets the theme that the user interface uses to stylize elements.
    /// </summary>
    protected virtual ITheme GetTheme()
    {
        if (Manager == null)
            throw new InvalidOperationException("User interface manager has not been set.");

        return Manager.Theme;
    }

    /// <summary>
    /// Gets the theme that the user interface uses to stylize elements.
    /// </summary>
    public ITheme Theme => GetTheme();

    /// <summary>
    /// Collection of UI elements that make up the user interface.
    /// </summary>
    private readonly List<Element> _elements = [];
    public IEnumerable<Element> Elements => _elements;

    /// <summary>
    /// Adds an element to the user interface.
    /// </summary>
    /// <param name="element">Element to add to the user interface.</param>
    public void Add(Element element)
    {
        _elements.Add(element);
        element.Parent = null;
    }

    /// <summary>
    /// Removes an element from the user interface.
    /// </summary>
    /// <param name="element">Element to remove from the user interface.</param>
    public bool Remove(Element element)
    {
        element.Parent = null;
        return _elements.Remove(element);
    }

    /// <summary>
    /// Removes all elements from the user interface.
    /// </summary>
    /// <param name="element">Element to remove from the user interface.</param>
    public void Clear()
    {
        foreach (Element element in _elements)
        {
            element.Parent = null;
        }
        _elements.Clear();
    }

    /// <summary>
    /// Element that is currently receiving user input.
    /// </summary>
    public Element? FocusedElement { get; private set; }

    /// <summary>
    /// Changes the element that currently has user input focus.
    /// </summary>
    /// <param name="element">Element to set focus to. If null, focus is removed from the current element.</param>
    public virtual bool SetFocus(Element element)
    {
        FocusedElement?.SetFocus(false);

        if (element != null && element.SetFocus(true))
        {
            FocusedElement = element;
            return true;
        }

        FocusedElement = null;
        return false;
    }

    /// <summary>
    /// Controller for handling mouse input events.
    /// </summary>
    public MouseInputController MouseInput { get; } = new();

    /// <summary>
    /// Controller for handling touch input events.
    /// </summary>
    public TouchInputController TouchInput { get; } = new();

    /// <summary>
    /// Controller for handling keyboard input events.
    /// </summary>
    public KeyboardInputController KeyboardInput { get; } = new();

    /// <summary>
    /// Controller for handling gamepad input events.
    /// </summary>
    public GamepadInputController GamepadInput { get; } = new();

    /// <summary>
    /// Called when the user interface is initialized.
    /// This method is only called once, when the user interface is first created.
    /// Override this method to add components and elements to the user interface.
    /// </summary>
    public virtual void Build() { }

    /// <summary>
    /// Called when the user interface is activated, before the first call to Update.
    /// Override this method to load resources such as textures and sounds.
    /// This method may be called multiple times if the user interface is unloaded and then reloaded.
    /// </summary>
    public virtual bool Activate()
    {
        if (Manager == null)
            throw new InvalidOperationException("User interface manager has not been set.");

        bool result = true;
        foreach (Element element in Elements)
        {
            if (!element.Activate()) result = false;
        }

        return result;
    }

    /// <summary>
    /// Called before the UI is unloaded by the user interface manager.
    /// Override this function to free up resources such as textures and sounds.
    /// </summary>
    public virtual void Unload()
    {
        foreach (Element element in Elements)
        {
            element.Unload();
        }
    }

    /// <summary>
    /// Resizes all child elements based on the size of the game window.
    /// Called when the user interface is first loaded, and whenever the game window is resized.
    /// </summary>
    public virtual void Resize()
    {
        foreach (Element element in Elements)
        {
            element.Resize();
        }
    }

    /// <summary>
    /// Updates the user interface. Called once per frame to update the state of UI elements.
    /// </summary>
    /// <param name="dt">Elapsed game time since the previous frame.</param>
    public virtual void Update(float dt)
    {
        MouseInput.Update(dt);

        // Ignore the mouse if it is outside the viewport
        if (MouseInput.CurrentState.X < 0) return;
        if (MouseInput.CurrentState.X > Viewport.Width) return;
        if (MouseInput.CurrentState.Y < 0) return;
        if (MouseInput.CurrentState.Y > Viewport.Height) return;

        // Update all child elements
        bool mouseInputHandled = false;
        foreach (Element element in Elements)
        {
            if (!mouseInputHandled && element is IMouseInputEventHandler h)
            {
                if (h.HandleMouseEvents(MouseInput)) mouseInputHandled = true;
            }

            element.Update(dt);
        }

        if (!mouseInputHandled) HandleMouseEvents();
    }

    /// <summary>
    /// Placeholder for handling mouse input events.
    /// </summary>
    public virtual void HandleMouseEvents() { }

    /// <summary>
    /// Draws all child elements of the user interface to the screen.
    /// </summary>
    public virtual void Draw(SpriteBatch batch)
    {
        batch.Begin();
        foreach (Element element in Elements)
        {
            element.Draw(batch);
        }
        batch.End();
    }
}
