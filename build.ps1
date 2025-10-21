<#
.SYNOPSIS
    Builds and publishes one or more NuGet packages with automatic versioning.
.DESCRIPTION
    This script increments the package version, builds the project(s),
    and outputs the NuGet packages to a local directory.
.PARAMETER VersionIncrement
    Specifies which version component to increment: Major, Minor, or Patch (default: Patch)
.PARAMETER OutputPath
    The directory where the NuGet package(s) will be published (default: C:\Workspaces\Packages)
.PARAMETER ProjectFile
    The path to the project file to pack, or "all" to pack the main projects.
    You can also pass a semicolon-separated list of project paths.
.EXAMPLE
    .\build.ps1
    .\build.ps1 -VersionIncrement Minor
    .\build.ps1 -ProjectFile "src\LowlandTech.Plugins.AspNetCore\LowlandTech.Plugins.AspNetCore.csproj"
    .\build.ps1 -ProjectFile all
#>

param(
    [Parameter()]
    [ValidateSet("Major", "Minor", "Patch")]
    [string]$VersionIncrement = "Patch",
    
    [Parameter()]
    [string]$OutputPath = "C:\Workspaces\Packages",
    
    [Parameter()]
    [string]$ProjectFile = "src\lowlandtech.plugins\LowlandTech.Plugins.csproj"
)

# Resolve project list
$projects = @()
if ($ProjectFile -eq 'all') {
    $projects = @(
        "src\lowlandtech.plugins\LowlandTech.Plugins.csproj",
        "src\LowlandTech.Plugins.AspNetCore\LowlandTech.Plugins.AspNetCore.csproj"
    )
}
elseif ($ProjectFile -like '*;*') {
    $projects = $ProjectFile -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
}
else {
    $projects = @($ProjectFile)
}

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "Created output directory: $OutputPath" -ForegroundColor Green
}

# Validate projects exist and make full paths
$fullProjectPaths = @()
foreach ($p in $projects) {
    $csprojPath = Join-Path $PSScriptRoot $p
    if (-not (Test-Path $csprojPath)) {
        Write-Error "Project file not found: $csprojPath"
        exit 1
    }
    $fullProjectPaths += $csprojPath
}

# Read version from the first project (fallback to PackageVersion)
[xml]$firstCsprojXml = Get-Content $fullProjectPaths[0]
$propertyGroups = $firstCsprojXml.Project.PropertyGroup | Where-Object { $_ -ne $null }
$versionNode = $null
$packageVersionNode = $null
foreach ($pg in $propertyGroups) { if ($pg.Version) { $versionNode = $pg.Version; break } }
foreach ($pg in $propertyGroups) { if ($pg.PackageVersion) { $packageVersionNode = $pg.PackageVersion; break } }
if (-not $versionNode -and -not $packageVersionNode) { Write-Error "Version or PackageVersion not found in $($fullProjectPaths[0])"; exit 1 }
$currentVersion = $versionNode
if (-not $currentVersion) { $currentVersion = $packageVersionNode }
Write-Host "Current version (from $($fullProjectPaths[0])): $currentVersion" -ForegroundColor Cyan

# Parse and increment
$versionParts = $currentVersion -split '\.'
if ($versionParts.Count -lt 3) { Write-Error "Invalid version format. Expected Major.Minor.Patch"; exit 1 }
$major = [int]$versionParts[0]; $minor = [int]$versionParts[1]; $patch = [int]$versionParts[2]
switch ($VersionIncrement) { 'Major' { $major++; $minor=0; $patch=0 } 'Minor' { $minor++; $patch=0 } 'Patch' { $patch++ } }
$newVersion = "$major.$minor.$patch"
Write-Host "New version: $newVersion" -ForegroundColor Green

# Update version in all specified project files
foreach ($csprojPath in $fullProjectPaths) {
    [xml]$csproj = Get-Content $csprojPath
    $pgs = $csproj.Project.PropertyGroup | Where-Object { $_ -ne $null }
    foreach ($pg in $pgs) {
        if ($pg.Version) { $pg.Version = $newVersion }
        if ($pg.PackageVersion) { $pg.PackageVersion = $newVersion }
    }
    $csproj.Save($csprojPath)
    Write-Host "Updated project file with new version: $csprojPath" -ForegroundColor Green
}

# Clean/restore/build each project
foreach ($csprojPath in $fullProjectPaths) {
    Write-Host "`nCleaning $csprojPath..." -ForegroundColor Cyan
    dotnet clean $csprojPath --configuration Release --verbosity quiet

    Write-Host "Restoring $csprojPath..." -ForegroundColor Cyan
    dotnet restore $csprojPath
    if ($LASTEXITCODE -ne 0) { Write-Error "Restore failed for $csprojPath"; exit 1 }

    Write-Host "Building $csprojPath..." -ForegroundColor Cyan
    dotnet build $csprojPath --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { Write-Error "Build failed for $csprojPath"; exit 1 }
}

# Pack each project
foreach ($csprojPath in $fullProjectPaths) {
    Write-Host "`nPacking NuGet package for $csprojPath..." -ForegroundColor Cyan
    $packArgs = @(
        'pack', $csprojPath,
        '--configuration', 'Release',
        '--no-build',
        '--output', $OutputPath,
        "/p:Version=$newVersion",
        "/p:PackageVersion=$newVersion"
    )

    $iconPath = Join-Path $PSScriptRoot 'icon.png'
    if (-not (Test-Path $iconPath)) { $packArgs += "/p:PackageIcon=" }

    & dotnet $packArgs
    if ($LASTEXITCODE -ne 0) { Write-Error "Pack failed for $csprojPath"; exit 1 }
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "? Packages successfully created!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Version: $newVersion" -ForegroundColor Cyan
Write-Host "Output: $OutputPath" -ForegroundColor Cyan

# List created packages
Get-ChildItem -Path $OutputPath -Filter "*.$newVersion.*" | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }

# Optional commit
$commitChanges = Read-Host "`nDo you want to commit the version change to git? (y/n)"
if ($commitChanges -eq 'y') { git add $fullProjectPaths; git commit -m "chore: bump versions to $newVersion"; Write-Host "Version change committed to git" -ForegroundColor Green }
