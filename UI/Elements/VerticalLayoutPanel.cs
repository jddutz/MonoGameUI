using System.Linq;

namespace MonoGameUI.Elements;

public class VerticalLayoutPanel(UserInterface ui) : Container(ui)
{
    public LayoutMode LayoutMode { get; set; } = LayoutMode.CENTER;

    public int Spacing { get; set; } = 20;

    /// <summary>
    /// Elements the direction in which the children are laid out.
    /// </summary>
    public bool ReverseDirection { get; set; } = false;

    private void LayoutChildrenStart()
    {
        int h = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                h += child.Height;
                h += Spacing;
            }
        }

        int y = ReverseDirection ? h : Spacing;
        foreach (Element child in Children)
        {
            child.Anchor = new(child.Anchor.X, 0.0f);
            child.Align = new(child.Align.X, ReverseDirection ? 1.0f : 0.0f);
            child.Offset = new(child.Offset.X, y);
            y += ReverseDirection ? -child.Height - Spacing : child.Height + Spacing;
        }
    }

    private void LayoutChildrenEnd()
    {
        int h = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                h += child.Height;
                h += Spacing;
            }
        }

        int y = ReverseDirection ? -Spacing : Height - h;
        foreach (Element child in Children)
        {
            child.Anchor = new(child.Anchor.X, ReverseDirection ? 1.0f : 0.0f);
            child.Align = new(child.Align.X, ReverseDirection ? 1.0f : 0.0f);
            child.Offset = new(child.Offset.X, y);
            y += ReverseDirection ? -child.Height - Spacing : child.Height + Spacing;
        }
    }

    private void LayoutChildrenCenter()
    {
        int h = 0;
        foreach (Element child in Children)
        {
            child.Resize();
            h += child.Height;
            h += Spacing;
        }

        int y = ReverseDirection ? h / 2 : -h / 2;
        foreach (Element child in Children)
        {
            child.Anchor = new(child.Anchor.X, 0.5f);
            child.Align = new(child.Align.X, ReverseDirection ? 1.0f : 0.0f);
            child.Offset = new(child.Offset.X, y);
            y += ReverseDirection ? -child.Height - Spacing : child.Height + Spacing;
        }
    }

    private void LayoutChildrenEqual()
    {
        int h = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                h += child.Height;
                h += Spacing;
            }
        }

        int n = Children.Count;
        int gap = Height > h ? (Height - h) / (n + 1) : (Height - h) / (n - 1);
        int y = 0;

        if (ReverseDirection)
        {
            if (Height > h)
            {
                y = Height - gap;
            }
        }
        else
        {
            if (Height > h)
            {
                y = gap;
            }
        }

        foreach (Element child in Children)
        {
            child.Anchor = new(child.Anchor.X, ReverseDirection ? 1.0f : 0.0f);
            child.Align = new(child.Align.X, ReverseDirection ? 1.0f : 0.0f);
            child.Offset = new(child.Offset.X, y);
            y += ReverseDirection ? -child.Height - gap : child.Height + gap;
        }
    }

    public override void LayoutChildren()
    {
        if (!Validate()) return;

        switch (LayoutMode)
        {
            case LayoutMode.START:
                LayoutChildrenStart();
                break;
            case LayoutMode.END:
                LayoutChildrenEnd();
                break;
            case LayoutMode.CENTER:
                LayoutChildrenCenter();
                break;
            default:
                LayoutChildrenEqual();
                break;
        }
    }
}
