# Legacy Classes Migration Tracking

## Classes Requiring Migration

### Core Base Classes (High Priority)

| Legacy Class | Status         | Target Components                                          | Estimated Effort | Assigned |
| ------------ | -------------- | ---------------------------------------------------------- | ---------------- | -------- |
| `Element`    | ❌ Not Started | TransformComponent + RenderableComponent + LayoutComponent | 2 days           | -        |
| `Container`  | ❌ Not Started | UIEntity + FlexLayoutComponent                             | 1 day            | -        |
| `Control`    | ❌ Not Started | InputComponent + StyleComponent                            | 1 day            | -        |

### UI Controls (Medium Priority)

| Legacy Class  | Status         | Target Components                                      | Estimated Effort | Dependencies                        |
| ------------- | -------------- | ------------------------------------------------------ | ---------------- | ----------------------------------- |
| `Button`      | ❌ Not Started | ButtonComponent + RenderableComponent + InputComponent | 2 days           | RenderableComponent, InputComponent |
| `TextButton`  | ❌ Not Started | ButtonComponent + RenderableComponent(text)            | 1 day            | Button migration                    |
| `ImageButton` | ❌ Not Started | ButtonComponent + RenderableComponent(sprite)          | 1 day            | Button migration                    |
| `TextLabel`   | ❌ Not Started | RenderableComponent(text) + StyleComponent             | 1 day            | RenderableComponent                 |
| `ImageRect`   | ❌ Not Started | RenderableComponent(sprite) + StyleComponent           | 1 day            | RenderableComponent                 |

### Layout Components (Medium Priority)

| Legacy Class            | Status         | Target Components           | Estimated Effort | Dependencies        |
| ----------------------- | -------------- | --------------------------- | ---------------- | ------------------- |
| `HorizontalLayoutPanel` | ❌ Not Started | FlexLayoutComponent(row)    | 1 day            | FlexLayoutComponent |
| `VerticalLayoutPanel`   | ❌ Not Started | FlexLayoutComponent(column) | 1 day            | FlexLayoutComponent |

### Advanced Controls (Lower Priority)

| Legacy Class        | Status         | Target Components                   | Estimated Effort | Dependencies                     |
| ------------------- | -------------- | ----------------------------------- | ---------------- | -------------------------------- |
| `DropdownMenu`      | ❌ Not Started | ButtonComponent + MenuComponent     | 3 days           | ButtonComponent, ScrollComponent |
| `DropdownMenuItem`  | ❌ Not Started | ButtonComponent + MenuItemComponent | 1 day            | ButtonComponent                  |
| `ToggleButton`      | ❌ Not Started | ButtonComponent + ToggleComponent   | 2 days           | ButtonComponent                  |
| `ToggleButtonGroup` | ❌ Not Started | GroupComponent + ToggleComponent    | 2 days           | ToggleComponent                  |

### Supporting Classes

| Legacy Class   | Status          | Target Components          | Notes                                      |
| -------------- | --------------- | -------------------------- | ------------------------------------------ |
| `Component`    | ❌ Not Started  | Custom entity factory      | Abstract base for custom UI components     |
| `ElementStyle` | ✅ Replaced     | StyleComponent             | Handled by new StyleComponent              |
| `BorderStyle`  | ❌ Needs update | StyleComponent integration | Partial migration needed                   |
| `ControlState` | ❌ Not Started  | StateComponent             | For button states (normal, hover, pressed) |

## Migration Dependencies Graph

```
Core Infrastructure (✅ Complete)
├── UIEntity, ComponentCollection, etc.
└── TransformComponent, LayoutComponent

Phase 2A Components (❌ Next Priority)
├── RenderableComponent ── Required by ── All visual elements
├── StyleComponent ────── Required by ── All styled elements
├── InputComponent ────── Required by ── All interactive elements
└── ButtonComponent ───── Depends on ── RenderableComponent + InputComponent

Phase 2B Advanced (❌ Depends on 2A)
├── FlexLayoutComponent ── Replaces ── HorizontalLayoutPanel, VerticalLayoutPanel
├── GridLayoutComponent ── New feature ── Table layouts
├── ScrollComponent ───── Enables ──── DropdownMenu, scrollable areas
└── MenuComponent ─────── Depends on ── ButtonComponent + ScrollComponent

Legacy Migration (❌ Depends on 2A+2B)
├── Element ───────────── Depends on ── All core components
├── Container ─────────── Depends on ── FlexLayoutComponent
├── Control ───────────── Depends on ── InputComponent + StyleComponent
└── All UI Controls ──── Depends on ── ButtonComponent + others
```

## Current Blockers Analysis

### No Blockers for Immediate Work

- ✅ Core ECS infrastructure complete
- ✅ Basic components (Transform, Layout) working
- ✅ Systems architecture in place
- ✅ All tests passing

### Ready to Start: Phase 2A Components

1. **RenderableComponent** - No dependencies, can start immediately
2. **StyleComponent** - No dependencies, can start immediately
3. **InputComponent** - May need UserInterface integration review
4. **ButtonComponent** - Depends on Renderable + Input components

## Effort Estimation Summary

| Phase                         | Total Effort | Priority | Dependencies          |
| ----------------------------- | ------------ | -------- | --------------------- |
| Phase 2A: Core Components     | 8-10 days    | HIGH     | None (ready to start) |
| Phase 2B: Advanced Components | 12-15 days   | MEDIUM   | Phase 2A complete     |
| Phase 3: Legacy Migration     | 15-20 days   | MEDIUM   | Phase 2A+2B complete  |
| Phase 4: Polish & Tools       | 8-10 days    | LOW      | Phase 3 complete      |

**Total Estimated Effort**: 43-55 development days (~9-11 weeks for one developer)
