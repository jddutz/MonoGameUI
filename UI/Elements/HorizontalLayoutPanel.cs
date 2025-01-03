namespace MonoGameUI.Elements;

public class HorizontalLayoutPanel(UserInterface ui) : Container(ui)
{
    public LayoutMode LayoutMode { get; set; } = LayoutMode.CENTER;

    public int Spacing { get; set; } = 20;

    /// <summary>
    /// Elements the direction in which the children are laid out.
    /// </summary>
    public bool ReverseDirection { get; set; } = false;

    private void LayoutChildrenStart()
    {
        if (Children.Count == 0) return;

        int w = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                w += child.Width;
                w += Spacing;
            }
        }

        int x = ReverseDirection ? w : Spacing;
        foreach (Element child in Children)
        {
            child.Anchor = new(0.0f, child.Anchor.Y);
            child.Align = new(ReverseDirection ? 1.0f : 0.0f, child.Align.Y);
            child.Offset = new(x, child.Offset.Y);
            x += ReverseDirection ? -child.Width - Spacing : child.Width + Spacing;
        }
    }

    private void LayoutChildrenEnd()
    {
        if (Children.Count == 0) return;

        int w = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                w += child.Width;
                w += Spacing;
            }
        }

        int x = ReverseDirection ? -Spacing : Width - w;
        foreach (Element child in Children)
        {
            child.Anchor = new(ReverseDirection ? 1.0f : 0.0f, child.Anchor.Y);
            child.Align = new(ReverseDirection ? 1.0f : 0.0f, child.Align.Y);
            child.Offset = new(x, child.Offset.Y);
            x += ReverseDirection ? -child.Width - Spacing : child.Width + Spacing;
        }
    }

    private void LayoutChildrenCenter()
    {
        if (Children.Count == 0) return;

        int w = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                w += child.Width;
                w += Spacing;
            }
        }

        int x = ReverseDirection ? w / 2 : -w / 2;
        foreach (Element child in Children)
        {
            child.Anchor = new(0.5f, child.Anchor.Y);
            child.Align = new(ReverseDirection ? 1.0f : 0.0f, child.Align.Y);
            child.Offset = new(x, child.Offset.Y);
            x += ReverseDirection ? -child.Width - Spacing : child.Width + Spacing;
        }
    }

    private void LayoutChildrenEqual()
    {
        if (Children.Count == 0) return;

        int w = 0;
        foreach (Element child in Children)
        {
            if (child.IsValid)
            {
                child.Resize();
                w += child.Width;
            }
        }

        int n = Children.Count;
        int gap = Width > w ? (Width - w) / (n + 1) : (Width - w) / (n - 1);
        int x = 0;

        if (ReverseDirection)
        {
            if (Width > w)
            {
                x = Width - gap;
            }
        }
        else
        {
            if (Width > w)
            {
                x = gap;
            }
        }

        foreach (Element child in Children)
        {
            child.Anchor = new(ReverseDirection ? 1.0f : 0.0f, child.Anchor.Y);
            child.Align = new(ReverseDirection ? 1.0f : 0.0f, child.Align.Y);
            child.Offset = new(x, child.Offset.Y);
            x += ReverseDirection ? -child.Width - gap : child.Width + gap;
        }
    }

    public override void Layout()
    {
        base.Layout();
        if (Children.Count == 0) return;

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
