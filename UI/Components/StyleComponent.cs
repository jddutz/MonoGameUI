using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Core;

namespace MonoGameUI.Components;

/// <summary>
/// Represents spacing values for margins and padding.
/// </summary>
public struct Thickness
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public Thickness(float uniformThickness)
    {
        Left = Top = Right = Bottom = uniformThickness;
    }

    public Thickness(float horizontal, float vertical)
    {
        Left = Right = horizontal;
        Top = Bottom = vertical;
    }

    public Thickness(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public readonly float TotalWidth => Left + Right;
    public readonly float TotalHeight => Top + Bottom;

    public static readonly Thickness Zero = new(0);

    public static implicit operator Thickness(float uniform) => new(uniform);
}

/// <summary>
/// Component responsible for visual styling including backgrounds, borders, margins, and padding.
/// This replaces styling functionality from the legacy Element system.
/// </summary>
public class StyleComponent : Core.Component
{
    private Color _backgroundColor = Color.Transparent;
    private Color _borderColor = Color.Black;
    private Thickness _borderThickness = Thickness.Zero;
    private Thickness _margin = Thickness.Zero;
    private Thickness _padding = Thickness.Zero;
    private float _opacity = 1.0f;
    private bool _visible = true;

    /// <summary>
    /// Background color for the element.
    /// </summary>
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (_backgroundColor != value)
            {
                _backgroundColor = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Border color for the element.
    /// </summary>
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            if (_borderColor != value)
            {
                _borderColor = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Border thickness on each side.
    /// </summary>
    public Thickness BorderThickness
    {
        get => _borderThickness;
        set
        {
            if (_borderThickness.Left != value.Left || _borderThickness.Top != value.Top ||
                _borderThickness.Right != value.Right || _borderThickness.Bottom != value.Bottom)
            {
                _borderThickness = value;
                MarkDirty(DirtyFlags.Render | DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Margin (space outside the element).
    /// </summary>
    public Thickness Margin
    {
        get => _margin;
        set
        {
            if (_margin.Left != value.Left || _margin.Top != value.Top ||
                _margin.Right != value.Right || _margin.Bottom != value.Bottom)
            {
                _margin = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Padding (space inside the element).
    /// </summary>
    public Thickness Padding
    {
        get => _padding;
        set
        {
            if (_padding.Left != value.Left || _padding.Top != value.Top ||
                _padding.Right != value.Right || _padding.Bottom != value.Bottom)
            {
                _padding = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Opacity level (0.0 = transparent, 1.0 = opaque).
    /// </summary>
    public float Opacity
    {
        get => _opacity;
        set
        {
            value = Math.Clamp(value, 0f, 1f);
            if (_opacity != value)
            {
                _opacity = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Whether the element is visible.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible != value)
            {
                _visible = value;
                MarkDirty(DirtyFlags.Render);
            }
        }
    }

    /// <summary>
    /// Get the effective color with opacity applied.
    /// </summary>
    public Color GetEffectiveBackgroundColor()
    {
        return Color.FromNonPremultiplied(
            BackgroundColor.R,
            BackgroundColor.G,
            BackgroundColor.B,
            (int)(BackgroundColor.A * Opacity)
        );
    }

    /// <summary>
    /// Get the effective border color with opacity applied.
    /// </summary>
    public Color GetEffectiveBorderColor()
    {
        return Color.FromNonPremultiplied(
            BorderColor.R,
            BorderColor.G,
            BorderColor.B,
            (int)(BorderColor.A * Opacity)
        );
    }

    /// <summary>
    /// Get the content area after accounting for padding and borders.
    /// </summary>
    public Rectangle GetContentBounds(Rectangle elementBounds)
    {
        var x = elementBounds.X + (int)(BorderThickness.Left + Padding.Left);
        var y = elementBounds.Y + (int)(BorderThickness.Top + Padding.Top);
        var width = elementBounds.Width - (int)(BorderThickness.TotalWidth + Padding.TotalWidth);
        var height = elementBounds.Height - (int)(BorderThickness.TotalHeight + Padding.TotalHeight);

        return new Rectangle(x, y, Math.Max(0, width), Math.Max(0, height));
    }

    /// <summary>
    /// Get the outer bounds including margins.
    /// </summary>
    public Rectangle GetOuterBounds(Rectangle elementBounds)
    {
        var x = elementBounds.X - (int)Margin.Left;
        var y = elementBounds.Y - (int)Margin.Top;
        var width = elementBounds.Width + (int)Margin.TotalWidth;
        var height = elementBounds.Height + (int)Margin.TotalHeight;

        return new Rectangle(x, y, width, height);
    }

    /// <summary>
    /// Render the background and borders for this style.
    /// This should be called before rendering content.
    /// </summary>
    public void RenderBackground(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (!Enabled || !Visible || Opacity <= 0f)
            return;

        // Render background
        if (BackgroundColor.A > 0)
        {
            var bgColor = GetEffectiveBackgroundColor();
            var pixelTexture = GetPixelTexture(spriteBatch.GraphicsDevice);
            spriteBatch.Draw(pixelTexture, bounds, bgColor);
        }
    }

    /// <summary>
    /// Render the borders for this style.
    /// This should be called after rendering content.
    /// </summary>
    public void RenderBorders(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (!Enabled || !Visible || Opacity <= 0f)
            return;

        if (BorderColor.A > 0 && (BorderThickness.Left > 0 || BorderThickness.Top > 0 ||
                                  BorderThickness.Right > 0 || BorderThickness.Bottom > 0))
        {
            var borderColor = GetEffectiveBorderColor();
            var pixelTexture = GetPixelTexture(spriteBatch.GraphicsDevice);

            // Top border
            if (BorderThickness.Top > 0)
            {
                var topRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, (int)BorderThickness.Top);
                spriteBatch.Draw(pixelTexture, topRect, borderColor);
            }

            // Right border
            if (BorderThickness.Right > 0)
            {
                var rightRect = new Rectangle(
                    bounds.Right - (int)BorderThickness.Right,
                    bounds.Y,
                    (int)BorderThickness.Right,
                    bounds.Height
                );
                spriteBatch.Draw(pixelTexture, rightRect, borderColor);
            }

            // Bottom border
            if (BorderThickness.Bottom > 0)
            {
                var bottomRect = new Rectangle(
                    bounds.X,
                    bounds.Bottom - (int)BorderThickness.Bottom,
                    bounds.Width,
                    (int)BorderThickness.Bottom
                );
                spriteBatch.Draw(pixelTexture, bottomRect, borderColor);
            }

            // Left border
            if (BorderThickness.Left > 0)
            {
                var leftRect = new Rectangle(bounds.X, bounds.Y, (int)BorderThickness.Left, bounds.Height);
                spriteBatch.Draw(pixelTexture, leftRect, borderColor);
            }
        }
    }

    private static readonly Dictionary<GraphicsDevice, Texture2D> _pixelTextures = new();

    private static Texture2D GetPixelTexture(GraphicsDevice graphicsDevice)
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
