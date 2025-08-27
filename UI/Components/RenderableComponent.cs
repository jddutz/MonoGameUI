using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Core;

namespace MonoGameUI.Components;

/// <summary>
/// Defines what type of content this component renders.
/// </summary>
public enum RenderType
{
    /// <summary>
    /// No rendering.
    /// </summary>
    None,

    /// <summary>
    /// Render a sprite/texture.
    /// </summary>
    Sprite,

    /// <summary>
    /// Render text using a font.
    /// </summary>
    Text,

    /// <summary>
    /// Render a solid color rectangle.
    /// </summary>
    SolidColor,

    /// <summary>
    /// Render a 9-slice sprite for UI panels.
    /// </summary>
    NineSlice
}

/// <summary>
/// Component responsible for rendering visual content (sprites, text, shapes).
/// This replaces the rendering functionality from the legacy Element system.
/// </summary>
public class RenderableComponent : Core.Component
{
    private RenderType _renderType = RenderType.None;
    private Texture2D? _texture;
    private SpriteFont? _font;
    private string _text = string.Empty;
    private Color _color = Color.White;
    private SpriteEffects _effects = SpriteEffects.None;
    private Rectangle? _sourceRectangle;
    private Vector2 _origin = Vector2.Zero;
    private float _layerDepth = 0f;

    /// <summary>
    /// The type of content to render.
    /// </summary>
    public RenderType RenderType
    {
        get => _renderType;
        set
        {
            if (_renderType != value)
            {
                _renderType = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Texture to render when RenderType is Sprite or NineSlice.
    /// </summary>
    public Texture2D? Texture
    {
        get => _texture;
        set
        {
            if (_texture != value)
            {
                _texture = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Font to use when RenderType is Text.
    /// </summary>
    public SpriteFont? Font
    {
        get => _font;
        set
        {
            if (_font != value)
            {
                _font = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Text to render when RenderType is Text.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value ?? string.Empty;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Color tint applied to the rendered content.
    /// </summary>
    public Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Sprite effects (flip horizontally/vertically).
    /// </summary>
    public SpriteEffects Effects
    {
        get => _effects;
        set
        {
            if (_effects != value)
            {
                _effects = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Source rectangle for sprite rendering (null = entire texture).
    /// </summary>
    public Rectangle? SourceRectangle
    {
        get => _sourceRectangle;
        set
        {
            if (_sourceRectangle != value)
            {
                _sourceRectangle = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Origin point for rotation and scaling.
    /// </summary>
    public Vector2 Origin
    {
        get => _origin;
        set
        {
            if (_origin != value)
            {
                _origin = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Layer depth for depth sorting (0 = front, 1 = back).
    /// </summary>
    public float LayerDepth
    {
        get => _layerDepth;
        set
        {
            if (_layerDepth != value)
            {
                _layerDepth = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Get the content bounds for text rendering.
    /// Returns the size that the text would occupy.
    /// </summary>
    public Vector2 GetTextSize()
    {
        if (RenderType != RenderType.Text || Font == null || string.IsNullOrEmpty(Text))
            return Vector2.Zero;

        return Font.MeasureString(Text);
    }

    /// <summary>
    /// Get the natural size of the content.
    /// For sprites, returns texture size. For text, returns measured text size.
    /// </summary>
    public Vector2 GetContentSize()
    {
        return RenderType switch
        {
            RenderType.Sprite when Texture != null => new Vector2(Texture.Width, Texture.Height),
            RenderType.NineSlice when Texture != null => new Vector2(Texture.Width, Texture.Height),
            RenderType.Text => GetTextSize(),
            _ => Vector2.Zero
        };
    }

    /// <summary>
    /// Set up for sprite rendering.
    /// </summary>
    public void SetSprite(Texture2D texture, Color? color = null, Rectangle? sourceRectangle = null)
    {
        RenderType = RenderType.Sprite;
        Texture = texture;
        Color = color ?? Color.White;
        SourceRectangle = sourceRectangle;
    }

    /// <summary>
    /// Set up for text rendering.
    /// </summary>
    public void SetText(string text, SpriteFont font, Color? color = null)
    {
        RenderType = RenderType.Text;
        Text = text;
        Font = font;
        Color = color ?? Color.White;
    }

    /// <summary>
    /// Set up for solid color rendering.
    /// </summary>
    public void SetSolidColor(Color color)
    {
        RenderType = RenderType.SolidColor;
        Color = color;
    }

    /// <summary>
    /// Render the component content using the provided SpriteBatch.
    /// This method should be called by the RenderSystem.
    /// </summary>
    public void Render(SpriteBatch spriteBatch, TransformComponent transform)
    {
        if (!Enabled || RenderType == RenderType.None)
            return;

        var worldMatrix = transform.GetWorldTransformMatrix();
        var position = Vector2.Transform(Vector2.Zero, worldMatrix);
        var size = transform.Size;

        switch (RenderType)
        {
            case RenderType.Sprite:
                RenderSprite(spriteBatch, position, size, worldMatrix);
                break;

            case RenderType.Text:
                RenderText(spriteBatch, position, size);
                break;

            case RenderType.SolidColor:
                RenderSolidColor(spriteBatch, position, size);
                break;

            case RenderType.NineSlice:
                RenderNineSlice(spriteBatch, position, size);
                break;
        }
    }

    private void RenderSprite(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Matrix transform)
    {
        if (Texture == null) return;

        var destinationRect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

        spriteBatch.Draw(
            texture: Texture,
            destinationRectangle: destinationRect,
            sourceRectangle: SourceRectangle,
            color: Color,
            rotation: 0f, // Rotation handled by transform matrix
            origin: Origin,
            effects: Effects,
            layerDepth: LayerDepth
        );
    }

    private void RenderText(SpriteBatch spriteBatch, Vector2 position, Vector2 size)
    {
        if (Font == null || string.IsNullOrEmpty(Text)) return;

        spriteBatch.DrawString(
            spriteFont: Font,
            text: Text,
            position: position,
            color: Color,
            rotation: 0f, // Rotation handled by transform matrix
            origin: Origin,
            scale: Vector2.One, // Scale handled by transform matrix  
            effects: Effects,
            layerDepth: LayerDepth
        );
    }

    private void RenderSolidColor(SpriteBatch spriteBatch, Vector2 position, Vector2 size)
    {
        // Create or use a 1x1 white pixel texture for solid color rendering
        var pixelTexture = CreatePixelTexture(spriteBatch.GraphicsDevice);
        var destinationRect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

        spriteBatch.Draw(
            texture: pixelTexture,
            destinationRectangle: destinationRect,
            color: Color
        );
    }

    private void RenderNineSlice(SpriteBatch spriteBatch, Vector2 position, Vector2 size)
    {
        if (Texture == null) return;

        // TODO: Implement 9-slice rendering
        // For now, fall back to regular sprite rendering
        RenderSprite(spriteBatch, position, size, Matrix.Identity);
    }

    private static readonly Dictionary<GraphicsDevice, Texture2D> _pixelTextures = new();

    private static Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        if (!_pixelTextures.TryGetValue(graphicsDevice, out var texture))
        {
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            _pixelTextures[graphicsDevice] = texture;
        }
        return texture;
    }
}
