using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUI.Components;
using MonoGameUI.Core;

namespace MonoGameUI.Entities;

public static class EntityFactory
{

    /// <summary>
    /// Create a grid layout container.
    /// </summary>
    /// <param name="columns">Number of columns (0 for auto).</param>
    /// <param name="rows">Number of rows (0 for auto).</param>
    /// <param name="columnGap">Gap between columns.</param>
    /// <param name="rowGap">Gap between rows.</param>
    /// <param name="autoFlow">How auto-placed children flow in the grid.</param>
    /// <returns>A grid layout entity.</returns>
    public static UIEntity CreateGridLayout(
        int columns = 0,
        int rows = 0,
        float columnGap = 0f,
        float rowGap = 0f,
        GridAutoFlow autoFlow = GridAutoFlow.Row)
    {
        var entity = CreateContainer("GridLayout");

        // Add grid layout component
        var gridLayout = entity.AddComponent<GridLayoutComponent>();
        if (columns > 0) gridLayout.Columns = columns;
        if (rows > 0) gridLayout.Rows = rows;
        gridLayout.ColumnGap = columnGap;
        gridLayout.RowGap = rowGap;
        gridLayout.AutoFlow = autoFlow;

        return entity;
    }

    /// <summary>
    /// Create a scrollable view container.
    /// </summary>
    /// <param name="horizontalScroll">Enable horizontal scrolling.</param>
    /// <param name="verticalScroll">Enable vertical scrolling.</param>
    /// <param name="scrollSpeed">Scroll speed for mouse wheel.</param>
    /// <param name="horizontalScrollbarMode">Horizontal scrollbar display mode.</param>
    /// <param name="verticalScrollbarMode">Vertical scrollbar display mode.</param>
    /// <returns>A scroll view entity.</returns>
    public static UIEntity CreateScrollView(
        bool horizontalScroll = false,
        bool verticalScroll = true,
        float scrollSpeed = 20f,
        ScrollBarMode horizontalScrollbarMode = ScrollBarMode.Auto,
        ScrollBarMode verticalScrollbarMode = ScrollBarMode.Auto)
    {
        var entity = CreateContainer("ScrollView");

        // Add scroll component
        var scroll = entity.AddComponent<ScrollComponent>();
        scroll.HorizontalScrollEnabled = horizontalScroll;
        scroll.VerticalScrollEnabled = verticalScroll;
        scroll.ScrollSpeed = scrollSpeed;
        scroll.HorizontalScrollbarMode = horizontalScrollbarMode;
        scroll.VerticalScrollbarMode = verticalScrollbarMode;

        return entity;
    }

    /// <summary>
    /// Create a grid item with specific grid positioning.
    /// </summary>
    /// <param name="column">Column position (-1 for auto).</param>
    /// <param name="row">Row position (-1 for auto).</param>
    /// <param name="columnSpan">Number of columns to span.</param>
    /// <param name="rowSpan">Number of rows to span.</param>
    /// <returns>A grid item entity.</returns>
    public static UIEntity CreateGridItem(
        int column = -1,
        int row = -1,
        int columnSpan = 1,
        int rowSpan = 1)
    {
        var entity = CreateControl("GridItem");

        // Add grid item component
        var gridItem = entity.AddComponent<GridItemComponent>();
        gridItem.Column = column;
        gridItem.Row = row;
        gridItem.ColumnSpan = columnSpan;
        gridItem.RowSpan = rowSpan;

        return entity;
    }
    /// <summary>
    /// Create a basic UI entity with transform and layout components.
    /// </summary>
    /// <param name="name">Optional name for the entity.</param>
    /// <returns>A new UIEntity with basic components.</returns>
    public static UIEntity CreateBasicEntity(string name = "")
    {
        var entity = new UIEntity(name.Length > 0 ? name : $"Entity_{Guid.NewGuid():N}"[..16]);

        // Add fundamental components
        entity.AddComponent<TransformComponent>();
        entity.AddComponent<LayoutComponent>();

        return entity;
    }

