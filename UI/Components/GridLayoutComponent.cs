using Microsoft.Xna.Framework;
using MonoGameUI.Core;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameUI.Components;

/// <summary>
/// Component that provides grid layout functionality for arranging child entities in a table-like structure.
/// Supports automatic and explicit column/row sizing, cell spanning, and gap spacing.
/// </summary>
public class GridLayoutComponent : Core.Component, ILayoutComponent
{
    private int _columns = 1;
    private int _rows = 1;
    private float _columnGap = 0f;
    private float _rowGap = 0f;
    private GridAutoFlow _autoFlow = GridAutoFlow.Row;
    private List<GridTrackSize> _columnSizes = new();
    private List<GridTrackSize> _rowSizes = new();

    /// <summary>
    /// Number of columns in the grid. Auto-calculated if not explicitly set.
    /// </summary>
    public int Columns
    {
        get => _columns;
        set
        {
            if (_columns != value && value > 0)
            {
                _columns = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Number of rows in the grid. Auto-calculated if not explicitly set.
    /// </summary>
    public int Rows
    {
        get => _rows;
        set
        {
            if (_rows != value && value > 0)
            {
                _rows = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Gap between columns in pixels.
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
    /// Gap between rows in pixels.
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
    /// Controls how auto-placed children are placed in the grid.
    /// </summary>
    public GridAutoFlow AutoFlow
    {
        get => _autoFlow;
        set
        {
            if (_autoFlow != value)
            {
                _autoFlow = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Explicit column size definitions. If empty, columns auto-size.
    /// </summary>
    public List<GridTrackSize> ColumnSizes
    {
        get => _columnSizes;
        set
        {
            _columnSizes = value ?? new List<GridTrackSize>();
            MarkDirty(DirtyFlags.Layout);
        }
    }

    /// <summary>
    /// Explicit row size definitions. If empty, rows auto-size.
    /// </summary>
    public List<GridTrackSize> RowSizes
    {
        get => _rowSizes;
        set
        {
            _rowSizes = value ?? new List<GridTrackSize>();
            MarkDirty(DirtyFlags.Layout);
        }
    }

    /// <summary>
    /// Sets a uniform gap for both columns and rows.
    /// </summary>
    public float Gap
    {
        set
        {
            ColumnGap = value;
            RowGap = value;
        }
    }

    /// <summary>
    /// Adds a column size definition.
    /// </summary>
    public void AddColumnSize(GridTrackSize size)
    {
        _columnSizes.Add(size);
        MarkDirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// Adds a row size definition.
    /// </summary>
    public void AddRowSize(GridTrackSize size)
    {
        _rowSizes.Add(size);
        MarkDirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// Removes all column size definitions, allowing auto-sizing.
    /// </summary>
    public void ClearColumnSizes()
    {
        _columnSizes.Clear();
        MarkDirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// Removes all row size definitions, allowing auto-sizing.
    /// </summary>
    public void ClearRowSizes()
    {
        _rowSizes.Clear();
        MarkDirty(DirtyFlags.Layout);
    }

    /// <summary>
    /// Performs grid layout calculation for child entities.
    /// </summary>
    public void PerformLayout(Rectangle availableSpace)
    {
        if (Entity?.Children == null || !Entity.Children.Any())
            return;

        var children = Entity.Children.ToList();
        var gridItems = new List<GridItem>();

        // Calculate grid dimensions
        int actualColumns = _columns;
        int actualRows = _rows;

        // Auto-calculate dimensions if needed
        if (actualColumns == 0 || actualRows == 0)
        {
            var childCount = children.Count;
            if (actualColumns == 0 && actualRows == 0)
            {
                // Calculate square-ish grid
                actualColumns = (int)System.Math.Ceiling(System.Math.Sqrt(childCount));
                actualRows = (int)System.Math.Ceiling((double)childCount / actualColumns);
            }
            else if (actualColumns == 0)
            {
                actualColumns = (int)System.Math.Ceiling((double)childCount / actualRows);
            }
            else
            {
                actualRows = (int)System.Math.Ceiling((double)childCount / actualColumns);
            }
        }

        // Create grid items with positioning
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var gridItemComponent = child.GetComponent<GridItemComponent>();

            GridItem item;
            if (gridItemComponent != null)
            {
                // Use explicit grid positioning
                item = new GridItem
                {
                    Entity = child,
                    Column = gridItemComponent.Column >= 0 ? gridItemComponent.Column : GetAutoColumn(i, actualColumns),
                    Row = gridItemComponent.Row >= 0 ? gridItemComponent.Row : GetAutoRow(i, actualColumns),
                    ColumnSpan = gridItemComponent.ColumnSpan,
                    RowSpan = gridItemComponent.RowSpan
                };
            }
            else
            {
                // Use auto-positioning
                item = new GridItem
                {
                    Entity = child,
                    Column = GetAutoColumn(i, actualColumns),
                    Row = GetAutoRow(i, actualColumns),
                    ColumnSpan = 1,
                    RowSpan = 1
                };
            }
            gridItems.Add(item);
        }

        // Calculate column and row sizes
        var columnWidths = CalculateTrackSizes(_columnSizes, actualColumns, availableSpace.Width, _columnGap);
        var rowHeights = CalculateTrackSizes(_rowSizes, actualRows, availableSpace.Height, _rowGap);

        // Position grid items
        for (int i = 0; i < gridItems.Count; i++)
        {
            var item = gridItems[i];
            var transform = item.Entity.GetComponent<TransformComponent>();
            if (transform == null) continue;

            // Calculate position
            float x = availableSpace.X;
            for (int col = 0; col < item.Column; col++)
            {
                x += columnWidths[col] + _columnGap;
            }

            float y = availableSpace.Y;
            for (int row = 0; row < item.Row; row++)
            {
                y += rowHeights[row] + _rowGap;
            }

            // Calculate size
            float width = 0;
            for (int col = item.Column; col < System.Math.Min(item.Column + item.ColumnSpan, actualColumns); col++)
            {
                if (col > item.Column) width += _columnGap;
                width += columnWidths[col];
            }

            float height = 0;
            for (int row = item.Row; row < System.Math.Min(item.Row + item.RowSpan, actualRows); row++)
            {
                if (row > item.Row) height += _rowGap;
                height += rowHeights[row];
            }

            // Apply transform
            transform.Position = new Vector2(x, y);
            var layout = item.Entity.GetComponent<LayoutComponent>();
            if (layout != null)
            {
                // Set the transform size since LayoutComponent doesn't have direct Width/Height properties
                transform.Size = new Vector2(width, height);
                item.Entity.MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    private int GetAutoColumn(int index, int columns)
    {
        return _autoFlow == GridAutoFlow.Row ? index % columns : index / columns;
    }

    private int GetAutoRow(int index, int columns)
    {
        return _autoFlow == GridAutoFlow.Row ? index / columns : index % columns;
    }

    private float[] CalculateTrackSizes(List<GridTrackSize> trackSizes, int trackCount, float availableSize, float gap)
    {
        var sizes = new float[trackCount];
        var totalGap = gap * (trackCount - 1);
        var remainingSpace = availableSize - totalGap;

        if (!trackSizes.Any())
        {
            // Auto-size all tracks equally
            var trackSize = remainingSpace / trackCount;
            for (int i = 0; i < trackCount; i++)
            {
                sizes[i] = trackSize;
            }
        }
        else
        {
            // Use explicit sizes where available
            var autoTracks = 0;
            var usedSpace = 0f;

            for (int i = 0; i < trackCount; i++)
            {
                if (i < trackSizes.Count)
                {
                    var trackSize = trackSizes[i];
                    if (trackSize.Type == GridTrackSizeType.Pixels)
                    {
                        sizes[i] = trackSize.Value;
                        usedSpace += trackSize.Value;
                    }
                    else if (trackSize.Type == GridTrackSizeType.Percentage)
                    {
                        sizes[i] = remainingSpace * (trackSize.Value / 100f);
                        usedSpace += sizes[i];
                    }
                    else // Auto
                    {
                        autoTracks++;
                    }
                }
                else
                {
                    autoTracks++;
                }
            }

            // Distribute remaining space to auto tracks
            if (autoTracks > 0)
            {
                var autoSize = System.Math.Max(0, (remainingSpace - usedSpace) / autoTracks);
                for (int i = 0; i < trackCount; i++)
                {
                    if (i >= trackSizes.Count || trackSizes[i].Type == GridTrackSizeType.Auto)
                    {
                        sizes[i] = autoSize;
                    }
                }
            }
        }

        return sizes;
    }

    /// <summary>
    /// Calculates the desired size for this grid layout based on its children.
    /// </summary>
    /// <param name="availableSize">Available space for the layout.</param>
    /// <returns>Desired size of the grid.</returns>
    public Vector2 CalculateDesiredSize(Vector2 availableSize)
    {
        if (Entity?.Children == null || !Entity.Children.Any())
        {
            return Vector2.Zero;
        }

        var children = Entity.Children.ToList();
        if (!children.Any())
        {
            return Vector2.Zero;
        }

        // Calculate grid dimensions
        int actualColumns = _columns > 0 ? _columns : (int)System.Math.Ceiling(System.Math.Sqrt(children.Count));
        int actualRows = _rows > 0 ? _rows : (int)System.Math.Ceiling((double)children.Count / actualColumns);

        // For desired size calculation, we'll use a simplified approach
        // In a real implementation, this would calculate based on child desired sizes
        var totalGapWidth = _columnGap * (actualColumns - 1);
        var totalGapHeight = _rowGap * (actualRows - 1);

        // Estimate size based on children transforms
        float averageChildWidth = 100f; // Default fallback
        float averageChildHeight = 50f; // Default fallback

        if (children.Any())
        {
            var childSizes = children
                .Select(c => c.GetComponent<TransformComponent>())
                .Where(t => t != null)
                .Select(t => t!.Size)
                .Where(s => s.X > 0 && s.Y > 0)
                .ToList();

            if (childSizes.Any())
            {
                averageChildWidth = childSizes.Average(s => s.X);
                averageChildHeight = childSizes.Average(s => s.Y);
            }
        }

        var desiredWidth = (averageChildWidth * actualColumns) + totalGapWidth;
        var desiredHeight = (averageChildHeight * actualRows) + totalGapHeight;

        return new Vector2(
            System.Math.Min(desiredWidth, availableSize.X),
            System.Math.Min(desiredHeight, availableSize.Y));
    }

    /// <summary>
    /// Arranges the grid items within the specified bounds.
    /// </summary>
    /// <param name="bounds">The final bounds for the grid container.</param>
    public void Arrange(Rectangle bounds)
    {
        PerformLayout(bounds);
    }

    private class GridItem
    {
        public required UIEntity Entity { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int ColumnSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
    }
}

/// <summary>
/// Defines how auto-placed grid items flow in the grid.
/// </summary>
public enum GridAutoFlow
{
    /// <summary>
    /// Fill each row in turn, creating new rows as needed.
    /// </summary>
    Row,

    /// <summary>
    /// Fill each column in turn, creating new columns as needed.
    /// </summary>
    Column
}

/// <summary>
/// Defines the size of a grid track (row or column).
/// </summary>
public class GridTrackSize
{
    public GridTrackSizeType Type { get; set; }
    public float Value { get; set; }

    /// <summary>
    /// Creates an auto-sized track.
    /// </summary>
    public static GridTrackSize Auto() => new() { Type = GridTrackSizeType.Auto };

    /// <summary>
    /// Creates a fixed pixel-sized track.
    /// </summary>
    public static GridTrackSize Pixels(float pixels) => new() { Type = GridTrackSizeType.Pixels, Value = pixels };

    /// <summary>
    /// Creates a percentage-sized track.
    /// </summary>
    public static GridTrackSize Percentage(float percentage) => new() { Type = GridTrackSizeType.Percentage, Value = percentage };
}

/// <summary>
/// Types of grid track sizing.
/// </summary>
public enum GridTrackSizeType
{
    /// <summary>
    /// Size based on content (distribute remaining space equally).
    /// </summary>
    Auto,

    /// <summary>
    /// Fixed size in pixels.
    /// </summary>
    Pixels,

    /// <summary>
    /// Percentage of available space.
    /// </summary>
    Percentage
}
