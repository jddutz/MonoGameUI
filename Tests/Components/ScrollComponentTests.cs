using Microsoft.Xna.Framework;
using MonoGameUI.Components;
using MonoGameUI.Core;
using System;
using Xunit;

namespace MonoGameUI.Tests.Components;

public class ScrollComponentTests
{
    [Fact(DisplayName = "Constructor should initialize with default values")]
    public void ConstructorTest01()
    {
        // Arrange & Act
        var component = new ScrollComponent();

        // Assert
        Assert.Equal(Vector2.Zero, component.ScrollOffset);
        Assert.Equal(Vector2.Zero, component.ContentSize);
        Assert.Equal(Vector2.Zero, component.ViewportSize);
        Assert.True(component.HorizontalScrollEnabled);
        Assert.True(component.VerticalScrollEnabled);
        Assert.Equal(20f, component.ScrollSpeed);
        Assert.Equal(16f, component.ScrollbarThickness);
        Assert.Equal(ScrollBarMode.Auto, component.HorizontalScrollbarMode);
        Assert.Equal(ScrollBarMode.Auto, component.VerticalScrollbarMode);
    }

    [Fact(DisplayName = "ScrollOffset setter should clamp to valid bounds")]
    public void ScrollOffsetTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ScrollOffset = new Vector2(700, 600); // Beyond max

        // Assert
        Assert.Equal(new Vector2(600, 500), component.ScrollOffset); // Clamped to max
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ScrollOffset setter should clamp negative values to zero")]
    public void ScrollOffsetTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ScrollOffset = new Vector2(-50, -100);

        // Assert
        Assert.Equal(Vector2.Zero, component.ScrollOffset);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ContentSize setter should update scroll bounds")]
    public void ContentSizeTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(800, 600); // Set initial content size first
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollOffset = new Vector2(200, 150);
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ContentSize = new Vector2(500, 400);

        // Assert
        Assert.Equal(new Vector2(500, 400), component.ContentSize);
        Assert.Equal(new Vector2(100, 100), component.ScrollOffset); // Adjusted to new max
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ViewportSize setter should update scroll bounds")]
    public void ViewportSizeTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ScrollOffset = new Vector2(700, 600);
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.ViewportSize = new Vector2(600, 500);

