using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

/// <summary>
/// Base class for all user interface elements.
/// Implements basic functionality to control size, position, and appearance.
/// Cannot be animated, change state, or receive user input.
/// </summary>
public abstract class Element(IUserInterface ui)
{
    /// <summary>
    /// The user interface that contains the element.
    /// </summary>
    public IUserInterface UI { get; } = ui;

    /// <summary>
    /// The parent container of the element.
    /// </summary>
    public Container? Parent { get; set; }

    /// <summary>
    /// Object that can be used to store additional data
    /// to provide additional context to the element.
    public object? Context { get; set; }

    /// <summary>
    /// Defines the location of the element on the screen.
    /// </summary>
    private Rectangle _screenRect = Rectangle.Empty;
    public Rectangle ScreenRect
    {
        get => _screenRect;
        set
        {
            _screenRect = value;
        }
    }

    /// <summary>
    /// Flag indicating whether the element can receive focus.
    /// Should be overridden in derived classes that can receive focus.
    /// </summary>
    public virtual bool ReceivesFocus => false;

    /// <summary>
    /// Called by the UI when the element gains focus.
    /// </summary>
    /// <returns>True if the element accepts focus, False otherwise.</returns>
    public virtual bool SetFocus(bool value) => ReceivesFocus;

    /// <summary>
    /// Flag indicating content has been loaded.
    /// </summary>
    public bool IsLoaded { get; protected set; }

    /// <summary>
    /// Called when the user interface is loading.
    /// Override this method to load content from the ContentManager.
    /// </summary>
    public virtual void LoadContent()
    {
        IsLoaded = true;
    }

    /// <summary>
    /// Called when the element is unloaded to free resources.
    /// </summary>
    public virtual void Unload() { }

    /// <summary>
    /// Flag indicating content whether has been loaded 
    /// and all properties are defined and valid.
    /// </summary>
    public bool IsValid { get; protected set; }

    /// <summary>
    /// Validates the element, checking all properties are properly defined.
    /// </summary>
    /// <returns>True if all properties are valid, False otherwise.</returns>
    public virtual bool Validate()
    {
        if (UI == null)
        {
            IsValid = false;
            return false;
        }

        if (HorizontalSizeMode == SizeMode.ASPECT_RATIO && VerticalSizeMode == SizeMode.ASPECT_RATIO)
        {
            IsValid = false;
            return false;
        }

        if (HorizontalSizeMode == SizeMode.PERCENT && RelativeSize.X <= 0)
        {
            IsValid = false;
            return false;
        }

        if (VerticalSizeMode == SizeMode.PERCENT && RelativeSize.Y <= 0)
        {
            IsValid = false;
            return false;
        }

        if (HorizontalSizeMode == SizeMode.FIT_CONTENT && this is not Container)
        {
            IsValid = false;
            return false;
        }

        if (VerticalSizeMode == SizeMode.FIT_CONTENT && this is not Container)
        {
            IsValid = false;
            return false;
        }

        // TODO: Check if the element is valid

        IsValid = true;
        return true;
    }

    /// <summary>
    /// Invalidates the element, forcing it to be validated and re-activated.
    /// </summary>
    public void Invalidate()
    {
        IsValid = false;
    }

    /// <summary>
    /// Flag indicating whether the element is currently active.
    /// </summary>
    public bool Enabled { get; protected set; }

    /// <summary>
    /// Flag indicating whether the element is currently visible.
    /// Determines if the element should be drawn when the Draw method is called.
    /// </summary>
    public bool Visible { get; set; } = true;

    #region Size

    /// <summary>
    /// Gets or sets the size of the element.
    /// </summary>