    /// <summary>
    /// Create a button entity with interactive behavior.
    /// This replaces the legacy Button, TextButton, and ImageButton classes.
    /// </summary>
    /// <param name="text">Text to display on the button.</param>
    /// <param name="font">Font for the button text.</param>
    /// <returns>A fully configured button entity.</returns>
    public static UIEntity CreateButton(string text = "Button", SpriteFont? font = null)
    {
        var entity = CreateBasicEntity($"Button_{text}");

        // Add button-specific components
        var renderable = entity.AddComponent<RenderableComponent>();
        var input = entity.AddComponent<InputComponent>();
        var style = entity.AddComponent<StyleComponent>();
        var button = entity.AddComponent<ButtonComponent>();

        // Configure for button behavior
        if (font != null)
        {
            renderable.SetText(text, font, Color.Black);
        }
        else
        {
            renderable.RenderType = RenderType.Text;
            renderable.Text = text;
            renderable.Color = Color.Black;
        }

        // Set up default button styling
        style.BackgroundColor = Color.LightGray;
        style.BorderColor = Color.DarkGray;
        style.BorderThickness = new Thickness(1);
        style.Padding = new Thickness(8, 4);

        // Configure input handling
        input.AcceptsMouseInput = true;
        input.CanReceiveFocus = true;

        // Set default size behavior
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.WidthMode = Components.SizeMode.FitContent;
        layout.HeightMode = Components.SizeMode.FitContent;

        // Set up state change handling to update appearance
        button.StateChanged += (oldState, newState) => button.ApplyStateToComponents();

        return entity;
    }

    /// <summary>
    /// Create a text label entity for displaying text.
    /// This replaces the legacy TextLabel class.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="font">Font for the text.</param>
    /// <param name="color">Text color.</param>
    /// <returns>A text label entity.</returns>
    public static UIEntity CreateTextLabel(string text = "Label", SpriteFont? font = null, Color? color = null)
    {
        var entity = CreateBasicEntity($"Label_{text}");

        // Add text rendering
        var renderable = entity.AddComponent<RenderableComponent>();
        var style = entity.AddComponent<StyleComponent>();

        // Configure text rendering
        if (font != null)
        {
            renderable.SetText(text, font, color ?? Color.Black);
        }
        else
        {
            renderable.RenderType = RenderType.Text;
            renderable.Text = text;
            renderable.Color = color ?? Color.Black;
        }

        // Set default size behavior
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.WidthMode = Components.SizeMode.FitContent;
        layout.HeightMode = Components.SizeMode.FitContent;

        return entity;
    }

    /// <summary>
    /// Create an image entity for displaying sprites.
    /// This replaces the legacy ImageRect class.
    /// </summary>
    /// <param name="texture">Texture to display.</param>
    /// <param name="color">Tint color.</param>
    /// <returns>An image entity.</returns>
    public static UIEntity CreateImage(Texture2D? texture = null, Color? color = null)
    {
        var entity = CreateBasicEntity("Image");

        // Add image rendering
        var renderable = entity.AddComponent<RenderableComponent>();
        var style = entity.AddComponent<StyleComponent>();

        // Configure sprite rendering
        if (texture != null)
        {
            renderable.SetSprite(texture, color ?? Color.White);
        }
        else
        {
            renderable.RenderType = RenderType.Sprite;
            renderable.Color = color ?? Color.White;
        }

        // Set default size behavior
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.WidthMode = Components.SizeMode.Fixed;
        layout.HeightMode = Components.SizeMode.Fixed;

        // Set size based on texture if available
        if (texture != null)
        {
            var transform = entity.GetComponent<TransformComponent>()!;
            transform.Size = new Vector2(texture.Width, texture.Height);
        }

        return entity;
    }

    /// <summary>
    /// Create a container entity for holding child elements.
    /// This replaces the legacy Container class.
    /// </summary>
    /// <param name="name">Name for the container.</param>
    /// <returns>A container entity.</returns>
    public static UIEntity CreateContainer(string name = "Container")
    {
        var entity = CreateBasicEntity(name);

        // Add optional styling
        var style = entity.AddComponent<StyleComponent>();

        // Set default size behavior
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.WidthMode = Components.SizeMode.Fill;
        layout.HeightMode = Components.SizeMode.Fill;

        return entity;
    }

    /// <summary>
    /// Create a control entity with input handling capabilities.
    /// This is the foundation for interactive UI elements.
    /// </summary>
    /// <param name="name">Name for the control.</param>
    /// <returns>A control entity.</returns>
    public static UIEntity CreateControl(string name = "Control")
    {
        var entity = CreateBasicEntity(name);

        // Add input and styling components
        var input = entity.AddComponent<InputComponent>();
        var style = entity.AddComponent<StyleComponent>();

        // Configure for control behavior
        input.AcceptsMouseInput = true;
        input.CanReceiveFocus = true;

        // Set default size behavior
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.WidthMode = Components.SizeMode.Fixed;
        layout.HeightMode = Components.SizeMode.Fixed;

        return entity;
    }

