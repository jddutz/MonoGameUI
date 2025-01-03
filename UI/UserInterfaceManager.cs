using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI;

/// <summary>
/// Manages user interfaces and provides a way to navigate between them.
/// </summary>/// 
public class UserInterfaceManager : IUserInterfaceManager
{
    /// <summary>
    /// Constructor used to create a new user interface manager for a game.
    /// </summary>
    /// <param name="game">Game instance that the user interface is attached to.</param>
    /// <param name="theme">Theme used to style elements.</param>
    public UserInterfaceManager(Game game, ITheme? theme = null)
    {
        if (game == null)
            throw new ArgumentNullException(nameof(game));

        GraphicsDevice = game.GraphicsDevice;
        Content = game.Content;
        Theme = theme ?? new Theme();
    }

    /// <summary>
    /// Viewport that the user interface is rendered to.
    /// </summary>
    public GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Content manager used to load assets for the user interface.
    /// </summary>
    public ContentManager Content { get; private set; }

    /// <summary>
    /// Theme used to style elements.
    /// </summary>
    public ITheme Theme { get; private set; }

    /// <summary>
    /// Viewport that the user interface is rendered to.
    /// </summary>
    public Viewport Viewport => GraphicsDevice.Viewport;

    /// <summary>
    /// User interface that is currently active.
    /// </summary>
    public IUserInterface? Current { get; private set; }

    /// <summary>
    /// Collection of user interfaces that have been navigated away from.
    /// Used for backward navigation.
    /// </summary>
    public Stack<IUserInterface> History { get; } = new();

    /// <summary>
    /// User interface that is pending to be loaded during the next Update() call.
    /// </summary>
    private IUserInterface? pending = null;

    /// <summary>
    /// Prepares a user interface to be loaded.
    /// 
    /// Calls <see cref="UserInterface.Build"/> and sets the Manager property.
    /// 
    /// Loading is delayed to avoid errors incurred by modifying the UI
    /// in the middle of an update. See <see cref="Update(float)"/>.
    /// </summary>
    /// <param name="ui">User interface to load.</param>
    /// <exception cref="ArgumentNullException">Thrown when the user interface is null.</exception>
    public void Load(IUserInterface ui)
    {
        if (ui == null)
            throw new ArgumentNullException(nameof(ui));

        if (Current == ui || pending == ui)
            return;

        ui.Manager = this;
        ui.Build();
        pending = ui;
    }

    /// <summary>
    /// Called once per frame. Updates the current user interface.
    /// If a change is pending, the current user interface is unloaded
    /// then the pending user interface is activated.
    /// </summary>
    public void Update(float dt)
    {
        if (dt < 0 || float.IsNaN(dt) || float.IsInfinity(dt))
            throw new ArgumentOutOfRangeException(nameof(dt), "Delta time must be non-negative.");

        if (pending != null)
        {
            if (Current != null)
            {
                History.Push(Current);
                Current.Unload();
            }

            Current = pending;
            pending = null;

            Current?.Activate();
        }

        Current?.Update(dt);
    }

    /// <summary>
    /// Navigates back to the previous user interface.
    /// </summary>
    public void Back()
    {
        if (History.Count > 0)
        {
            pending = History.Pop();
        }
    }
}