    public Point Size
    {
        get => ScreenRect.Size;
        set
        {
            ScreenRect = new(ScreenRect.Location, value);
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the width of the element.
    /// </summary>
    public int Width
    {
        get => ScreenRect.Size.X;
        set
        {
            Size = new(value, ScreenRect.Size.Y);
        }
    }

    /// <summary>
    /// Gets or sets the height of the element.
    /// </summary>
    public int Height
    {
        get => ScreenRect.Size.Y;
        set
        {
            Size = new(ScreenRect.Size.X, value);
        }
    }

    /// <summary>
    /// Defines how the element's width is calculated.
    /// </summary>
    private SizeMode _hSizeMode = SizeMode.FIXED;
    public SizeMode HorizontalSizeMode
    {
        get => _hSizeMode;
        set
        {
            _hSizeMode = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Defines how the element's height is calculated.
    /// </summary>
    private SizeMode _vSizeMode = SizeMode.FIXED;
    public SizeMode VerticalSizeMode
    {
        get => _vSizeMode;
        set
        {
            _vSizeMode = value;
            Invalidate();
        }
    }

    /// <summary>
    /// The size of the element relative to its parent.
    /// </summary>
    private Vector2 _relSize = Vector2.Zero;
    public Vector2 RelativeSize
    {
        get => _relSize;
        set
        {
            _relSize = value;
            Invalidate();
        }
    }

    /// <summary>
    /// The width of the element relative to its parent.
    /// </summary>
    public float RelativeWidth
    {
        get => _relSize.X;
        set
        {
            RelativeSize = new(value, RelativeSize.Y);
        }
    }

    /// <summary>
    /// The height of the element relative to its parent.
    /// </summary>
    public float RelativeHeight
    {
        get => _relSize.Y;
        set
        {
            RelativeSize = new(RelativeSize.X, value);
        }
    }

    /// <summary>
    /// Calculates the width of the element based on SizeMode.
    /// </summary>
    public virtual int CalculateWidth()
    {
        if (!IsValid) return 0;

        switch (HorizontalSizeMode)
        {
            case SizeMode.RELATIVE:
                if (Parent == null)
                    throw new InvalidOperationException("Parent cannot be null when using SizeMode.RELATIVE.");

                return Parent.Width + (int)RelativeSize.X;

            case SizeMode.PERCENT:
                if (Parent == null)
                    throw new InvalidOperationException("Parent cannot be null when using SizeMode.PERCENT.");

                return (int)(Parent.Width * RelativeSize.X);

            case SizeMode.FILL_PARENT:
                if (Parent == null)
                    throw new InvalidOperationException("Parent cannot be null when using SizeMode.FILL_PARENT.");

                return Parent.Width;

            case SizeMode.ASPECT_RATIO:
                return RelativeSize.Y == 0.0f ?
                    (int)(CalculateHeight() * RelativeSize.X) :
                    (int)(CalculateHeight() * RelativeSize.X / RelativeSize.Y);

            default: // SizeMode = FIXED
                return Width;
        }
    }

    /// <summary>
    /// Calculates the height of the element based on SizeMode.
    /// </summary>
    /// <returns>The height of the element.</returns>
    public virtual int CalculateHeight()
    {
        if (!IsValid) return 0;

        switch (VerticalSizeMode)
        {
            case SizeMode.RELATIVE:
                if (Parent == null)
                    throw new InvalidOperationException("Parent cannot be null when using SizeMode.RELATIVE.");

                return Parent.Height + (int)RelativeSize.Y;

            case SizeMode.PERCENT:
                if (Parent == null)
                    throw new InvalidOperationException("Parent cannot be null when using SizeMode.PERCENT.");

                return (int)(Parent.Height * RelativeSize.Y);

            case SizeMode.FILL_PARENT:
                if (Parent == null)
                    throw new InvalidOperationException("Parent cannot be null when using SizeMode.FILL_PARENT.");

                return Parent.Height;

            case SizeMode.ASPECT_RATIO:
                return RelativeSize.X == 0.0f ?
                    (int)(CalculateWidth() * RelativeSize.Y) :
                    (int)(CalculateWidth() * RelativeSize.Y / RelativeSize.X);

            default: // SizeMode = FIXED
                return Height;
        }
    }

    public virtual void Resize()
    {
        int w = CalculateWidth();
        int h = CalculateHeight();
        ScreenRect = new(ScreenRect.Location, new(w, h));
    }

    #endregion

    #region Position

    /// <summary>
    /// Used to calculate the position of the element relative to its parent.
    /// Values greater than 1 or less than 0 are rare, but allowed
    /// and will position the element outside its parent.
    /// </summary>
    private Vector2 _anchor = Vector2.Zero;
    public Vector2 Anchor
    {
        get => _anchor;
        set
        {
            _anchor = value;
            Invalidate();
        }
    }

    /// <summary>
    /// The alignment of the element relative to the anchor point.
    /// Values greater than 1 or less than 0 are rare, but allowed
    /// and will create a gap between the element and its anchor point
    /// that will vary based on the size of the element.
    /// </summary>
    private Vector2 _align = Vector2.Zero;
    public Vector2 Align
    {
        get => _align;
        set
        {
            _align = value;
            Invalidate();
        }
    }

    /// <summary>
    /// The offset of the element from its calculated position.
    /// </summary>
    private Vector2 _offset = Vector2.Zero;
    public Vector2 Offset
    {
        get => _offset;
        set
        {
            _offset = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Updates the element's ScreenRect based on its parent's size and position.
    /// </summary>
    public virtual void Layout()
    {
        if (!IsValid) return;

        // If Parent is null, lay out relative to the UI viewport
        if (Parent == null)
        {
            int x = (int)(UI.Viewport.Bounds.Size.X * Anchor.X - Width * Align.X + Offset.X);
            int y = (int)(UI.Viewport.Bounds.Size.Y * Anchor.Y - Height * Align.Y + Offset.Y);

            ScreenRect = new(x, y, Width, Height);
        }
        else
        {
            int x = (int)(Parent.ScreenRect.Left + Parent.Width * Anchor.X - Width * Align.X + Offset.X);
            int y = (int)(Parent.ScreenRect.Top + Parent.Height * Anchor.Y - Height * Align.Y + Offset.Y);

            ScreenRect = new(x, y, Width, Height);
        }
    }

    #endregion

    #region Appearance

    public ElementStyle? StyleOverride { get; set; }
    protected ElementStyle GetElementStyle()
    {
        if (StyleOverride != null)
            return StyleOverride;

        if (Parent != null)
            return Parent.Style;

        return UI.Theme.DefaultElementStyle;
    }

    public virtual ElementStyle Style
    {
        get => GetElementStyle();
        set
        {
            StyleOverride = value;
            Invalidate();
        }
    }

    /// <summary>
    /// The background color of the element.
    /// </summary>
    private Color? _bgColor = null;
    public Color BackgroundColor
    {
        get => GetBackgroundColor();
        set
        {
            _bgColor = value;
            Invalidate();
        }
    }

    public virtual Color GetBackgroundColor()
    {
        if (_bgColor.HasValue)
            return _bgColor.Value;

        if (Style != null)
            return Style.BackgroundColor;

        return Color.Transparent;
    }

    private Color? _fgColor = null;
    public Color ForegroundColor
    {
        get => GetForegroundColor();
        set
        {
            _fgColor = value;
            Invalidate();
        }
    }

    public virtual Color GetForegroundColor()
    {
        if (_fgColor.HasValue)
            return _fgColor.Value;

        if (Style != null)
            return Style.ForegroundColor;

        return Color.Transparent;
    }

    private BorderStyle _borderStyle = Borders.Default;
    public BorderStyle Border
    {
        get
        {
            if (_borderStyle != null)
                return _borderStyle;

            if (Style != null)
                return Style.BorderStyle;

            return UI.Theme.DefaultElementStyle.BorderStyle;
        }
        set
        {
            _borderStyle = value;
            Invalidate();
        }
    }

    #endregion

    #region Behavior

    /// <summary>
    /// Checks if the element is valid and activates the control, allowing it to be drawn.
    /// Should be called before the first Update and Draw calls.
    /// </summary>
    /// <returns>True if the element is successfully activated, False otherwise.</returns>
    public virtual bool Activate()
    {
        if (Validate())
        {
            LoadContent();
            Resize();
            Layout();
            Render();
            Enabled = true;
        }
        else
        {
            Enabled = false;
        }

        return Enabled;
    }

    /// <summary>
    /// Updates the element and its children. Called every frame.
    /// </summary>
    /// <param name="gameTime">Snapshot of the game's timing state.</param>
    public virtual void Update(float dt)
    {
        if (!IsValid && Validate()) Render();
    }

    /// <summary>
    /// Renders and pre-caches textures used by the Draw method.
    /// Called during Activate() if successfully validated.
    /// </summary>
    public abstract void Render();

    protected virtual void DrawBackground(SpriteBatch batch)
    {
        Texture2D texel = new(UI.GraphicsDevice, 1, 1);
        texel.SetData([Color.White]);

        batch.Draw(texel, ScreenRect, BackgroundColor);
    }

    protected abstract void DrawForeground(SpriteBatch batch);

    protected virtual void DrawBorders(SpriteBatch batch)
    {
        if (Border == null) return;

        Texture2D texel = new(batch.GraphicsDevice, 1, 1);
        texel.SetData([Color.White]);

        if (Border.Thickness[0] > 0)
        {
            Rectangle top = new(ScreenRect.X, ScreenRect.Y, ScreenRect.Width, Border.Thickness[0]);
            batch.Draw(texel, top, Border.Color);
        }

        if (Border.Thickness[1] > 0)
        {
            Rectangle right = new(ScreenRect.X + ScreenRect.Width - Border.Thickness[1], ScreenRect.Y, Border.Thickness[1], ScreenRect.Height);
            batch.Draw(texel, right, Border.Color);
        }

        if (Border.Thickness[2] > 0)
        {
            Rectangle bottom = new(ScreenRect.X, ScreenRect.Y + ScreenRect.Height - Border.Thickness[2], ScreenRect.Width, Border.Thickness[2]);
            batch.Draw(texel, bottom, Border.Color);
        }

        if (Border.Thickness[3] > 0)
        {
            Rectangle left = new(ScreenRect.X, ScreenRect.Y, Border.Thickness[3], ScreenRect.Height);
            batch.Draw(texel, left, Border.Color);
        }
    }

    /// <summary>
    /// Draw the element and its children to the screen.
    /// Called once per frame. For improved performance,
    /// use the Render method to render and pre-cache textures.
    /// </summary>
    public virtual void Draw(SpriteBatch batch)
    {
        if (!IsValid) return;
        DrawBackground(batch);
        DrawForeground(batch);
        DrawBorders(batch);
    }

    #endregion
}