    /// <summary>
    /// Create a panel entity with background and border styling.
    /// This provides a styled container with visual appearance.
    /// </summary>
    /// <param name="backgroundColor">Background color for the panel.</param>
    /// <param name="borderColor">Border color for the panel.</param>
    /// <param name="borderThickness">Border thickness.</param>
    /// <returns>A styled panel entity.</returns>
    public static UIEntity CreatePanel(Color? backgroundColor = null, Color? borderColor = null, float borderThickness = 1f)
    {
        var entity = CreateContainer("Panel");

        // Configure styling
        var style = entity.GetComponent<StyleComponent>()!;
        style.BackgroundColor = backgroundColor ?? Color.White;
        style.BorderColor = borderColor ?? Color.Gray;
        style.BorderThickness = new Thickness(borderThickness);
        style.Padding = new Thickness(4);

        return entity;
    }

    /// <summary>
    /// Create a horizontal layout container.
    /// This replaces the legacy HorizontalLayoutPanel class.
    /// </summary>
    /// <param name="spacing">Spacing between child elements.</param>
    /// <returns>A horizontal layout entity.</returns>
    public static UIEntity CreateHorizontalLayout(float spacing = 0f)
    {
        var entity = CreateContainer("HorizontalLayout");

        // Configure existing layout component
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.FlexDirection = FlexDirection.Row;
        layout.Gap = spacing;

        return entity;
    }

    /// <summary>
    /// Create a vertical layout container.
    /// This replaces the legacy VerticalLayoutPanel class.
    /// </summary>
    /// <param name="spacing">Spacing between child elements.</param>
    /// <returns>A vertical layout entity.</returns>
    public static UIEntity CreateVerticalLayout(float spacing = 0f)
    {
        var entity = CreateContainer("VerticalLayout");

        // Configure existing layout component
        var layout = entity.GetComponent<LayoutComponent>()!;
        layout.FlexDirection = FlexDirection.Column;
        layout.Gap = spacing;

        return entity;
    }

    /// <summary>
    /// Create a flexible layout container with custom configuration.
    /// </summary>
    /// <param name="direction">Layout direction.</param>
    /// <param name="justifyContent">Main axis alignment.</param>
    /// <param name="alignItems">Cross axis alignment.</param>
    /// <param name="gap">Gap between items.</param>
    /// <returns>A flex layout entity.</returns>
    public static UIEntity CreateFlexLayout(
        FlexDirection direction = FlexDirection.Row,
        JustifyContent justifyContent = JustifyContent.FlexStart,
        AlignItems alignItems = AlignItems.Stretch,
        float gap = 0f)
    {
        var entity = CreateContainer("FlexLayout");

        // Add flex layout component
        var flexLayout = entity.AddComponent<FlexLayoutComponent>();
        flexLayout.FlexDirection = direction;
        flexLayout.JustifyContent = justifyContent;
        flexLayout.AlignItems = alignItems;
        flexLayout.Gap = gap;

        return entity;
    }

    /// <summary>
    /// Create an image button with both sprite and text.
    /// </summary>
    /// <param name="texture">Button background texture.</param>
    /// <param name="text">Button text.</param>
    /// <param name="font">Font for the text.</param>
    /// <returns>An image button entity.</returns>
    public static UIEntity CreateImageButton(Texture2D? texture = null, string text = "Button", SpriteFont? font = null)
    {
        var button = CreateButton(text, font);

        // If texture is provided, set up sprite rendering
        if (texture != null)
        {
            var renderable = button.GetComponent<RenderableComponent>()!;
            // For image buttons, we might want to render both sprite and text
            // For now, prioritize the sprite
            renderable.SetSprite(texture, Color.White);
        }

        return button;
    }

    /// <summary>
    /// Apply common configuration to an entity.
    /// </summary>
    /// <param name="entity">Entity to configure.</param>
    /// <param name="position">Position to set.</param>
    /// <param name="size">Size to set.</param>
    /// <param name="visible">Visibility state.</param>
    /// <returns>The configured entity for method chaining.</returns>
    public static UIEntity Configure(this UIEntity entity, Vector2? position = null, Vector2? size = null, bool visible = true)
    {
        if (entity.GetComponent<TransformComponent>() is TransformComponent transform)
        {
            if (position.HasValue)
                transform.Position = position.Value;
            if (size.HasValue)
                transform.Size = size.Value;
        }

        if (entity.GetComponent<StyleComponent>() is StyleComponent style)
        {
            style.Visible = visible;
        }

        return entity;
    }
}
