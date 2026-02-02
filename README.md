# ProjectAction

## Code Convention Checking System
This repository uses `.editorconfig` plus Roslyn analyzers to keep C# style and naming consistent across Unity scripts.

## Primary / Fallback Asset Policy
See `docs/AssetPolicy.md` for setup, usage, and editor toggle instructions.

## Game Design Document (GDD)
`docs/GDD.md` is the authoritative design reference for gameplay scope, features, and UX targets.
目前僅實作第一階段（跑酷核心）；第二階段與「未來擴展」內容屬於設計規劃。

## Phase 1 Parkour Core Vertical Slice v0
使用 `docs/PrototypeVerticalSlice.md` 進行安裝與測試說明。
目前僅提供 Prototype 方塊關卡流程，透過 `RootScene` 的 UniTask `Run()` 驅動完整跑酷閉環。

### Local setup
1. Open the project once in Unity to generate the solution and project files.
2. One-shot format workflow (renames duplicate Player display names, restores, then formats):
   - Windows: `powershell -ExecutionPolicy Bypass -File scripts/format.ps1`
   - macOS/Linux: `bash scripts/format.sh`
3. Run the PrimaryRoot reference check in Unity Test Runner (EditMode).

### Before opening a PR
- Run the one-shot format workflow above and fix all diagnostics it reports.
- If you changed `.editorconfig` or analyzer settings, rerun the checks to confirm no new violations.

### Updating or extending rules
- Edit `.editorconfig` for naming, formatting, and code-style rules.
- Edit `Directory.Build.props` to add/remove analyzer packages or adjust analysis settings.
- Use per-directory overrides in `.editorconfig` (for example, to relax rules in a specific folder).

### Unity-specific notes
- Unity lifecycle methods (e.g., `Awake`, `Start`, `OnEnable`) already match PascalCase rules.
- Private fields (including `[SerializeField] private`) must be `_camelCase` (example: `_appleFruit`).
- Private methods use PascalCase without a leading underscore (example: `CalculateSpeed`).
- Constants and `static readonly` fields use `ALL_UPPER` (example: `MAX_SPEED`).
- If a Unity API pattern conflicts with a rule, prefer a scoped `.editorconfig` override in that folder.

## Camera Setup (Cinemachine Orbital)
- The installer creates a Cinemachine setup (Main Camera + CinemachineBrain + CinemachineCamera with Orbital Follow).
- The virtual camera is driven by `CinemachineOrbitalCamera` to map the existing look input to Cinemachine axes.
- A Rotation Composer is added to keep the camera aimed at the player.
- If you need to re-create the scene, run `Tools > Prototype > Install Parkour Vertical Slice v0` again.
