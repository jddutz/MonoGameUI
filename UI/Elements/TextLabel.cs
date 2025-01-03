using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

public class TextLabel(UserInterface ui) : Element(ui)
{
    private string text = "TextLabel";
    public string Text
    {
        get => text;
        set
        {
            text = value;
            Invalidate();
            if (Enabled) Resize();
        }
    }

    private string fontName = "DefaultFont";
    public string FontName
    {
        get => fontName;
        set
        {
            fontName = value;
            Invalidate();
            if (Enabled) Resize();
        }
    }

    private SpriteFont? font = null;
    public SpriteFont Font
    {
        get
        {
            if (font == null)
            {
                if (UI?.Content == null)
                {
                    throw new InvalidOperationException("ContentManager is not set.");
                }

                font = UI.Content.Load<SpriteFont>(fontName);
            }

            return font;
        }
        set
        {
            font = value;
            Invalidate();
            if (Enabled) Resize();
        }
    }

    public override void LoadContent()
    {
        if (UI?.Content == null) return;

        font = UI.Content.Load<SpriteFont>(fontName);
        IsLoaded = true;
    }

    public override void Resize()
    {
        if (!IsLoaded) return;
        Size = Font.MeasureString(Text).ToPoint();
    }

    private Texture2D? texture;
    public override void Render()
    {
        RenderTarget2D target = new(UI.GraphicsDevice, Width, Height);

        UI.GraphicsDevice.SetRenderTarget(target);
        UI.GraphicsDevice.Clear(Color.Transparent);

        SpriteBatch batch = new(UI.GraphicsDevice);

        batch.Begin();
        batch.DrawString(font, Text, Vector2.Zero, Color.White);
        batch.End();

        UI.GraphicsDevice.SetRenderTarget(null);

        texture = target;
    }

    protected override void DrawForeground(SpriteBatch batch)
    {
        batch.Draw(texture, ScreenRect, Color.White);
    }
}
