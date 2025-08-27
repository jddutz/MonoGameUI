# Component System Design

## Data Binding System

### Observable Properties:

```csharp
public class ObservableProperty<T> : INotifyPropertyChanged
{
    private T _value;
    public T Value
    {
        get => _value;
        set => SetValue(value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // Binding support
    public IDisposable Bind(Action<T> callback)
    public static implicit operator T(ObservableProperty<T> prop)
}
```

### Data Context:

```csharp
public interface IDataContext
{
    object? GetValue(string path);
    void SetValue(string path, object? value);
    IDisposable Subscribe(string path, Action<object?> callback);
}
```

## Theme System

### Component-Based Theming:

```csharp
public class ThemeRule
{
    public ComponentSelector Selector { get; set; }
    public Dictionary<string, object> Properties { get; set; }
    public int Priority { get; set; }
}

public class ComponentSelector
{
    public Type? ComponentType { get; set; }
    public string? Name { get; set; }
    public string? State { get; set; }
    public List<string> Tags { get; set; }
}
```

### Cascading Styles:

- Inherit from parent entities
- Component-specific overrides
- State-based styling (hover, active, disabled)
- Tag-based targeting

## Layout System

### FlexBox-Inspired Layout:

```csharp
public class FlexLayoutComponent : IComponent
{
    public FlexDirection Direction { get; set; } = FlexDirection.Row;
    public FlexWrap Wrap { get; set; } = FlexWrap.NoWrap;
    public FlexJustifyContent JustifyContent { get; set; } = FlexJustifyContent.Start;
    public FlexAlignItems AlignItems { get; set; } = FlexAlignItems.Stretch;
    public FlexAlignContent AlignContent { get; set; } = FlexAlignContent.Start;
    public float Gap { get; set; } = 0;
}

public class FlexItemComponent : IComponent
{
    public float FlexGrow { get; set; } = 0;
    public float FlexShrink { get; set; } = 1;
    public FlexBasis FlexBasis { get; set; } = FlexBasis.Auto;
    public FlexAlignSelf AlignSelf { get; set; } = FlexAlignSelf.Auto;
    public int Order { get; set; } = 0;
}
```

## Input System

### Event-Based Input:

```csharp
public interface IInputComponent : IComponent
{
    bool CanReceiveInput { get; }
    InputPriority Priority { get; }
    bool HandleInput(InputEvent inputEvent);
}

public abstract class InputEvent
{
    public InputDevice Device { get; }
    public TimeSpan Timestamp { get; }
    public bool Handled { get; set; }
}

// Specific input events
public class MouseInputEvent : InputEvent
public class TouchInputEvent : InputEvent
public class KeyboardInputEvent : InputEvent
public class GamepadInputEvent : InputEvent
```

## Animation System

### Timeline-Based Animations:

```csharp
public class AnimationComponent : IComponent
{
    public List<AnimationTrack> Tracks { get; }
    public AnimationState State { get; set; }
    public float Time { get; set; }
    public float Duration { get; set; }
    public bool Loop { get; set; }
}

public class AnimationTrack
{
    public string PropertyPath { get; set; }
    public List<Keyframe> Keyframes { get; }
    public EasingFunction Easing { get; set; }
}
```

## Resource Management

### Asset Loading:

```csharp
public interface IAssetLoader
{
    Task<T> LoadAsync<T>(string path) where T : class;
    T Load<T>(string path) where T : class;
    void Unload(string path);
}

// UI-specific assets
public class UIAssetManager
{
    public TextureAtlas SpriteAtlas { get; }
    public FontCollection Fonts { get; }
    public ThemeCollection Themes { get; }
}
```
