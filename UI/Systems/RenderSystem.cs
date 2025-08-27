using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Components;
using MonoGameUI.Core;

namespace MonoGameUI.Systems;

/// <summary>
/// System that processes RenderableComponent and StyleComponent to draw entities.
/// Handles batched rendering optimization and layer/depth sorting.
/// </summary>
public class RenderSystem : UISystem
{
    private SpriteBatch? _spriteBatch;
    private GraphicsDevice? _graphicsDevice;
    private readonly List<RenderItem> _renderItems = new();

    public override int Priority => 200; // Render after layout (100) but before validation (1000)

    /// <summary>
    /// Initialize the render system with graphics dependencies.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device for rendering.</param>
    /// <param name="spriteBatch">The sprite batch for efficient rendering.</param>
    public void Initialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
    }

    public override void Update(float deltaTime)
    {
        if (!Enabled || _spriteBatch == null || _graphicsDevice == null)
            return;

        // Collect all renderable entities that need drawing
        CollectRenderItems();

        // Sort by depth/layer for proper rendering order
        SortRenderItems();

        // Render all collected items
        RenderItems();

        // Clear render items for next frame
        _renderItems.Clear();
    }

    private void CollectRenderItems()
    {
        foreach (var entity in GetEntitiesWith<RenderableComponent, TransformComponent>())
        {
            // Skip if not dirty and not visible
            if (!entity.IsDirty(DirtyFlags.Render) && !IsVisible(entity))
                continue;

            var renderable = entity.GetComponent<RenderableComponent>()!;
            var transform = entity.GetComponent<TransformComponent>()!;

            // Skip if nothing to render
            if (renderable.RenderType == RenderType.None)
                continue;

            // Create render item
            var renderItem = new RenderItem
            {
                Entity = entity,
                Renderable = renderable,
                Transform = transform,
                Style = entity.GetComponent<StyleComponent>(), // Optional
                Depth = GetRenderDepth(entity),
                Bounds = transform.WorldBounds
            };

            _renderItems.Add(renderItem);
        }
    }

    private void SortRenderItems()
    {
        // Sort by depth (higher depth values render on top)
        _renderItems.Sort((a, b) => a.Depth.CompareTo(b.Depth));
    }

    private void RenderItems()
    {
        if (_renderItems.Count == 0) return;

        _spriteBatch!.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.None,
            rasterizerState: RasterizerState.CullNone);

        try
        {
            foreach (var item in _renderItems)
            {
                RenderItem(item);

                // Clear render dirty flag after successful render
                if (item.Entity.IsDirty(DirtyFlags.Render))
                {
                    item.Entity.DirtyFlags &= ~DirtyFlags.Render;
                }
            }
        }
        finally
        {
            _spriteBatch.End();
        }
    }

    private void RenderItem(RenderItem item)
    {
        var renderable = item.Renderable;
        var bounds = item.Bounds;

        // Apply style background if present
        if (item.Style != null)
        {
            RenderBackground(item.Style, bounds);
            RenderBorder(item.Style, bounds);
        }

        // Render the main content based on type
        switch (renderable.RenderType)
        {
            case RenderType.Sprite:
                RenderSprite(renderable, bounds);
                break;

            case RenderType.Text:
                RenderText(renderable, bounds);
                break;

            case RenderType.SolidColor:
                RenderSolidColor(renderable, bounds);
                break;

            case RenderType.NineSlice:
                RenderNineSlice(renderable, bounds);
                break;
        }
    }

    private void RenderBackground(StyleComponent style, Rectangle bounds)
    {
        if (style.BackgroundColor.A > 0)
        {
            // Create a 1x1 white pixel texture if we don't have one
            var pixelTexture = GetPixelTexture();
            if (pixelTexture != null)
            {
                _spriteBatch!.Draw(pixelTexture, bounds, style.BackgroundColor);
            }
        }
    }

    private void RenderBorder(StyleComponent style, Rectangle bounds)
    {
        if (style.BorderThickness.Left > 0 && style.BorderColor.A > 0)
        {
            var pixelTexture = GetPixelTexture();
            if (pixelTexture != null)
            {
                int borderWidth = (int)style.BorderThickness.Left;

                // Top border
                _spriteBatch!.Draw(pixelTexture,
                    new Rectangle(bounds.X, bounds.Y, bounds.Width, borderWidth),
                    style.BorderColor);

                // Bottom border
                _spriteBatch.Draw(pixelTexture,
                    new Rectangle(bounds.X, bounds.Bottom - borderWidth, bounds.Width, borderWidth),
                    style.BorderColor);

                // Left border
                _spriteBatch.Draw(pixelTexture,
                    new Rectangle(bounds.X, bounds.Y, borderWidth, bounds.Height),
                    style.BorderColor);

                // Right border
                _spriteBatch.Draw(pixelTexture,
                    new Rectangle(bounds.Right - borderWidth, bounds.Y, borderWidth, bounds.Height),
                    style.BorderColor);
            }
        }
    }

    private void RenderSprite(RenderableComponent renderable, Rectangle bounds)
    {
        if (renderable.Texture == null) return;

        _spriteBatch!.Draw(
            texture: renderable.Texture,
            destinationRectangle: bounds,
            sourceRectangle: renderable.SourceRectangle,
            color: renderable.Color,
            rotation: 0f, // No rotation for now
            origin: renderable.Origin,
            effects: renderable.Effects,
            layerDepth: 0f);
    }

    private void RenderText(RenderableComponent renderable, Rectangle bounds)
    {
        if (renderable.Font == null || string.IsNullOrEmpty(renderable.Text)) return;

        Vector2 position = new Vector2(bounds.X, bounds.Y);

        // Apply text alignment if needed (basic left-aligned for now)
        _spriteBatch!.DrawString(
            spriteFont: renderable.Font,
            text: renderable.Text,
            position: position,
            color: renderable.Color);
    }

    private void RenderSolidColor(RenderableComponent renderable, Rectangle bounds)
    {
        var pixelTexture = GetPixelTexture();
        if (pixelTexture != null)
        {
            _spriteBatch!.Draw(pixelTexture, bounds, renderable.Color);
        }
    }

    private void RenderNineSlice(RenderableComponent renderable, Rectangle bounds)
    {
        if (renderable.Texture == null) return;

        // For now, just render as a regular sprite
        // TODO: Implement proper 9-slice rendering logic
        RenderSprite(renderable, bounds);
    }

    private bool IsVisible(UIEntity entity)
    {
        var style = entity.GetComponent<StyleComponent>();
        return style?.Visible ?? true;
    }

    private float GetRenderDepth(UIEntity entity)
    {
        // Use depth calculation based on hierarchy for now
        // TODO: Add ZIndex property to StyleComponent if needed
        float depth = 0f;
        var current = entity;
        while (current.Parent != null)
        {
            depth += 0.01f; // Each level adds small depth increment
            current = current.Parent;
        }

        return depth;
    }

    private Texture2D? _pixelTexture;

    private Texture2D? GetPixelTexture()
    {
        if (_pixelTexture == null && _graphicsDevice != null)
        {
            _pixelTexture = new Texture2D(_graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }
        return _pixelTexture;
    }

    public override void OnDetached()
    {
        _pixelTexture?.Dispose();
        _pixelTexture = null;
        base.OnDetached();
    }
}

/// <summary>
/// Internal data structure for tracking render items during a frame.
/// </summary>
internal class RenderItem
{
    public UIEntity Entity { get; set; } = null!;
    public RenderableComponent Renderable { get; set; } = null!;
    public TransformComponent Transform { get; set; } = null!;
    public StyleComponent? Style { get; set; }
    public float Depth { get; set; }
    public Rectangle Bounds { get; set; }
}
