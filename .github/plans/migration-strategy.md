# Migration Strategy & Implementation Plan

## Current State Assessment (as of August 26, 2025)

**🎉 RECENT COMPLETION**: RenderSystem and InputSystem fully implemented and tested (August 26, 2025)

✅ **Phase 1 Complete**: Core infrastructure implemented and tested

- UIEntity, ComponentCollection, PropertyBag, DirtyFlags
- TransformComponent, LayoutComponent
- UISystem, UIWorld architecture
- 168 passing tests covering new architecture (was 156)

✅ **Phase 2A COMPLETE**: Core rendering & input components implemented

- ✅ RenderableComponent (sprites, text, shapes, 9-slice) + dedicated tests
- ✅ StyleComponent (backgrounds, borders, margins, padding) + dedicated tests
- ✅ InputComponent (mouse, keyboard, touch, gamepad events) + dedicated tests
- ✅ ButtonComponent (interactive button behavior with states) + dedicated tests
- ✅ EntityFactory (high-level creation APIs)
- ✅ LayoutSystem, ValidationSystem, UIWorld
- ✅ **NEW** RenderSystem (processes RenderableComponent for drawing)
- ✅ **NEW** InputSystem (processes InputComponent for events & focus)

✅ **Phase 2B STARTED**: Advanced layout components

- ✅ FlexLayoutComponent (CSS Flexbox-inspired layout) + comprehensive tests

**Missing Phase 2B Components**: Still need to implement

- GridLayoutComponent, ScrollComponent
- TextInputComponent, SliderComponent, ToggleComponent
- RenderSystem, InputSystem for processing

❌ **Legacy System**: Still contains inheritance-based classes that need migration

- `Element` (base class) → Convert to components
- `Container`, `Control` → Convert to component compositions
- `Button`, `TextButton`, `ImageButton` → Convert to ButtonComponent + others
- `HorizontalLayoutPanel`, `VerticalLayoutPanel` → Convert to FlexLayoutComponent
- `DropdownMenu`, `ToggleButton`, etc. → Convert to specialized components

## Migration Phases

### Phase 2A: Core Rendering & Input Components ✅ **COMPLETE**

**Week 1-2: Basic Rendering** ✅ **COMPLETE**

1. ✅ **DONE** Implement `RenderableComponent` + comprehensive tests

   - ✅ Sprite rendering with texture, color, effects
   - ✅ Text rendering with font, alignment, wrapping
   - ✅ Shape rendering (rectangles, circles, lines)
   - ✅ 9-slice sprite support for UI panels

2. ✅ **DONE** Implement `StyleComponent` + comprehensive tests

   - ✅ Background colors, borders, shadows
   - ✅ Margin, padding properties
   - ✅ Theme integration hooks

3. ✅ **DONE** Implement `VisibilityComponent` (integrated into StyleComponent)
   - ✅ Show/hide functionality
   - ✅ Opacity/alpha blending
   - ✅ Culling optimization

**Week 3-4: Input System** ✅ **COMPLETE**

4. ✅ **DONE** Implement `InputComponent` + comprehensive tests

- ✅ Mouse event handling (click, hover, drag)
- ✅ Touch event support
- ✅ Keyboard focus and input
- ✅ Gamepad navigation

5. ✅ **DONE** Implement `InteractableComponent/ButtonComponent` + comprehensive tests
   - ✅ Button-like behavior
   - ✅ State management (normal, hover, pressed, disabled)
   - ✅ Event delegation and state changes

**Still Missing:**
✅ **COMPLETE** RenderSystem (for processing RenderableComponent drawing)
✅ **COMPLETE** InputSystem (for processing InputComponent events)

### Phase 2B: Layout & Control Components (NEXT PRIORITY - Weeks 5-8)

**Week 5-6: Advanced Layout**

6. ✅ **COMPLETE** Implement `FlexLayoutComponent`

   - ✅ CSS Flexbox-inspired layout with full property support
   - ✅ Direction (Row, RowReverse, Column, ColumnReverse)
   - ✅ JustifyContent (FlexStart, FlexEnd, Center, SpaceBetween, SpaceAround, SpaceEvenly)
   - ✅ AlignItems (Stretch, FlexStart, FlexEnd, Center, Baseline)
   - ✅ FlexWrap support (NoWrap, Wrap, WrapReverse)
   - ✅ Gap properties (Gap, RowGap, ColumnGap)
   - ✅ 14 comprehensive tests covering all functionality
   - ✅ EntityFactory integration (CreateFlexLayout method)
   - ✅ Replaces HorizontalLayoutPanel/VerticalLayoutPanel functionality

