using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

public class DropdownMenu(UserInterface ui) : Element(ui)
{
    public VerticalLayoutPanel LayoutPanel { get; } = new(ui);
    public virtual void Add(DropdownMenuItem item)
    {
        item.Parent = LayoutPanel;
        LayoutPanel.Children.Add(item);
    }

    public virtual void AddRange(params DropdownMenuItem[] items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public virtual void Clear()
    {
        LayoutPanel.Children.Clear();
    }

    public override void Render()
    {
        throw new System.NotImplementedException();
    }

    protected override void DrawForeground(SpriteBatch batch)
    {
        throw new System.NotImplementedException();
    }
}