# ProjectAction

## Code Convention Checking System
This repository uses `.editorconfig` plus Roslyn analyzers to keep C# style and naming consistent across Unity scripts.

## Primary / Fallback Asset Policy
See `docs/AssetPolicy.md` for setup, usage, and editor toggle instructions.

## Game Design Document (GDD)
`docs/GDD.md` is the authoritative design reference for gameplay scope, features, and UX targets.
目前僅實作第一階段（跑酷核心）；第二階段與「未來擴展」內容屬於設計規劃。

### Local setup
1. Open the project once in Unity to generate the solution and project files.
2. Run the checks (built-in .NET SDK command):
   - `dotnet format ProjectAction.sln --verify-no-changes --severity warn --exclude "Library/**" "Assets/Plugins/**" "Assets/TutorialInfo/**"`
   - `--exclude <EXCLUDE>` accepts a space-separated list of relative paths to exclude.
3. One-shot format workflow (renames duplicate Player display names, restores, then formats):
   - Windows: `powershell -ExecutionPolicy Bypass -File scripts/format.ps1`
   - macOS/Linux: `bash scripts/format.sh`
4. Run the PrimaryRoot reference check in Unity Test Runner (EditMode).

### Before opening a PR
- Run the command above and fix all diagnostics it reports.
- If you changed `.editorconfig` or analyzer settings, rerun the checks to confirm no new violations.

### Updating or extending rules
- Edit `.editorconfig` for naming, formatting, and code-style rules.
- Edit `Directory.Build.props` to add/remove analyzer packages or adjust analysis settings.
- Use per-directory overrides in `.editorconfig` (for example, to relax rules in a specific folder).

### Unity-specific notes
- Unity lifecycle methods (e.g., `Awake`, `Start`, `OnEnable`) already match PascalCase rules.
- Private fields (including `[SerializeField] private`) must be `_camelCase` (example: `_appleFruit`).
- Constants and readonly fields use `ALL_UPPER` (example: `MAX_SPEED`).
- If a Unity API pattern conflicts with a rule, prefer a scoped `.editorconfig` override in that folder.
