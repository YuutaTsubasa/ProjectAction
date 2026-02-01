#!/usr/bin/env bash
set -euo pipefail

solution_path="${1:-ProjectAction.sln}"

if [[ ! -f "$solution_path" ]]; then
  echo "Solution not found: $solution_path" >&2
  exit 1
fi

perl -0pe 's/Project\("\{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC\}"\) = "([^"]+)", "\1\.Player\.csproj"/Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$1.Player", "$1.Player.csproj"/g' \
  -i "$solution_path"

dotnet restore "$solution_path"
dotnet format "$solution_path" --verify-no-changes --severity warn --exclude "Library/**" "Assets/Plugins/**" "Assets/TutorialInfo/**"
