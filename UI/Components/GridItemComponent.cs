using MonoGameUI.Core;

namespace MonoGameUI.Components;

/// <summary>
/// Component that specifies how a UI entity should be positioned within a GridLayoutComponent.
/// Allows explicit control over grid cell placement and spanning.
/// </summary>
public class GridItemComponent : Core.Component
{
    private int _column = -1;
    private int _row = -1;
    private int _columnSpan = 1;
    private int _rowSpan = 1;

    /// <summary>
    /// The column index for this grid item. If -1, will be auto-positioned.
    /// </summary>
    public int Column
    {
        get => _column;
        set
        {
            if (_column != value)
            {
                _column = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// The row index for this grid item. If -1, will be auto-positioned.
    /// </summary>
    public int Row
    {
        get => _row;
        set
        {
            if (_row != value)
            {
                _row = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Number of columns this item spans. Must be >= 1.
    /// </summary>
    public int ColumnSpan
    {
        get => _columnSpan;
        set
        {
            if (_columnSpan != value && value >= 1)
            {
                _columnSpan = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Number of rows this item spans. Must be >= 1.
    /// </summary>
    public int RowSpan
    {
        get => _rowSpan;
        set
        {
            if (_rowSpan != value && value >= 1)
            {
                _rowSpan = value;
                MarkDirty(DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Sets the grid position for this item.
    /// </summary>
    /// <param name="column">Column index (0-based)</param>
    /// <param name="row">Row index (0-based)</param>
    public void SetPosition(int column, int row)
    {
        bool changed = false;

        if (_column != column)
        {
            _column = column;
            changed = true;
        }

        if (_row != row)
        {
            _row = row;
            changed = true;
        }

        if (changed)
        {
            MarkDirty(DirtyFlags.Layout);
        }
    }

    /// <summary>
    /// Sets the grid span for this item.
    /// </summary>
    /// <param name="columnSpan">Number of columns to span</param>
    /// <param name="rowSpan">Number of rows to span</param>
    public void SetSpan(int columnSpan, int rowSpan)
    {
        bool changed = false;

        if (_columnSpan != columnSpan && columnSpan >= 1)
        {
            _columnSpan = columnSpan;
            changed = true;
        }

        if (_rowSpan != rowSpan && rowSpan >= 1)
        {
            _rowSpan = rowSpan;
            changed = true;
        }

        if (changed)
        {
            MarkDirty(DirtyFlags.Layout);
        }
    }
}
