using Microsoft.Xna.Framework;
using MonoGameUI.Core;

namespace MonoGameUI.Components;

/// <summary>
/// Defines the direction for layout flow in containers.
/// </summary>
public enum FlexDirection
{
    /// <summary>
    /// Layout children horizontally (left to right).
    /// </summary>
    Row,

    /// <summary>
    /// Layout children horizontally (right to left).
    /// </summary>
    RowReverse,

    /// <summary>
    /// Layout children vertically (top to bottom).
    /// </summary>
    Column,

    /// <summary>
    /// Layout children vertically (bottom to top).
    /// </summary>
    ColumnReverse
}

/// <summary>
/// Defines how flex items are aligned along the main axis.
/// </summary>
public enum JustifyContent
{
    /// <summary>
    /// Items are aligned to the start of the flex container.
    /// </summary>
    FlexStart,

    /// <summary>
    /// Items are aligned to the end of the flex container.
    /// </summary>
    FlexEnd,

    /// <summary>
    /// Items are centered along the main axis.
    /// </summary>
    Center,

    /// <summary>
    /// Items are evenly distributed with equal space between them.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Items are evenly distributed with equal space around them.
    /// </summary>
    SpaceAround,

    /// <summary>
    /// Items are evenly distributed with equal space around them (including edges).
    /// </summary>
    SpaceEvenly
}

/// <summary>
/// Defines how flex items are aligned along the cross axis.
/// </summary>
public enum AlignItems
{
    /// <summary>
    /// Items stretch to fill the cross axis.
    /// </summary>
    Stretch,

    /// <summary>
    /// Items are aligned to the start of the cross axis.
    /// </summary>
    FlexStart,

    /// <summary>
    /// Items are aligned to the end of the cross axis.
    /// </summary>
    FlexEnd,

    /// <summary>
    /// Items are centered along the cross axis.
    /// </summary>
    Center,

    /// <summary>
    /// Items are aligned to their baseline.
    /// </summary>
    Baseline
}

/// <summary>
/// Defines whether flex items wrap to new lines.
/// </summary>
public enum FlexWrap
{
    /// <summary>
    /// Items do not wrap and stay on a single line.
    /// </summary>
    NoWrap,

    /// <summary>
    /// Items wrap to new lines as needed.
    /// </summary>
    Wrap,

    /// <summary>
    /// Items wrap to new lines in reverse order.
    /// </summary>
    WrapReverse
}

/// <summary>
/// Defines how an entity's size and position are calculated relative to its parent.
/// </summary>
public enum SizeMode
{
    /// <summary>
    /// Fixed size in pixels.
    /// </summary>
    Fixed,

    /// <summary>
    /// Size relative to parent (e.g., parent width + offset).
    /// </summary>
    Relative,

    /// <summary>
    /// Percentage of parent size (0.0 to 1.0).
    /// </summary>
    Percent,

    /// <summary>
    /// Fill the entire parent container.
    /// </summary>
    Fill,

    /// <summary>
    /// Size based on aspect ratio of another dimension.
    /// </summary>
    AspectRatio,

    /// <summary>
    /// Size to fit content (children or intrinsic content).
    /// </summary>
    FitContent
}

/// <summary>
/// Defines how an entity is positioned relative to its parent.
/// </summary>
public enum AnchorMode
{
    /// <summary>
    /// Position relative to top-left corner.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Position relative to top-center.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Position relative to top-right corner.
    /// </summary>
    TopRight,

    /// <summary>
    /// Position relative to middle-left.
    /// </summary>
    MiddleLeft,

    /// <summary>
    /// Position relative to center.
    /// </summary>
    MiddleCenter,

    /// <summary>
    /// Position relative to middle-right.
    /// </summary>
    MiddleRight,

    /// <summary>
    /// Position relative to bottom-left corner.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Position relative to bottom-center.
    /// </summary>
    BottomCenter,

    /// <summary>
    /// Position relative to bottom-right corner.
    /// </summary>
    BottomRight,

    /// <summary>
    /// Custom anchor point.
    /// </summary>
    Custom
}

/// <summary>
/// Component that handles layout calculations for positioning and sizing entities.
/// </summary>
public class LayoutComponent : Core.Component, ILayoutComponent
{
    private SizeMode _widthMode = SizeMode.Fixed;
    private SizeMode _heightMode = SizeMode.Fixed;
    private AnchorMode _anchorMode = AnchorMode.TopLeft;
    private Vector2 _customAnchor = Vector2.Zero;
    private Vector2 _offset = Vector2.Zero;
    private Vector2 _relativeSize = Vector2.Zero;
    private float _aspectRatio = 1.0f;
    private Vector2 _minSize = Vector2.Zero;
    private Vector2 _maxSize = new(float.MaxValue);
    private FlexDirection _flexDirection = FlexDirection.Column;
    private float _gap = 0f;

