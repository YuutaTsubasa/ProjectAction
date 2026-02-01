# Repository Guidelines

## Project Structure & Module Organization
This is a Unity project. Primary sources live under `Assets/` (game code, scenes, prefabs, shaders, and ScriptableObjects). Key project configuration lives in `ProjectSettings/`, while package dependencies are declared in `Packages/manifest.json`. Generated or machine-specific data appears in `Library/`, `Temp/`, and `Logs/` and should not be hand-edited or committed. Example content paths in this repo include `Assets/Scenes/`, `Assets/Settings/`, and `Assets/TutorialInfo/`.

## Build, Test, and Development Commands
- Open the project in Unity Hub using the repo root (Unity editor version: 6000.3.6f1).
- Play mode: press the Play button in the Editor to run locally.
- Build: `File > Build Settings` in the Editor, select a target platform, then Build.
- Automated tests: `Window > General > Test Runner` and run EditMode/PlayMode tests as needed.
- Package changes: edit `Packages/manifest.json`, then let Unity resolve dependencies.

## Coding Style & Naming Conventions
- C# code is standard for Unity. Use 4-space indentation and brace-on-same-line style.
- Use PascalCase for public types and methods, camelCase for locals and private fields.
- Keep asset and folder names in TitleCase or PascalCase (e.g., `PlayerController`, `MainMenuScene`).
- Prefer Unity’s built-in formatting or your IDE formatter (Rider/Visual Studio).

## Testing Guidelines
- Use the Unity Test Framework for EditMode and PlayMode tests.
- Create tests under `Assets/Tests/EditMode` or `Assets/Tests/PlayMode`.
- Name tests with `*Tests.cs` and keep them scoped to a single feature.
- Run tests from the Test Runner before submitting changes that impact gameplay or systems.

## Commit & Pull Request Guidelines
- This directory does not contain Git history. If you add Git, use concise, imperative commit messages (e.g., `Add player movement input`), and group related changes.
- PRs should include: a short description, testing notes (what you ran), and screenshots or GIFs for visual changes.

## Security & Configuration Tips
- Do not commit `Library/`, `Temp/`, `Logs/`, or user-specific settings under `UserSettings/`.
- Verify the Unity editor version matches `ProjectSettings/ProjectVersion.txt` before making changes.