7. ❌ **TODO** Implement `GridLayoutComponent`

   - Table-style grid layout
   - Column/row spanning
   - Auto-sizing capabilities

8. ❌ **TODO** Implement `ScrollComponent`
   - Scrollable content areas
   - Scroll bars and wheel support
   - Virtualization for performance

**Week 7-8: Common Controls**

9. ❌ **TODO** Implement `TextInputComponent`

   - Text entry and editing
   - Selection, cursor, validation
   - Placeholder text support

10. ❌ **TODO** Implement `SliderComponent`
    - Value selection with drag
    - Horizontal/vertical orientation
    - Min/max/step configuration

**Week 7-8: Essential Systems** ✅ **COMPLETE**

11. ✅ **COMPLETE** Implement `RenderSystem`

    - ✅ Process RenderableComponent drawing
    - ✅ Batch rendering optimization
    - ✅ Layer/depth sorting
    - ✅ Background and border rendering from StyleComponent
    - ✅ Multiple render types: Sprite, Text, SolidColor, NineSlice
    - ✅ 6 comprehensive tests covering functionality

12. ✅ **COMPLETE** Implement `InputSystem`
    - ✅ Process InputComponent events
    - ✅ Input routing and hit testing
    - ✅ Focus management with SetFocus API
    - ✅ Mouse event handling (enter, exit, move, down, up, click, wheel)
    - ✅ Keyboard event handling with proper focus behavior
    - ✅ Input priority and depth sorting for correct event routing
    - ✅ 6 comprehensive tests covering focus management and event handling

**📈 Test Count**: 168 passing tests (up from 156)

### Phase 3: Legacy Class Migration (Weeks 9-12)

**Migration Priority Order:**

1. **Elements** (Week 9)

   - `Container` → UIEntity + FlexLayoutComponent
   - `Control` → UIEntity + InputComponent + StyleComponent
   - `Element` → UIEntity + TransformComponent + RenderableComponent

2. **Simple Controls** (Week 10)

   - `TextButton` → UIEntity + ButtonComponent + RenderableComponent(text)
   - `ImageButton` → UIEntity + ButtonComponent + RenderableComponent(sprite)
   - `TextLabel` → UIEntity + RenderableComponent(text) + StyleComponent
   - `ImageRect` → UIEntity + RenderableComponent(sprite) + StyleComponent

3. **Layout Panels** (Week 11) - **READY FOR MIGRATION**

   - `HorizontalLayoutPanel` → UIEntity + FlexLayoutComponent(row) ✅ **COMPONENT READY**
   - `VerticalLayoutPanel` → UIEntity + FlexLayoutComponent(column) ✅ **COMPONENT READY**

4. **Complex Controls** (Week 12)
   - `DropdownMenu` → UIEntity + ButtonComponent + MenuComponent
   - `ToggleButton` → UIEntity + ButtonComponent + ToggleComponent
   - `ToggleButtonGroup` → UIEntity + GroupComponent

### Phase 4: API Compatibility & Migration Tools (Weeks 13-16)

**Week 13-14: Compatibility Layer**

- Create adapter classes that wrap new components with old API
- Maintain backward compatibility for existing user code
- Implement conversion helpers for common patterns

**Week 15-16: Migration Tools**

- Automated code analysis and conversion tools
- Migration guides and documentation
- Performance comparison benchmarks

## Detailed Migration Strategy

### 1. **Parallel Implementation Approach**

```csharp
// New system runs alongside old
namespace MonoGameUI.Elements; // Old system (deprecated)
namespace MonoGameUI.Components; // New system
namespace MonoGameUI.Entities; // New high-level entity factories
```

### 2. **Component-First Design**

```csharp
// Instead of Button : Control : Element
// Use composition:
var button = new UIEntity("Button");
button.AddComponent<TransformComponent>();
button.AddComponent<LayoutComponent>();
button.AddComponent<RenderableComponent>();
button.AddComponent<ButtonComponent>();
button.AddComponent<StyleComponent>();
```

