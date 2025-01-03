using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

public class TextButton(UserInterface ui) : Button(ui)
{
    private string text = "TextLabel";
    public string Text
    {
        get => text;
        set
        {
            text = value;
            Invalidate();
        }
    }

    private string fontName = "LargeFont";
    public string FontName
    {
        get => fontName;
        set
        {
            fontName = value;
            Invalidate();
        }
    }

    private SpriteFont? font = null;
    public SpriteFont? Font
    {
        get => font;
        set
        {
            font = value;
            Invalidate();
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();
        font = UI.Content.Load<SpriteFont>(fontName);
    }

    /// <summary>
    /// The anchor point used to position the text within the button.
    /// </summary>
    public Vector2 TextAnchor { get; set; } = AnchorPresets.MiddleCenter;

    /// <summary>
    /// The alignment of the text relative to the anchor point.
    /// </summary>
    public Vector2 TextAlignment { get; set; } = AlignmentPresets.MiddleCenter;

    /// <summary>
    /// The offset of the text from its calculated position.
    /// </summary>
    public Vector2 TextOffset { get; set; }

    protected override Texture2D Render(ControlState state)
    {
        if (font == null)
        {
            throw new InvalidOperationException("Font must be set to a valid SpriteFont.");
        }

        Point textSize = font.MeasureString(Text).ToPoint();
        int x = (int)(TextAnchor.X * Width) - (int)(TextAlignment.X * textSize.X) + (int)TextOffset.X;
        int y = (int)(TextAnchor.Y * Height) - (int)(TextAlignment.Y * textSize.Y) + (int)TextOffset.Y;
        Vector2 pos = new(x, y);

        RenderTarget2D target = new(UI.GraphicsDevice, Width, Height);

        UI.GraphicsDevice.SetRenderTarget(target);
        UI.GraphicsDevice.Clear(Color.Transparent);

        SpriteBatch batch = new(UI.GraphicsDevice);

        batch.Begin();
        batch.DrawString(font, Text, Vector2.Zero, Color.White);
        batch.End();

        UI.GraphicsDevice.SetRenderTarget(null);

        return target;
    }
}