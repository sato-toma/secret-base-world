$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectPath = Join-Path $repositoryRoot 'UnityProject'
$templateUrl = 'https://github.com/ClusterVR/ClusterCreatorKitTemplate/archive/master.zip'
$requiredPaths = @(
    'Assets/ClusterCreatorKitTemplate/Minimal/Scenes/MinimalSample.unity',
    'Assets/ClusterCreatorKitTemplate/Minimal/Scenes/MinimalSample.unity.meta',
    'Packages/manifest.json',
    'ProjectSettings/ProjectVersion.txt',
    'LICENSE.md',
    'README.md'
)

if (Test-Path -LiteralPath $projectPath) {
    $isInstalled = $true
    foreach ($path in $requiredPaths) {
        if (-not (Test-Path -LiteralPath (Join-Path $projectPath $path))) {
            $isInstalled = $false
            break
        }
    }

    if ($isInstalled) {
        Write-Output "Cluster Creator Kit template is already installed in $projectPath"
        exit 0
    }

    throw "UnityProject already exists but does not contain a complete template. Inspect it and move it out of the way before running setup again: $projectPath"
}

$setupId = [guid]::NewGuid().ToString('N')
$workDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "cluster-creator-kit-template-$setupId"
$archivePath = Join-Path $workDirectory 'template.zip'
$extractPath = Join-Path $workDirectory 'extracted'
$stagingPath = Join-Path $repositoryRoot ".UnityProject-setup-$setupId"

try {
    New-Item -ItemType Directory -Path $workDirectory | Out-Null
    $null = Get-Command curl.exe -ErrorAction Stop
    & curl.exe --fail --location --retry 3 --silent --show-error --output $archivePath $templateUrl
    if ($LASTEXITCODE -ne 0) {
        throw "Template download failed with curl exit code $LASTEXITCODE."
    }

    Expand-Archive -LiteralPath $archivePath -DestinationPath $extractPath
    $sourceRoot = Join-Path $extractPath 'ClusterCreatorKitTemplate-master'
    if (-not (Test-Path -LiteralPath $sourceRoot)) {
        throw 'The downloaded archive did not contain the expected template directory.'
    }

    foreach ($path in $requiredPaths) {
        if (-not (Test-Path -LiteralPath (Join-Path $sourceRoot $path))) {
            throw "The template is missing a required file: $path"
        }
    }

    New-Item -ItemType Directory -Path $stagingPath | Out-Null
    Get-ChildItem -LiteralPath $sourceRoot -Force |
        Where-Object { $_.Name -ne '.gitignore' } |
        Copy-Item -Destination $stagingPath -Recurse

    foreach ($path in $requiredPaths) {
        if (-not (Test-Path -LiteralPath (Join-Path $stagingPath $path))) {
            throw "Staged template is missing a required file: $path"
        }
    }

    Move-Item -LiteralPath $stagingPath -Destination $projectPath
    Write-Output "Cluster Creator Kit template installed in $projectPath"
    Write-Output 'Open UnityProject in Unity Hub, then open Assets/ClusterCreatorKitTemplate/Minimal/Scenes/MinimalSample.unity.'
}
finally {
    foreach ($path in @($stagingPath, $workDirectory)) {
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Recurse -Force
        }
    }
}
