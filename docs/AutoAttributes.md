# Auto Attributes

## Purpose
`AutoAttributes` lets fields declare where to fetch components from, then resolve lazily at first access or pre-bind in editor.

## Supported Attributes
- `[GetComponent]`
- `[GetComponentInParent]`
- `[GetComponentInChildren]`
- `[GetComponentsInChildren]` (field type must be `Component[]` or `List<ComponentType>`)

## Runtime Usage (ProjectBehaviour Auto-Bind)
In this project, classes that need auto binding should inherit `ProjectBehaviour`.
`ProjectBehaviour.Awake()` calls `AutoComponentBinder.Bind(this, false)` automatically.

```csharp
using ProjectAction.AutoAttributes;
using ProjectAction.Core;
using UnityEngine;

public sealed class ExampleView : ProjectBehaviour
{
    [SerializeField, GetComponent] private Rigidbody _rigidbody;
    [SerializeField, GetComponentInChildren] private UnityEngine.Camera _camera;
}
```

If initialization logic is needed in Awake timing, override `OnProjectAwake()`.

## Editor Pre-Bind
Menu:
- `Tools/Auto Attributes/Bind/Active Scene`
- `Tools/Auto Attributes/Bind/Active Scene (Overwrite Existing)`
- `Tools/Auto Attributes/Bind/Current Prefab Stage`
- `Tools/Auto Attributes/Bind/Current Prefab Stage (Overwrite Existing)`

These commands scan MonoBehaviours with Auto Attributes and write resolved references back to serialized fields.
