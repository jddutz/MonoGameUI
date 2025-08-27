using Microsoft.Xna.Framework;
using MonoGameUI.Core;
using System;

namespace MonoGameUI.Components;

/// <summary>
/// Component that provides scrollable content areas with scroll bars and wheel support.
/// Supports both horizontal and vertical scrolling with customizable scroll behavior.
/// </summary>
public class ScrollComponent : Core.Component
{
    private Vector2 _scrollOffset = Vector2.Zero;
    private Vector2 _contentSize = Vector2.Zero;
    private Vector2 _viewportSize = Vector2.Zero;
    private bool _horizontalScrollEnabled = true;
    private bool _verticalScrollEnabled = true;
    private float _scrollSpeed = 20f;
    private float _scrollbarThickness = 16f;
    private ScrollBarMode _horizontalScrollbarMode = ScrollBarMode.Auto;
    private ScrollBarMode _verticalScrollbarMode = ScrollBarMode.Auto;

    /// <summary>
    /// Current scroll offset in pixels.
    /// </summary>
    public Vector2 ScrollOffset
    {
        get => _scrollOffset;
        set
        {
            var newOffset = ClampScrollOffset(value);
            var offsetChanged = _scrollOffset != newOffset;
            var valueChanged = value != _scrollOffset; // Check if the input value differs from current

            if (offsetChanged)
            {
                _scrollOffset = newOffset;
                OnScrollChanged?.Invoke(this, new ScrollChangedEventArgs(_scrollOffset));
            }

            // Mark dirty if either the offset changed OR if user attempted to set a different value
            if (offsetChanged || valueChanged)
            {
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Size of the scrollable content in pixels.
    /// </summary>
    public Vector2 ContentSize
    {
        get => _contentSize;
        set
        {
            if (_contentSize != value)
            {
                _contentSize = value;
                _scrollOffset = ClampScrollOffset(_scrollOffset);
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Size of the visible viewport in pixels.
    /// </summary>
    public Vector2 ViewportSize
    {
        get => _viewportSize;
        set
        {
            if (_viewportSize != value)
            {
                _viewportSize = value;
                _scrollOffset = ClampScrollOffset(_scrollOffset);
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Whether horizontal scrolling is enabled.
    /// </summary>
    public bool HorizontalScrollEnabled
    {
        get => _horizontalScrollEnabled;
        set
        {
            if (_horizontalScrollEnabled != value)
            {
                _horizontalScrollEnabled = value;
                if (!value)
                {
                    ScrollOffset = new Vector2(0, _scrollOffset.Y);
                }
                else
                {
                    MarkDirty(DirtyFlags.Layout);
                }
            }
        }
    }

    /// <summary>
    /// Whether vertical scrolling is enabled.
    /// </summary>
    public bool VerticalScrollEnabled
    {
        get => _verticalScrollEnabled;
        set
        {
            if (_verticalScrollEnabled != value)
            {
                _verticalScrollEnabled = value;
                if (!value)
                {
                    ScrollOffset = new Vector2(_scrollOffset.X, 0);
                }
                else
                {
                    MarkDirty(DirtyFlags.Layout);
                }
            }
        }
    }

    /// <summary>
    /// Scroll speed in pixels per wheel notch.
    /// </summary>
    public float ScrollSpeed
    {
        get => _scrollSpeed;
        set
        {
            if (_scrollSpeed != value && value > 0)
            {
                _scrollSpeed = value;
            }
        }
    }

    /// <summary>
    /// Thickness of scroll bars in pixels.
    /// </summary>
    public float ScrollbarThickness
    {
        get => _scrollbarThickness;
        set
        {
            if (_scrollbarThickness != value && value > 0)
            {
                _scrollbarThickness = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Horizontal scrollbar display mode.
    /// </summary>
    public ScrollBarMode HorizontalScrollbarMode
    {
        get => _horizontalScrollbarMode;
        set
        {
            if (_horizontalScrollbarMode != value)
            {
                _horizontalScrollbarMode = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Vertical scrollbar display mode.
    /// </summary>
    public ScrollBarMode VerticalScrollbarMode
    {
        get => _verticalScrollbarMode;
        set
        {
            if (_verticalScrollbarMode != value)
            {
                _verticalScrollbarMode = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Whether the horizontal scrollbar should be visible.
    /// </summary>
    public bool ShowHorizontalScrollbar =>
        _horizontalScrollbarMode switch
        {
            ScrollBarMode.Always => true,
            ScrollBarMode.Never => false,
            ScrollBarMode.Auto => _horizontalScrollEnabled && _contentSize.X > _viewportSize.X,
            _ => false
        };

    /// <summary>
    /// Whether the vertical scrollbar should be visible.
    /// </summary>
    public bool ShowVerticalScrollbar =>
        _verticalScrollbarMode switch
        {
            ScrollBarMode.Always => true,
            ScrollBarMode.Never => false,
            ScrollBarMode.Auto => _verticalScrollEnabled && _contentSize.Y > _viewportSize.Y,
            _ => false
        };

    /// <summary>
    /// Maximum scroll offset in the X direction.
    /// </summary>
    public float MaxScrollX => Math.Max(0, _contentSize.X - _viewportSize.X);

    /// <summary>
    /// Maximum scroll offset in the Y direction.
    /// </summary>
    public float MaxScrollY => Math.Max(0, _contentSize.Y - _viewportSize.Y);

    /// <summary>
    /// Whether the content can be scrolled horizontally.
    /// </summary>
    public bool CanScrollHorizontally => _horizontalScrollEnabled && MaxScrollX > 0;

    /// <summary>
    /// Whether the content can be scrolled vertically.
    /// </summary>
    public bool CanScrollVertically => _verticalScrollEnabled && MaxScrollY > 0;

    /// <summary>
    /// Event fired when the scroll offset changes.
    /// </summary>
    public event EventHandler<ScrollChangedEventArgs>? OnScrollChanged;

    /// <summary>
    /// Scrolls by the specified delta amount.
    /// </summary>
    /// <param name="delta">Amount to scroll in pixels</param>
    public void ScrollBy(Vector2 delta)
    {
        ScrollOffset += delta;
    }

    /// <summary>
    /// Scrolls to the specified absolute position.
    /// </summary>
    /// <param name="position">Absolute scroll position in pixels</param>
    public void ScrollTo(Vector2 position)
    {
        ScrollOffset = position;
    }

    /// <summary>
    /// Scrolls to the specified position as a percentage (0-1).
    /// </summary>
    /// <param name="percentage">Scroll position as percentage (0-1)</param>
    public void ScrollToPercentage(Vector2 percentage)
    {
        var maxScroll = new Vector2(MaxScrollX, MaxScrollY);
        ScrollOffset = Vector2.Multiply(maxScroll, Vector2.Clamp(percentage, Vector2.Zero, Vector2.One));
    }

    /// <summary>
    /// Scrolls to ensure the specified rectangle is visible.
    /// </summary>
    /// <param name="rect">Rectangle to make visible in content coordinates</param>
    public void ScrollToVisible(Rectangle rect)
    {
        var newOffset = _scrollOffset;

        // Horizontal scrolling
        if (_horizontalScrollEnabled)
        {
            if (rect.Left < _scrollOffset.X)
            {
                newOffset.X = rect.Left;
            }
            else if (rect.Right > _scrollOffset.X + _viewportSize.X)
            {
                newOffset.X = rect.Right - _viewportSize.X;
            }
        }

        // Vertical scrolling
        if (_verticalScrollEnabled)
        {
            if (rect.Top < _scrollOffset.Y)
            {
                newOffset.Y = rect.Top;
            }
            else if (rect.Bottom > _scrollOffset.Y + _viewportSize.Y)
            {
                newOffset.Y = rect.Bottom - _viewportSize.Y;
            }
        }

        ScrollOffset = newOffset;
    }

    /// <summary>
    /// Handles mouse wheel input for scrolling.
    /// </summary>
    /// <param name="wheelDelta">Mouse wheel delta value</param>
    /// <param name="isHorizontal">Whether this is horizontal scrolling (e.g., Shift+Wheel)</param>
    public void HandleMouseWheel(float wheelDelta, bool isHorizontal = false)
    {
        var delta = Vector2.Zero;
        var scrollAmount = wheelDelta * _scrollSpeed;

        if (isHorizontal && _horizontalScrollEnabled)
        {
            delta.X = -scrollAmount;
        }
        else if (!isHorizontal && _verticalScrollEnabled)
        {
            delta.Y = -scrollAmount;
        }

        ScrollBy(delta);
    }

    /// <summary>
    /// Resets scroll position to the top-left corner.
    /// </summary>
    public void ResetScroll()
    {
        ScrollOffset = Vector2.Zero;
    }

    /// <summary>
    /// Updates content and viewport sizes based on the current entity layout.
    /// This should be called by the layout system.
    /// </summary>
    public void UpdateLayout(Rectangle availableSpace)
    {
        ViewportSize = new Vector2(availableSpace.Width, availableSpace.Height);

        // Calculate content size based on children
        if (Entity?.Children != null)
        {
            var contentBounds = Rectangle.Empty;
            foreach (var child in Entity.Children)
            {
                var transform = child.GetComponent<TransformComponent>();
                if (transform != null)
                {
                    var childBounds = new Rectangle(
                        (int)transform.Position.X,
                        (int)transform.Position.Y,
                        (int)transform.Size.X,
                        (int)transform.Size.Y);

                    if (contentBounds == Rectangle.Empty)
                        contentBounds = childBounds;
                    else
                        contentBounds = Rectangle.Union(contentBounds, childBounds);
                }
            }

            if (contentBounds != Rectangle.Empty)
            {
                ContentSize = new Vector2(
                    contentBounds.Right,
                    contentBounds.Bottom);
            }
        }

        // Apply scroll offset to children
        ApplyScrollToChildren();
    }

    /// <summary>
    /// Applies the current scroll offset to all child entities.
    /// </summary>
    private void ApplyScrollToChildren()
    {
        if (Entity?.Children == null) return;

        foreach (var child in Entity.Children)
        {
            var transform = child.GetComponent<TransformComponent>();
            if (transform != null)
            {
                // Apply scroll offset by modifying the child's position
                // Note: This assumes the child's Position is in content coordinates
                var scrolledPosition = transform.Position - _scrollOffset;
                // Only update if the position actually changed to avoid infinite loops
                if (transform.Position != scrolledPosition)
                {
                    transform.Position = scrolledPosition;
                }
            }
        }
    }

    /// <summary>
    /// Clamps the scroll offset to valid bounds.
    /// </summary>
    private Vector2 ClampScrollOffset(Vector2 offset)
    {
        var clampedOffset = offset;

        if (!_horizontalScrollEnabled)
            clampedOffset.X = 0;
        else
            clampedOffset.X = MathHelper.Clamp(clampedOffset.X, 0, MaxScrollX);

        if (!_verticalScrollEnabled)
            clampedOffset.Y = 0;
        else
            clampedOffset.Y = MathHelper.Clamp(clampedOffset.Y, 0, MaxScrollY);

        return clampedOffset;
    }
}

/// <summary>
/// Defines when scrollbars should be displayed.
/// </summary>
public enum ScrollBarMode
{
    /// <summary>
    /// Show scrollbar automatically when content exceeds viewport.
    /// </summary>
    Auto,

    /// <summary>
    /// Always show scrollbar regardless of content size.
    /// </summary>
    Always,

    /// <summary>
    /// Never show scrollbar.
    /// </summary>
    Never
}

/// <summary>
/// Event arguments for scroll change events.
/// </summary>
public class ScrollChangedEventArgs : EventArgs
{
    public Vector2 ScrollOffset { get; }

    public ScrollChangedEventArgs(Vector2 scrollOffset)
    {
        ScrollOffset = scrollOffset;
    }
}