    /// <summary>
    /// How the width is calculated.
    /// </summary>
    public SizeMode WidthMode
    {
        get => _widthMode;
        set
        {
            if (_widthMode != value)
            {
                _widthMode = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// How the height is calculated.
    /// </summary>
    public SizeMode HeightMode
    {
        get => _heightMode;
        set
        {
            if (_heightMode != value)
            {
                _heightMode = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// How the entity is anchored to its parent.
    /// </summary>
    public AnchorMode AnchorMode
    {
        get => _anchorMode;
        set
        {
            if (_anchorMode != value)
            {
                _anchorMode = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Custom anchor point when AnchorMode is Custom (0,0 = top-left, 1,1 = bottom-right).
    /// </summary>
    public Vector2 CustomAnchor
    {
        get => _customAnchor;
        set
        {
            if (_customAnchor != value)
            {
                _customAnchor = value;
                if (_anchorMode == AnchorMode.Custom)
                {
                    MarkDirty(DirtyFlags.Layout);
                }
            }
        }
    }

    /// <summary>
    /// Offset from the anchor point.
    /// </summary>
    public Vector2 Offset
    {
        get => _offset;
        set
        {
            if (_offset != value)
            {
                _offset = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Size value used for Relative, Percent, and AspectRatio modes.
    /// </summary>
    public Vector2 RelativeSize
    {
        get => _relativeSize;
        set
        {
            if (_relativeSize != value)
            {
                _relativeSize = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Aspect ratio for AspectRatio size mode.
    /// </summary>
    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            if (_aspectRatio != value)
            {
                _aspectRatio = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Minimum size constraints.
    /// </summary>
    public Vector2 MinSize
    {
        get => _minSize;
        set
        {
            if (_minSize != value)
            {
                _minSize = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Maximum size constraints.
    /// </summary>
    public Vector2 MaxSize
    {
        get => _maxSize;
        set
        {
            if (_maxSize != value)
            {
                _maxSize = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Direction for layout flow when this entity contains children.
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
    /// Spacing between child elements in layouts.
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
    /// Calculate the desired size for this component.
    /// </summary>
    /// <param name="availableSize">Available space for the component.</param>
    /// <returns>The desired size.</returns>
    public Vector2 CalculateDesiredSize(Vector2 availableSize)
    {
        var transform = Entity?.GetComponent<TransformComponent>();
        if (transform == null)
        {
            return Vector2.Zero;
        }

        var width = CalculateWidth(availableSize.X, transform.Width);
        var height = CalculateHeight(availableSize.Y, transform.Height);

        // Apply constraints
        width = MathHelper.Clamp(width, _minSize.X, _maxSize.X);
        height = MathHelper.Clamp(height, _minSize.Y, _maxSize.Y);

        return new Vector2(width, height);
    }

    /// <summary>
    /// Arrange the component within the given bounds.
    /// </summary>
    /// <param name="bounds">The final bounds for the component.</param>
    public void Arrange(Rectangle bounds)
    {
        var transform = Entity?.GetComponent<TransformComponent>();
        if (transform == null)
        {
            return;
        }

        var desiredSize = CalculateDesiredSize(new Vector2(bounds.Width, bounds.Height));
        var anchorPoint = GetAnchorPoint(bounds);
        var position = anchorPoint + _offset;

        transform.Position = position;
        transform.Size = desiredSize;
    }

    private float CalculateWidth(float availableWidth, float currentWidth)
    {
        return _widthMode switch
        {
            SizeMode.Fixed => currentWidth,
            SizeMode.Relative => availableWidth + _relativeSize.X,
            SizeMode.Percent => availableWidth * _relativeSize.X,
            SizeMode.Fill => availableWidth,
            SizeMode.AspectRatio => CalculateHeight(float.MaxValue, 0) * _aspectRatio,
            SizeMode.FitContent => CalculateContentWidth(),
            _ => currentWidth
        };
    }

    private float CalculateHeight(float availableHeight, float currentHeight)
    {
        return _heightMode switch
        {
            SizeMode.Fixed => currentHeight,
            SizeMode.Relative => availableHeight + _relativeSize.Y,
            SizeMode.Percent => availableHeight * _relativeSize.Y,
            SizeMode.Fill => availableHeight,
            SizeMode.AspectRatio => CalculateWidth(float.MaxValue, 0) / _aspectRatio,
            SizeMode.FitContent => CalculateContentHeight(),
            _ => currentHeight
        };
    }

    private Vector2 GetAnchorPoint(Rectangle parentBounds)
    {
        var anchor = _anchorMode switch
        {
            AnchorMode.TopLeft => Vector2.Zero,
            AnchorMode.TopCenter => new Vector2(0.5f, 0f),
            AnchorMode.TopRight => new Vector2(1f, 0f),
            AnchorMode.MiddleLeft => new Vector2(0f, 0.5f),
            AnchorMode.MiddleCenter => new Vector2(0.5f, 0.5f),
            AnchorMode.MiddleRight => new Vector2(1f, 0.5f),
            AnchorMode.BottomLeft => new Vector2(0f, 1f),
            AnchorMode.BottomCenter => new Vector2(0.5f, 1f),
            AnchorMode.BottomRight => Vector2.One,
            AnchorMode.Custom => _customAnchor,
            _ => Vector2.Zero
        };

        return new Vector2(
            parentBounds.X + parentBounds.Width * anchor.X,
            parentBounds.Y + parentBounds.Height * anchor.Y
        );
    }

    private float CalculateContentWidth()
    {
        // This would calculate the width needed to fit all child content
        // For now, return a default value
        return 100f; // TODO: Implement content measurement
    }

    private float CalculateContentHeight()
    {
        // This would calculate the height needed to fit all child content
        // For now, return a default value
        return 100f; // TODO: Implement content measurement
    }

    public override bool Validate()
    {
        if (!base.Validate())
        {
            return false;
        }

        // Validate that we have a transform component
        return Entity?.HasComponent<TransformComponent>() == true;
    }
}
