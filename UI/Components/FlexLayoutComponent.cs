using Microsoft.Xna.Framework;
using MonoGameUI.Core;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameUI.Components;

/// <summary>
/// Component that provides flexible box layout functionality similar to CSS Flexbox.
/// Allows arranging child entities in rows or columns with various alignment and distribution options.
/// </summary>
public class FlexLayoutComponent : Core.Component, ILayoutComponent
{
    private FlexDirection _flexDirection = FlexDirection.Row;
    private JustifyContent _justifyContent = JustifyContent.FlexStart;
    private AlignItems _alignItems = AlignItems.Stretch;
    private FlexWrap _flexWrap = FlexWrap.NoWrap;
    private float _gap = 0f;
    private float _rowGap = 0f;
    private float _columnGap = 0f;

    /// <summary>
    /// Defines the direction of the main axis (row or column).
    /// </summary>
    public FlexDirection FlexDirection
    {
        get => _flexDirection;
        set
        {
            if (_flexDirection != value)
            {
                _flexDirection = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Defines how flex items are aligned along the main axis.
    /// </summary>
    public JustifyContent JustifyContent
    {
        get => _justifyContent;
        set
        {
            if (_justifyContent != value)
            {
                _justifyContent = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Defines how flex items are aligned along the cross axis.
    /// </summary>
    public AlignItems AlignItems
    {
        get => _alignItems;
        set
        {
            if (_alignItems != value)
            {
                _alignItems = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Defines whether flex items wrap to new lines.
    /// </summary>
    public FlexWrap FlexWrap
    {
        get => _flexWrap;
        set
        {
            if (_flexWrap != value)
            {
                _flexWrap = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Gap between flex items in both directions.
    /// When set, overrides both RowGap and ColumnGap.
    /// </summary>
    public float Gap
    {
        get => _gap;
        set
        {
            if (_gap != value)
            {
                _gap = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Gap between rows of flex items.
    /// </summary>
    public float RowGap
    {
        get => _rowGap;
        set
        {
            if (_rowGap != value)
            {
                _rowGap = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Gap between columns of flex items.
    /// </summary>
    public float ColumnGap
    {
        get => _columnGap;
        set
        {
            if (_columnGap != value)
            {
                _columnGap = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Whether the main axis is horizontal (row directions).
    /// </summary>
    public bool IsMainAxisHorizontal => FlexDirection == FlexDirection.Row || FlexDirection == FlexDirection.RowReverse;

    /// <summary>
    /// Whether the flex direction is reversed.
    /// </summary>
    public bool IsReversed => FlexDirection == FlexDirection.RowReverse || FlexDirection == FlexDirection.ColumnReverse;

    /// <summary>
    /// Calculate the desired size for this flex container.
    /// </summary>
    /// <param name="availableSize">Available space for the container.</param>
    /// <returns>The desired size.</returns>
    public Vector2 CalculateDesiredSize(Vector2 availableSize)
    {
        if (Entity?.Children == null || !Entity.Children.Any())
        {
            return Vector2.Zero;
        }

        var children = GetFlexChildren();
        if (!children.Any())
        {
            return Vector2.Zero;
        }

        float totalMainSize = 0f;
        float maxCrossSize = 0f;
        float gapSize = Gap > 0 ? Gap : (IsMainAxisHorizontal ? ColumnGap : RowGap);

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var childSize = child.Size;

            if (IsMainAxisHorizontal)
            {
                totalMainSize += childSize.X;
                maxCrossSize = MathHelper.Max(maxCrossSize, childSize.Y);
            }
            else
            {
                totalMainSize += childSize.Y;
                maxCrossSize = MathHelper.Max(maxCrossSize, childSize.X);
            }

            // Add gap between items (not after the last item)
            if (i < children.Count - 1)
            {
                totalMainSize += gapSize;
            }
        }

        return IsMainAxisHorizontal
            ? new Vector2(totalMainSize, maxCrossSize)
            : new Vector2(maxCrossSize, totalMainSize);
    }

    /// <summary>
    /// Arrange the flex items within the given bounds.
    /// </summary>
    /// <param name="bounds">The final bounds for the container.</param>
    public void Arrange(Rectangle bounds)
    {
        if (Entity?.Children == null || !Entity.Children.Any())
        {
            return;
        }

        var children = GetFlexChildren();
        if (!children.Any())
        {
            return;
        }

        var containerSize = new Vector2(bounds.Width, bounds.Height);
        var gapSize = Gap > 0 ? Gap : (IsMainAxisHorizontal ? ColumnGap : RowGap);

        // Calculate total size of all children along main axis
        float totalChildrenSize = 0f;
        foreach (var child in children)
        {
            totalChildrenSize += IsMainAxisHorizontal ? child.Size.X : child.Size.Y;
        }

        // Add gaps
        if (children.Count > 1)
        {
            totalChildrenSize += gapSize * (children.Count - 1);
        }

        // Calculate available extra space
        float availableMainSize = IsMainAxisHorizontal ? containerSize.X : containerSize.Y;
        float extraSpace = availableMainSize - totalChildrenSize;

        // Calculate starting position and spacing based on JustifyContent
        float currentPosition = 0f;
        float itemSpacing = 0f;

        switch (JustifyContent)
        {
            case JustifyContent.FlexStart:
                currentPosition = 0f;
                break;
            case JustifyContent.FlexEnd:
                currentPosition = extraSpace;
                break;
            case JustifyContent.Center:
                currentPosition = extraSpace / 2f;
                break;
            case JustifyContent.SpaceBetween:
                currentPosition = 0f;
                if (children.Count > 1)
                    itemSpacing = extraSpace / (children.Count - 1);
                break;
            case JustifyContent.SpaceAround:
                itemSpacing = children.Count > 0 ? extraSpace / children.Count : 0f;
                currentPosition = itemSpacing / 2f;
                break;
            case JustifyContent.SpaceEvenly:
                itemSpacing = children.Count > 0 ? extraSpace / (children.Count + 1) : 0f;
                currentPosition = itemSpacing;
                break;
        }

        // Position each child
        var childrenToProcess = IsReversed ? children.AsEnumerable().Reverse().ToList() : children;

        foreach (var child in childrenToProcess)
        {
            var childTransform = child.Entity?.GetComponent<TransformComponent>();
            if (childTransform == null) continue;

            Vector2 childPosition;

            if (IsMainAxisHorizontal)
            {
                // Calculate cross-axis position based on AlignItems
                float crossPosition = CalculateCrossAxisPosition(child.Size.Y, containerSize.Y);
                childPosition = new Vector2(bounds.X + currentPosition, bounds.Y + crossPosition);
                currentPosition += child.Size.X + gapSize + itemSpacing;
            }
            else
            {
                // Calculate cross-axis position based on AlignItems
                float crossPosition = CalculateCrossAxisPosition(child.Size.X, containerSize.X);
                childPosition = new Vector2(bounds.X + crossPosition, bounds.Y + currentPosition);
                currentPosition += child.Size.Y + gapSize + itemSpacing;
            }

            childTransform.Position = childPosition;
        }
    }

    /// <summary>
    /// Calculate the cross-axis position for a child based on AlignItems.
    /// </summary>
    private float CalculateCrossAxisPosition(float childCrossSize, float containerCrossSize)
    {
        return AlignItems switch
        {
            AlignItems.FlexStart => 0f,
            AlignItems.FlexEnd => containerCrossSize - childCrossSize,
            AlignItems.Center => (containerCrossSize - childCrossSize) / 2f,
            AlignItems.Stretch => 0f, // TODO: Implement stretching
            AlignItems.Baseline => 0f, // TODO: Implement baseline alignment
            _ => 0f
        };
    }

    /// <summary>
    /// Get all child entities that have TransformComponents.
    /// </summary>
    private List<TransformComponent> GetFlexChildren()
    {
        var children = new List<TransformComponent>();

        if (Entity?.Children != null)
        {
            foreach (var child in Entity.Children)
            {
                var transform = child.GetComponent<TransformComponent>();
                if (transform != null)
                {
                    children.Add(transform);
                }
            }
        }

        return children;
    }
}
