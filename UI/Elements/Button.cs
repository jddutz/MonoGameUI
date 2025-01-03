using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

/// <summary>
/// A UI control that responds to mouse input and touch events.
/// </summary>
public abstract class Button(UserInterface ui) : Control(ui)
{
    public Dictionary<ControlState, ElementStyle> ButtonStyles { get; set; } = [];

    public ElementStyle GetButtonStyle() => GetButtonStyle(CurrentState);

    public ElementStyle GetButtonStyle(ControlState state)
    {
        if (ButtonStyles.TryGetValue(state, out ElementStyle? style))
        {
            return style;
        }

        if (ButtonStyles.TryGetValue(ControlState.NORMAL, out ElementStyle? defaultStyle))
        {
            return defaultStyle;
        }

        ButtonStyles[state] = new ElementStyle();

        return ButtonStyles[state];
    }

    public override Color GetBackgroundColor() => GetButtonStyle().BackgroundColor;
    public override Color GetForegroundColor() => GetButtonStyle().ForegroundColor;

    public override void LoadContent()
    {
        // TODO: Button styles should be loaded from UI.Theme

        ButtonStyles.Clear();
        ButtonStyles[ControlState.NORMAL] = new ElementStyle()
        {
            BackgroundColor = Color.Black,
            ForegroundColor = Color.White,
            BorderStyle = new BorderStyle()
            {
                Color = Color.White,
                Thickness = [2, 2, 2, 2]
            }
        };

        ButtonStyles[ControlState.HOVER] = new ElementStyle()
        {
            BackgroundColor = new Color(15, 15, 15, 255),
            ForegroundColor = Color.White,
            BorderStyle = new BorderStyle()
            {
                Color = Color.White,
                Thickness = [2, 2, 2, 2]
            }
        };

        ButtonStyles[ControlState.SELECTED] = new ElementStyle()
        {
            BackgroundColor = new Color(100, 100, 100, 255),
            ForegroundColor = new Color(15, 15, 15, 255),
            BorderStyle = new BorderStyle()
            {
                Color = Color.DarkGray,
                Thickness = [2, 2, 2, 2]
            }
        };

        ButtonStyles[ControlState.INACTIVE] = new ElementStyle()
        {
            BackgroundColor = Color.Black,
            ForegroundColor = new Color(15, 15, 15, 255),
            BorderStyle = new BorderStyle()
            {
                Color = Color.DarkGray,
                Thickness = [2, 2, 2, 2]
            }
        };
    }

    public event EventHandler? OnClick;

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (CurrentState == ControlState.INACTIVE) return;
        CurrentState = ControlState.HOVER;
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        if (CurrentState == ControlState.INACTIVE) return;
        CurrentState = ControlState.NORMAL;
    }

    public override void OnMouseButtonDown()
    {
        base.OnMouseButtonDown();
        if (CurrentState == ControlState.INACTIVE) return;
        CurrentState = ControlState.SELECTED;
    }

    public override void OnMouseButtonUp()
    {
        base.OnMouseButtonUp();
        if (CurrentState == ControlState.INACTIVE) return;
        CurrentState = ControlState.HOVER;
    }

    public override void OnMouseClick()
    {
        OnClick?.Invoke(this, EventArgs.Empty);
    }

    protected Dictionary<ControlState, Texture2D> textures = [];

    protected abstract Texture2D Render(ControlState state);

    public override void Render()
    {
        textures.Clear();
        foreach (ControlState state in Enum.GetValues<ControlState>())
        {
            textures[state] = Render(state);
        }
    }

    protected override void DrawForeground(SpriteBatch batch)
    {
        if (textures.TryGetValue(CurrentState, out Texture2D? texture))
        {
            batch.Draw(texture, ScreenRect, GetForegroundColor());
        }
    }

    protected override void DrawBorders(SpriteBatch batch)
    {
        base.DrawBorders(batch);
    }
}
