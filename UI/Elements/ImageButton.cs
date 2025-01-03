using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

public class ImageButton(UserInterface ui) : Button(ui)
{
    private Texture2D? icon;

    public string IconPath { get; set; } = "";

    public override void LoadContent()
    {
        base.LoadContent();
        if (string.IsNullOrEmpty(IconPath))
        {
            throw new InvalidOperationException("IconPath must be set to a valid path.");
        }

        icon = UI.Content.Load<Texture2D>(IconPath);

        if (Width == 0)
        {
            Width = icon.Width;
        }

        if (Height == 0)
        {
            Height = icon.Height;
        }
    }

    public ImageSizeMode ImageSizeMode { get; set; } = ImageSizeMode.STRETCH;

    public bool FlipHorizontal { get; set; } = false;
    public bool FlipVertical { get; set; } = false;
    public int RotateCCW { get; set; } = 0;

    /// <summary>
    /// Renders the icon onto the background texture.
    /// </summary>
    /// <param name="batch">The SpriteBatch used to draw the element.</param>
    protected override Texture2D Render(ControlState state)
    {
        RenderTarget2D target = new(UI.GraphicsDevice, Width, Height);

        UI.GraphicsDevice.SetRenderTarget(target);

        if (icon == null)
        {
            UI.GraphicsDevice.Clear(Color.Magenta);
            return target;
        }

        UI.GraphicsDevice.Clear(Color.Transparent);

        SpriteBatch batch = new(UI.GraphicsDevice);

        switch (ImageSizeMode)
        {
            case ImageSizeMode.STRETCH:
                batch.Draw(icon, new Rectangle(0, 0, Width, Height), Color.White);
                break;
            case ImageSizeMode.BEST_FIT:
                float scaleX = (float)Width / icon.Width;
                float scaleY = (float)Height / icon.Height;
                float scale = Math.Min(scaleX, scaleY);
                int w = (int)(icon.Width * scale);
                int h = (int)(icon.Height * scale);
                int dx = (Width - w) / 2;
                int dy = (Height - h) / 2;
                batch.Draw(icon, new Rectangle(dx, dy, w, h), Color.White);
                break;
            case ImageSizeMode.FIT_HORIZONTAL:
                scale = (float)Width / icon.Width;
                w = (int)(icon.Width * scale);
                h = (int)(icon.Height * scale);
                dy = (Height - h) / 2;
                batch.Draw(icon, new Rectangle(0, dy, Width, h), Color.White);
                break;
            case ImageSizeMode.FIT_VERTICAL:
                scale = (float)Height / icon.Height;
                w = (int)(icon.Width * scale);
                h = (int)(icon.Height * scale);
                dx = (Width - w) / 2;
                batch.Draw(icon, new Rectangle(dx, 0, w, Height), Color.White);
                break;
            case ImageSizeMode.TILE:
                for (int x = 0; x < Width; x += icon.Width)
                {
                    for (int y = 0; y < Height; y += icon.Height)
                    {
                        batch.Draw(icon, new Rectangle(x, y, icon.Width, icon.Height), Color.White);
                    }
                }
                break;
            case ImageSizeMode.TILE_CENTER:
                int cols = 1 + 2 * (Width - icon.Width) / icon.Width;
                int rows = 1 + 2 * (Height - icon.Height) / icon.Height;
                w = icon.Width * cols;
                h = icon.Height * rows;
                dx = (Width - w) / 2;
                dy = (Height - h) / 2;
                for (int x = 0; x < w; x += icon.Width)
                {
                    for (int y = 0; y < h; y += icon.Height)
                    {
                        batch.Draw(icon, new Rectangle(dx + x, dy + y, icon.Width, icon.Height), Color.White);
                    }
                }
                break;
            default: // ImageSizeMode.CENTER
                dx = (Width - icon.Width) / 2;
                dy = (Height - icon.Height) / 2;
                batch.Draw(icon, new Rectangle(dx, dy, icon.Width, icon.Height), Color.White);
                break;
        }

        UI.GraphicsDevice.SetRenderTarget(null);

        return target;
    }
}