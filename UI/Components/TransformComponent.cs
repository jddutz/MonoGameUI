using Microsoft.Xna.Framework;

namespace MonoGameUI.Components;

/// <summary>
/// Component that defines the position, size, rotation, and scale of a UI entity.
/// This is the fundamental component for all spatial calculations.
/// </summary>
public class TransformComponent : Core.Component
{
    private Vector2 _position = Vector2.Zero;
    private Vector2 _size = Vector2.Zero;
    private float _rotation = 0f;
    private Vector2 _scale = Vector2.One;
    private Vector2 _pivot = new(0.5f, 0.5f);

    /// <summary>
    /// Local position relative to parent (or screen if no parent).
    /// </summary>
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                MarkDirty(Core.DirtyFlags.Transform | Core.DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Size of the entity in pixels.
    /// </summary>
    public Vector2 Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                MarkDirty(Core.DirtyFlags.Transform | Core.DirtyFlags.Layout);
            }
        }
    }

    /// <summary>
    /// Rotation in radians around the pivot point.
    /// </summary>
    public float Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                MarkDirty(Core.DirtyFlags.Transform);
            }
        }
    }

    /// <summary>
    /// Scale factor applied to the entity.
    /// </summary>
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                MarkDirty(Core.DirtyFlags.Transform);
            }
        }
    }

    /// <summary>
    /// Pivot point for rotation and scaling (0,0 = top-left, 0.5,0.5 = center, 1,1 = bottom-right).
    /// </summary>
    public Vector2 Pivot
    {
        get => _pivot;
        set
        {
            if (_pivot != value)
            {
                _pivot = value;
                MarkDirty(Core.DirtyFlags.Transform);
            }
        }
    }

    /// <summary>
    /// Convenience property for X position.
    /// </summary>
    public float X
    {
        get => _position.X;
        set => Position = new Vector2(value, _position.Y);
    }

    /// <summary>
    /// Convenience property for Y position.
    /// </summary>
    public float Y
    {
        get => _position.Y;
        set => Position = new Vector2(_position.X, value);
    }

    /// <summary>
    /// Convenience property for width.
    /// </summary>
    public float Width
    {
        get => _size.X;
        set => Size = new Vector2(value, _size.Y);
    }

    /// <summary>
    /// Convenience property for height.
    /// </summary>
    public float Height
    {
        get => _size.Y;
        set => Size = new Vector2(_size.X, value);
    }

    /// <summary>
    /// Get the bounds rectangle in local coordinates.
    /// </summary>
    public Rectangle LocalBounds => new(0, 0, (int)_size.X, (int)_size.Y);

    /// <summary>
    /// Get the bounds rectangle in world coordinates.
    /// </summary>
    public Rectangle WorldBounds => new((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y);

    /// <summary>
    /// Get the transformation matrix for this transform.
    /// </summary>
    /// <returns>The transformation matrix.</returns>
    public Matrix GetTransformMatrix()
    {
        var pivotOffset = _pivot * _size;

        return Matrix.CreateTranslation(-pivotOffset.X, -pivotOffset.Y, 0) *
               Matrix.CreateScale(_scale.X, _scale.Y, 1) *
               Matrix.CreateRotationZ(_rotation) *
               Matrix.CreateTranslation(_position.X + pivotOffset.X, _position.Y + pivotOffset.Y, 0);
    }

    /// <summary>
    /// Get the world transformation matrix including parent transforms.
    /// </summary>
    /// <returns>The world transformation matrix.</returns>
    public Matrix GetWorldTransformMatrix()
    {
        var localMatrix = GetTransformMatrix();

        if (Entity?.Parent?.GetComponent<TransformComponent>() is TransformComponent parentTransform)
        {
            return localMatrix * parentTransform.GetWorldTransformMatrix();
        }

        return localMatrix;
    }

    /// <summary>
    /// Convert a point from local to world coordinates.
    /// </summary>
    /// <param name="localPoint">Point in local coordinates.</param>
    /// <returns>Point in world coordinates.</returns>
    public Vector2 LocalToWorld(Vector2 localPoint)
    {
        return Vector2.Transform(localPoint, GetWorldTransformMatrix());
    }

    /// <summary>
    /// Convert a point from world to local coordinates.
    /// </summary>
    /// <param name="worldPoint">Point in world coordinates.</param>
    /// <returns>Point in local coordinates.</returns>
    public Vector2 WorldToLocal(Vector2 worldPoint)
    {
        var worldMatrix = GetWorldTransformMatrix();
        Matrix.Invert(ref worldMatrix, out var inverseMatrix);
        return Vector2.Transform(worldPoint, inverseMatrix);
    }

    /// <summary>
    /// Check if a world point is within this entity's bounds.
    /// </summary>
    /// <param name="worldPoint">Point in world coordinates.</param>
    /// <returns>True if the point is within bounds.</returns>
    public bool ContainsPoint(Vector2 worldPoint)
    {
        var localPoint = WorldToLocal(worldPoint);
        return LocalBounds.Contains(localPoint);
    }
}