        // Assert
        Assert.Equal(new Vector2(600, 500), component.ViewportSize);
        Assert.Equal(new Vector2(400, 300), component.ScrollOffset); // Adjusted to new max
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "HorizontalScrollEnabled setter should reset horizontal scroll when disabled")]
    public void HorizontalScrollEnabledTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollOffset = new Vector2(100, 50);
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.HorizontalScrollEnabled = false;

        // Assert
        Assert.False(component.HorizontalScrollEnabled);
        Assert.Equal(new Vector2(0, 50), component.ScrollOffset);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "VerticalScrollEnabled setter should reset vertical scroll when disabled")]
    public void VerticalScrollEnabledTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollOffset = new Vector2(100, 50);
        entity.DirtyFlags = DirtyFlags.None;

        // Act
        component.VerticalScrollEnabled = false;

        // Assert
        Assert.False(component.VerticalScrollEnabled);
        Assert.Equal(new Vector2(100, 0), component.ScrollOffset);
        Assert.True(entity.DirtyFlags.HasFlag(DirtyFlags.Layout));
    }

    [Fact(DisplayName = "ScrollSpeed setter should reject non-positive values")]
    public void ScrollSpeedTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.ScrollSpeed = 25f;

        // Act
        component.ScrollSpeed = -10f;

        // Assert
        Assert.Equal(25f, component.ScrollSpeed); // Should remain unchanged
    }

    [Fact(DisplayName = "ScrollbarThickness setter should reject non-positive values")]
    public void ScrollbarThicknessTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ScrollbarThickness = 20f;

        // Act
        component.ScrollbarThickness = 0f;

        // Assert
        Assert.Equal(20f, component.ScrollbarThickness); // Should remain unchanged
    }

    [Fact(DisplayName = "ShowHorizontalScrollbar should return correct value for Auto mode")]
    public void ShowHorizontalScrollbarTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.HorizontalScrollbarMode = ScrollBarMode.Auto;
        component.HorizontalScrollEnabled = true;
        component.ContentSize = new Vector2(1000, 600);
        component.ViewportSize = new Vector2(800, 400);

        // Act & Assert
        Assert.True(component.ShowHorizontalScrollbar); // Content wider than viewport
    }

    [Fact(DisplayName = "ShowHorizontalScrollbar should return false when content fits")]
    public void ShowHorizontalScrollbarTest02()
    {
        // Arrange
        var component = new ScrollComponent();
        component.HorizontalScrollbarMode = ScrollBarMode.Auto;
        component.HorizontalScrollEnabled = true;
        component.ContentSize = new Vector2(600, 600);
        component.ViewportSize = new Vector2(800, 400);

        // Act & Assert
        Assert.False(component.ShowHorizontalScrollbar); // Content fits in viewport
    }

    [Fact(DisplayName = "ShowVerticalScrollbar should return correct value for Always mode")]
    public void ShowVerticalScrollbarTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.VerticalScrollbarMode = ScrollBarMode.Always;

        // Act & Assert
        Assert.True(component.ShowVerticalScrollbar);
    }

    [Fact(DisplayName = "ShowVerticalScrollbar should return false for Never mode")]
    public void ShowVerticalScrollbarTest02()
    {
        // Arrange
        var component = new ScrollComponent();
        component.VerticalScrollbarMode = ScrollBarMode.Never;

        // Act & Assert
        Assert.False(component.ShowVerticalScrollbar);
    }

    [Fact(DisplayName = "MaxScrollX should calculate correct maximum horizontal scroll")]
    public void MaxScrollXTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.ContentSize = new Vector2(1000, 600);
        component.ViewportSize = new Vector2(400, 300);

        // Act & Assert
        Assert.Equal(600f, component.MaxScrollX);
    }

    [Fact(DisplayName = "MaxScrollY should calculate correct maximum vertical scroll")]
    public void MaxScrollYTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);

        // Act & Assert
        Assert.Equal(500f, component.MaxScrollY);
    }

    [Fact(DisplayName = "CanScrollHorizontally should return correct value")]
    public void CanScrollHorizontallyTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.HorizontalScrollEnabled = true;
        component.ContentSize = new Vector2(1000, 600);
        component.ViewportSize = new Vector2(400, 300);

        // Act & Assert
        Assert.True(component.CanScrollHorizontally);
    }

    [Fact(DisplayName = "CanScrollVertically should return false when disabled")]
    public void CanScrollVerticallyTest01()
    {
        // Arrange
        var component = new ScrollComponent();
        component.VerticalScrollEnabled = false;
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);

        // Act & Assert
        Assert.False(component.CanScrollVertically);
    }

    [Fact(DisplayName = "ScrollBy should add delta to current offset")]
    public void ScrollByTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollOffset = new Vector2(100, 50);

        // Act
        component.ScrollBy(new Vector2(50, 25));

        // Assert
        Assert.Equal(new Vector2(150, 75), component.ScrollOffset);
    }

    [Fact(DisplayName = "ScrollTo should set absolute position")]
    public void ScrollToTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);

        // Act
        component.ScrollTo(new Vector2(200, 150));

        // Assert
        Assert.Equal(new Vector2(200, 150), component.ScrollOffset);
    }

    [Fact(DisplayName = "ScrollToPercentage should set position as percentage of max scroll")]
    public void ScrollToPercentageTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);

        // Act
        component.ScrollToPercentage(new Vector2(0.5f, 0.8f));

        // Assert
        Assert.Equal(new Vector2(300, 400), component.ScrollOffset); // 50% of 600, 80% of 500
    }

    [Fact(DisplayName = "HandleMouseWheel should scroll vertically by default")]
    public void HandleMouseWheelTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollSpeed = 10f;
        component.ScrollOffset = new Vector2(0, 100); // Start with some scroll position

        // Act
        component.HandleMouseWheel(3f); // Positive wheel delta (scroll up)

        // Assert
        Assert.Equal(new Vector2(0, 70), component.ScrollOffset); // Should scroll up by 30 pixels
    }

    [Fact(DisplayName = "HandleMouseWheel should scroll horizontally when specified")]
    public void HandleMouseWheelTest02()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollSpeed = 10f;
        component.ScrollOffset = new Vector2(100, 0); // Start with some horizontal scroll position

        // Act
        component.HandleMouseWheel(2f, isHorizontal: true);

        // Assert
        Assert.Equal(new Vector2(80, 0), component.ScrollOffset); // Should scroll left by 20 pixels
    }

    [Fact(DisplayName = "ResetScroll should set offset to zero")]
    public void ResetScrollTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ScrollOffset = new Vector2(100, 50);

        // Act
        component.ResetScroll();

        // Assert
        Assert.Equal(Vector2.Zero, component.ScrollOffset);
    }

    [Fact(DisplayName = "ScrollToVisible should make rectangle visible")]
    public void ScrollToVisibleTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        component.ScrollOffset = Vector2.Zero;

        // Act
        var targetRect = new Rectangle(500, 400, 100, 50);
        component.ScrollToVisible(targetRect);

        // Assert
        // Should scroll to make the rectangle visible
        Assert.True(component.ScrollOffset.X >= 200); // rect.Right - viewportWidth
        Assert.True(component.ScrollOffset.Y >= 150); // rect.Bottom - viewportHeight
    }

    [Fact(DisplayName = "OnScrollChanged event should fire when scroll offset changes")]
    public void OnScrollChangedTest01()
    {
        // Arrange
        var entity = new UIEntity("test");
        var component = entity.AddComponent<ScrollComponent>();
        component.ContentSize = new Vector2(1000, 800);
        component.ViewportSize = new Vector2(400, 300);
        var eventFired = false;
        Vector2 eventOffset = Vector2.Zero;

        component.OnScrollChanged += (sender, args) =>
        {
            eventFired = true;
            eventOffset = args.ScrollOffset;
        };

        // Act
        component.ScrollOffset = new Vector2(100, 50);

        // Assert
        Assert.True(eventFired);
        Assert.Equal(new Vector2(100, 50), eventOffset);
    }
}

public class ScrollChangedEventArgsTests
{
    [Fact(DisplayName = "Constructor should set ScrollOffset property")]
    public void ConstructorTest01()
    {
        // Arrange
        var offset = new Vector2(150, 100);

        // Act
        var args = new ScrollChangedEventArgs(offset);

        // Assert
        Assert.Equal(offset, args.ScrollOffset);
    }
}
