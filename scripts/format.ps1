param(
    [string]$SolutionPath = "ProjectAction.sln"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $SolutionPath)) {
    throw "Solution not found: $SolutionPath"
}

$content = Get-Content -LiteralPath $SolutionPath -Raw
$pattern = 'Project\("\{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC\}"\) = "([^"]+)", "\1\.Player\.csproj"'
$replacement = 'Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$1.Player", "$1.Player.csproj"'
$updated = [System.Text.RegularExpressions.Regex]::Replace($content, $pattern, $replacement)

if ($updated -ne $content) {
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($SolutionPath, $updated, $utf8NoBom)
    Write-Host "Updated Player project display names in $SolutionPath"
}

dotnet restore $SolutionPath
dotnet format $SolutionPath --verify-no-changes --severity warn --exclude "Library/**" "Assets/Plugins/**" "Assets/TutorialInfo/**"