### 3. **Legacy Wrapper Pattern**

```csharp
// Maintain old API while using new system internally
public class Button : ILegacyElement
{
    private UIEntity _entity;
    private ButtonComponent _button;

    public Button()
    {
        _entity = EntityFactory.CreateButton();
        _button = _entity.GetComponent<ButtonComponent>();
    }

    // Legacy properties delegate to new components
    public string Text
    {
        get => _entity.GetComponent<RenderableComponent>().Text;
        set => _entity.GetComponent<RenderableComponent>().Text = value;
    }
}
```

### 4. **Incremental Conversion Process**

**Step 1**: Create component equivalent
**Step 2**: Implement entity factory method
**Step 3**: Create compatibility wrapper
**Step 4**: Add migration unit tests
**Step 5**: Update documentation
**Step 6**: Mark old class as [Obsolete]

### 5. **Migration Validation**

Each migrated class must:

- ✅ Pass all existing unit tests through compatibility layer
- ✅ Demonstrate performance improvement or parity
- ✅ Support all original features
- ✅ Provide clear migration path for users

## Migration Testing Strategy

### Test Categories

**1. Unit Tests for New Components**

```csharp
[Test]
public void RenderableComponent_SetsTextCorrectly()
{
    var entity = new UIEntity();
    var renderable = entity.AddComponent<RenderableComponent>();

    renderable.Text = "Hello World";

    Assert.AreEqual("Hello World", renderable.Text);
    Assert.IsTrue(entity.DirtyFlags.HasFlag(DirtyFlags.Render));
}
```

**2. Integration Tests**

```csharp
[Test]
public void EntityFactory_CreateButton_CreatesWorkingButton()
{
    var button = EntityFactory.CreateButton("Test");

    Assert.IsNotNull(button.GetComponent<TransformComponent>());
    Assert.IsNotNull(button.GetComponent<RenderableComponent>());
    Assert.AreEqual("Test", button.GetComponent<RenderableComponent>().Text);
}
```

**3. Compatibility Tests**

```csharp
[Test]
public void LegacyButton_BehavesLikeOriginal()
{
    var oldButton = new Elements.Button(ui); // Old system
    var newButton = new Button(ui);          // New wrapper

    // Test that both behave identically
    oldButton.Text = "Test";
    newButton.Text = "Test";

    Assert.AreEqual(oldButton.Text, newButton.Text);
}
```

**4. Performance Tests**

```csharp
[Test]
public void NewSystem_PerformsBetterThanOld()
{
    // Create 1000 buttons with both systems
    var oldTime = MeasureCreationTime(() => CreateOldButtons(1000));
    var newTime = MeasureCreationTime(() => CreateNewButtons(1000));

    Assert.Less(newTime, oldTime, "New system should be faster");
}
```

## Risk Mitigation

### Identified Risks & Solutions

**Risk**: Breaking existing user code
**Solution**: Maintain compatibility layer until v2.0 release

**Risk**: Performance regression during transition  
**Solution**: Benchmark every component, optimization-first development

**Risk**: Feature gaps in new system
**Solution**: Feature parity checklist for each migrated class

**Risk**: Team velocity decrease during migration
**Solution**: Parallel development - new features on new system only

## Dependencies & Blockers

### External Dependencies

- ✅ MonoGame.Framework (already available)
- ✅ Core testing infrastructure (already working)

### Internal Dependencies

- ✅ Core ECS system (Phase 1 complete)
- ❌ RenderSystem implementation (needed for visual components)
- ❌ InputSystem implementation (needed for interactive components)

### Current Blockers

1. **None identified** - ready to proceed with Phase 2A
2. All prerequisite architecture is complete and tested

## Success Metrics

### Performance Targets

- **Memory**: 30% reduction in memory allocation for UI creation
- **CPU**: 50% reduction in layout calculation time for complex UIs
- **Startup**: No regression in UI initialization time

### Feature Targets

- **100% feature parity** with existing Button, TextButton, ImageButton
- **Zero breaking changes** for existing user APIs during transition
- **Complete migration path** documented for all legacy classes
