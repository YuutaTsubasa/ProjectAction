# Primary / Fallback Asset Policy

## Overview
This project supports a Primary/Fallback asset replacement flow so GitHub clones can run without paid assets.
Primary assets live in a local-only folder and are never committed; Fallback assets are committed and always safe.

## Folder Layout
- Primary (not committed): `Assets/ProjectContents/PrimaryRoot/`
- Fallback (committed): `Assets/ProjectContents/Fallback/`
- Resources (optional dynamic loading):
  - `Assets/Resources/Primary/{ASSET_PATH}`
  - `Assets/Resources/Fallback/{ASSET_PATH}`

## Static References (AssetAdapter)
Use `AssetAdapter` assets in Prefabs/Scenes instead of referencing Primary assets directly.
Each adapter holds:
- `primaryAsset` (optional, local-only)
- `fallbackAsset` (required, committed)

Resolution rules:
- Full mode: Primary first, fallback if Primary is missing.
- Restricted mode: Always fallback, even if Primary exists.

Adapters are ScriptableObjects created via:
`Assets > Create > Project > Asset Adapter > {Type}`

## Dynamic References (AssetLoader)
`AssetLoader.Load<T>(path)` loads from Resources:
- Full mode: `Resources/Primary/{path}` then `Resources/Fallback/{path}`
- Restricted mode: `Resources/Fallback/{path}` only

Missing assets log a warning with mode, path, and attempted locations.

## Switching Policy
Use Unity menu:
- `Project > Asset Policy > Use Full (Primary Preferred)`
- `Project > Asset Policy > Use Restricted (Fallback Only)`

The setting is stored in `Assets/Resources/GlobalSettings.asset`.

## PrimaryRoot Reference Check (Editor Test)
An EditMode test enforces the rule that Prefab/Scene/ScriptableObject assets must not
reference `Assets/ProjectContents/PrimaryRoot` directly.

Run in Unity Test Runner:
- Category: EditMode
- Test: `PrimaryRootReferenceTests.NoAssetsReferencePrimaryRoot`

Failure output lists:
- The violating asset path
- The referenced PrimaryRoot asset path

## FAQ
**Q: What happens if both Primary and Fallback are missing?**  
A: You get a warning log and `null` is returned.

**Q: How do I add a new adapter type?**  
A: Create a new `AssetAdapter<T>` subclass with `[CreateAssetMenu]`.

**Q: How do I avoid Primary references leaking into scenes?**  
A: Only reference `AssetAdapter` assets or Fallback assets in Prefabs/Scenes.

## Decision Notes
The repository enforces `_camelCase` for private fields and `_PascalCase` for private methods via analyzers.
This matches the `.editorconfig` naming rules and is required to avoid analyzer errors.
