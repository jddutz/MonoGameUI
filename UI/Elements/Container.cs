using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUI.Elements;

/// <summary>
/// Defines an element which is a collection of other elements.
/// </summary>
public class Container(UserInterface ui) : Element(ui), IEnumerable<Element>, ICollection<Element>, IMouseInputEventHandler
{
    public List<Element> Children { get; private set; } = [];

    public int Count => Children.Count;

    public override bool Validate()
    {
        if (Children.Count == 0)
        {
            IsValid = false;
            return false;
        }

        foreach (Element child in Children)
        {
            if (!child.Validate())
            {
                IsValid = false;
                return false;
            }
        }

        IsValid = true;
        return true;
    }

    public override void Resize()
    {
        base.Resize();
        ResizeChildren();
    }

    public virtual void ResizeChildren()
    {
        foreach (Element child in Children)
        {
            child.Resize();
        }
    }

    /// <summary>
    /// Lays out the children elements of the container, updating ScreenRect.
    /// This method should be overridden by child classes to provide custom layout logic.
    /// </summary>
    public override void Layout()
    {
        base.Layout();
        LayoutChildren();
    }

    public virtual void LayoutChildren()
    {
        foreach (Element child in Children)
        {
            child.Layout();
        }
    }

    /// <summary>
    /// Checks if the element is valid and activates the control, allowing it to be drawn.
    /// </summary>
    /// <returns>True if the element is successfully activated, False otherwise.</returns>
    public override bool Activate()
    {
        if (Validate())
        {
            LoadContent();
            Resize();
            Layout();
            Render();
            Enabled = ActivateChildren();
        }
        else
        {
            Enabled = false;
        }

        return Enabled;
    }

    public virtual bool ActivateChildren()
    {
        bool activated = true;
        foreach (Element child in Children)
        {
            if (!child.Activate()) activated = false;
        }

        return activated;
    }

    /// <summary>
    /// Calculates the width of the element based on SizeMode.
    /// </summary>
    public override int CalculateWidth()
    {
        if (!IsValid) return 0;

        int parentWidth = Parent?.Width ?? UI.Viewport.Bounds.Size.X;

        switch (HorizontalSizeMode)
        {
            case SizeMode.FIXED:
                return Width;

            case SizeMode.RELATIVE:
                return parentWidth + (int)RelativeSize.X;

            case SizeMode.PERCENT:
                return (int)(parentWidth * RelativeSize.X);

            case SizeMode.FILL_PARENT:
                return parentWidth;

            case SizeMode.ASPECT_RATIO:
                return RelativeSize.Y == 0.0f ?
                    (int)(CalculateHeight() * RelativeSize.X) :
                    (int)(CalculateHeight() * RelativeSize.X / RelativeSize.Y);

            case SizeMode.FIT_CONTENT:
                return GetContentWidth();

            default:
                return Width;
        }
    }

    /// <summary>
    /// Calculates the height of the element based on SizeMode.
    /// </summary>
    /// <returns>The height of the element.</returns>
    public override int CalculateHeight()
    {
        if (!IsValid) return 0;

        int parentHeight = Parent?.Height ?? UI.Viewport.Bounds.Size.Y;

        switch (VerticalSizeMode)
        {
            case SizeMode.FIXED:
                return Height;

            case SizeMode.RELATIVE:
                return parentHeight + (int)RelativeSize.Y;

            case SizeMode.PERCENT:
                return (int)(parentHeight * RelativeSize.Y);

            case SizeMode.FILL_PARENT:
                return parentHeight;

            case SizeMode.ASPECT_RATIO:
                return RelativeSize.X == 0.0f ?
                    (int)(CalculateWidth() * RelativeSize.Y) :
                    (int)(CalculateWidth() * RelativeSize.Y / RelativeSize.X);

            case SizeMode.FIT_CONTENT:
                return GetContentHeight();

            default:
                return Height;
        }
    }

    /// <summary>
    /// Calculates the total width of the element's children
    /// factoring in the overall extents of each child element.
    /// </summary>
    /// <returns>The total width of child elements.</returns>
    public virtual int GetContentWidth()
    {
        if (Children.Count == 0) return 0;

        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (Element e in Children)
        {
            Point p = GetLocalPosition(e);
            min = Math.Min(min, p.X);
            max = Math.Max(max, p.X + e.Width);
        }

        return max - min;
    }

    /// <summary>
    /// Calculates the total height of the element's children
    /// factoring in the overall extents of each child element.
    /// </summary>
    /// <returns>The total height of child elements.</returns>
    public virtual int GetContentHeight()
    {
        if (Children.Count == 0) return 0;

        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (Element e in Children)
        {
            Point p = GetLocalPosition(e);
            min = Math.Min(min, p.Y);
            max = Math.Max(max, p.Y + e.Height);
        }

        return max - min;
    }

    public virtual Point GetLocalPosition(Element child)
    {
        if (child.IsValid)
        {
            Vector2 anchor = new(
                (int)(Width * child.Anchor.X),
                (int)(Height * child.Anchor.Y)
            );

            Vector2 origin = new(
                (int)(child.Width * child.Align.X),
                (int)(child.Height * child.Align.Y)
            );

            return (anchor - origin + child.Offset).ToPoint();
        }

        return Point.Zero;
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        foreach (Element child in Children)
        {
            child.Update(dt);
        }
    }

    public override void Render()
    {
        foreach (Element child in Children)
        {
            child.Render();
        }
    }

    protected override void DrawForeground(SpriteBatch batch)
    {
        // Nothing to draw
    }

    public override void Draw(SpriteBatch batch)
    {
        if (!Visible) return;

        DrawBackground(batch);

        foreach (Element child in Children)
        {
            child.Draw(batch);
        }

        DrawBorders(batch);
    }

    public bool HandleMouseEvents(MouseInputController mouseInput)
    {
        foreach (Element element in Children)
        {
            if (element is IMouseInputEventHandler h)
            {
                if (h.HandleMouseEvents(mouseInput)) return true;
            }
        }

        return false;
    }

    #region IEnumerable

    public IEnumerator<Element> GetEnumerator() => Children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region ICollection

    public bool IsReadOnly => false;

    public void Add(Element item)
    {
        item.Parent = this;
        Children.Add(item);
    }

    public void Clear()
    {
        foreach (Element child in Children)
        {
            child.Parent = null;
        }

        Children.Clear();
    }

    public bool Contains(Element item)
    {
        return Children.Contains(item);
    }

    public void CopyTo(Element[] array, int arrayIndex)
    {
        Children.CopyTo(array, arrayIndex);
    }

    public bool Remove(Element item)
    {
        return Children.Remove(item);
    }

    #endregion
}