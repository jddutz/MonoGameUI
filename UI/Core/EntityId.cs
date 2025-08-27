using System;

namespace MonoGameUI.Core;

/// <summary>
/// Unique identifier for UI entities.
/// </summary>
public readonly struct EntityId : IEquatable<EntityId>
{
    private readonly uint _value;

    private EntityId(uint value)
    {
        _value = value;
    }

    public static EntityId New() => new(GenerateId());

    public static readonly EntityId None = new(0);

    private static uint _nextId = 1;
    private static uint GenerateId() => _nextId++;

    public bool Equals(EntityId other) => _value == other._value;
    public override bool Equals(object? obj) => obj is EntityId other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => $"Entity({_value})";

    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);
    public static bool operator !=(EntityId left, EntityId right) => !(left == right);
}
