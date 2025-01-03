using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace MonoGameUI.Elements;

public class ImageRect(UserInterface ui) : Element(ui)
{
    private Texture2D? image;
    protected Texture2D? texture;

    public string Path { get; set; } = "";

    public override void LoadContent()
    {
        if (Path != "")
        {
            image = UI.Content.Load<Texture2D>(Path);
        }

        base.LoadContent();
    }

    public ImageSizeMode ImageSizeMode { get; set; } = ImageSizeMode.STRETCH;

    public bool FlipHorizontal { get; set; } = false;
    public bool FlipVertical { get; set; } = false;
    public int RotateCCW { get; set; } = 0;

    /// <summary>
    /// Renders the image onto the background texture.
    /// </summary>
    /// <param name="batch">The SpriteBatch used to draw the element.</param>
    protected virtual Texture2D RenderImage()
    {
        RenderTarget2D target = new(UI.GraphicsDevice, Width, Height);

        UI.GraphicsDevice.SetRenderTarget(target);

        if (image == null)
        {
            UI.GraphicsDevice.Clear(Color.Magenta);
            return target;
        }

        UI.GraphicsDevice.Clear(Color.Transparent);

        SpriteBatch batch = new(UI.GraphicsDevice);

        batch.Begin();
        switch (ImageSizeMode)
        {
            case ImageSizeMode.STRETCH:
                batch.Draw(image, new Rectangle(0, 0, Width, Height), Color.White);
                break;
            case ImageSizeMode.BEST_FIT:
                float scaleX = (float)Width / image.Width;
                float scaleY = (float)Height / image.Height;
                float scale = Math.Min(scaleX, scaleY);
                int w = (int)(image.Width * scale);
                int h = (int)(image.Height * scale);
                int dx = (Width - w) / 2;
                int dy = (Height - h) / 2;
                batch.Draw(image, new Rectangle(dx, dy, w, h), Color.White);
                break;
            case ImageSizeMode.FIT_HORIZONTAL:
                scale = (float)Width / image.Width;
                w = (int)(image.Width * scale);
                h = (int)(image.Height * scale);
                dy = (Height - h) / 2;
                batch.Draw(image, new Rectangle(0, dy, Width, h), Color.White);
                break;
            case ImageSizeMode.FIT_VERTICAL:
                scale = (float)Height / image.Height;
                w = (int)(image.Width * scale);
                h = (int)(image.Height * scale);
                dx = (Width - w) / 2;
                batch.Draw(image, new Rectangle(dx, 0, w, Height), Color.White);
                break;
            case ImageSizeMode.TILE:
                for (int x = 0; x < Width; x += image.Width)
                {
                    for (int y = 0; y < Height; y += image.Height)
                    {
                        batch.Draw(image, new Rectangle(x, y, image.Width, image.Height), Color.White);
                    }
                }
                break;
            case ImageSizeMode.TILE_CENTER:
                int cols = 1 + 2 * (Width - image.Width) / image.Width;
                int rows = 1 + 2 * (Height - image.Height) / image.Height;
                w = image.Width * cols;
                h = image.Height * rows;
                dx = (Width - w) / 2;
                dy = (Height - h) / 2;
                for (int x = 0; x < w; x += image.Width)
                {
                    for (int y = 0; y < h; y += image.Height)
                    {
                        batch.Draw(image, new Rectangle(dx + x, dy + y, image.Width, image.Height), Color.White);
                    }
                }
                break;
            default: // ImageSizeMode.CENTER
                dx = (Width - image.Width) / 2;
                dy = (Height - image.Height) / 2;
                batch.Draw(image, new Rectangle(dx, dy, image.Width, image.Height), Color.White);
                break;
        }
        batch.End();

        UI.GraphicsDevice.SetRenderTarget(null);

        return target;
    }

    public override void Render()
    {
        texture = RenderImage();
    }

    protected override void DrawForeground(SpriteBatch batch)
    {
        batch.Draw(texture, ScreenRect, ForegroundColor);
    }
}