# Repository Guidelines

## Project Structure & Module Organization
- `Assets/`: Unity project content. Core scripts, prefabs, and scenes live here (for example, scenes under `Assets/Scenes/`).
- `Assets/Tests/`: EditMode tests, including asset policy checks.
- `Packages/` and `ProjectSettings/`: Unity package manifest and editor/project configuration.
- `docs/`: Design and workflow references (see `docs/GDD.md`, `docs/AssetPolicy.md`, `docs/PrototypeVerticalSlice.md`).
- `scripts/`: Repo utilities (currently formatting scripts).

## Build, Test, and Development Commands
- `bash scripts/format.sh` (macOS/Linux) or `powershell -ExecutionPolicy Bypass -File scripts/format.ps1` (Windows): one-shot formatting and diagnostic pass; run before PRs.
- Unity Editor: open the project once to generate `.sln`/`.csproj` files.
- Unity Test Runner (EditMode): run `PrimaryRootReferenceTests` to enforce the asset policy (see `docs/AssetPolicy.md`).

## Coding Style & Naming Conventions
- Indentation: 4 spaces; LF line endings; trim trailing whitespace (see `.editorconfig`).
C# naming rules are enforced by Roslyn analyzers and `.editorconfig`. Key patterns:
- Public members: `PascalCase`.
- Private fields: `_camelCase` (including `[SerializeField] private`).
- Private methods: `PascalCase`.
- Constants and `static readonly`: `ALL_UPPER` (for example, `MAX_SPEED`).
- Interfaces: `IPascalCase`.

## Testing Guidelines
- Framework: Unity Test Runner with NUnit for EditMode tests.
- Naming: `*Tests.cs` under `Assets/Tests/EditMode/...` (for example, `PrimaryRootReferenceTests.cs`).
- Run: Unity Test Runner -> EditMode -> `PrimaryRootReferenceTests`.

## Commit & Pull Request Guidelines
- Commit messages in history mix Conventional Commits (`feat:`, `chore:`) and descriptive sentences. Prefer Conventional Commits for new work when feasible.
- PRs must run the format script and fix all diagnostics it reports.
- PRs must run EditMode `PrimaryRootReferenceTests`.
- If you change analyzer or `.editorconfig` rules, rerun checks to confirm no new violations.

## Asset Policy & Prototype Notes
- PrimaryRoot assets must not be referenced by non-PrimaryRoot assets; rely on the installer workflow in `docs/PrototypeVerticalSlice.md`.
- For the parkour vertical slice, use `Tools > Prototype > Install Parkour Vertical Slice v0` to recreate the scene setup.
