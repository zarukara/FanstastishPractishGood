param(
    [string]$Unity = 'C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Unity.exe',
    [switch]$Build,
    [switch]$RebuildScene
)
$ErrorActionPreference = 'Stop'
$project = Split-Path -Parent $PSScriptRoot
$logs = Join-Path $project 'Logs'
New-Item -ItemType Directory -Force -Path $logs | Out-Null
if (!(Test-Path -LiteralPath $Unity)) { throw "Unity not found: $Unity" }
function Invoke-Unity([string]$arguments, [string]$logName, [switch]$Rendered) {
    $log = Join-Path $logs $logName
    $mode = if ($Rendered) { "" } else { "-batchmode" }
    $process = Start-Process -FilePath $Unity -ArgumentList "$mode -projectPath `"$project`" $arguments -logFile `"$log`"" -PassThru -Wait -WindowStyle Hidden
    if ($process.ExitCode -ne 0) { throw "Unity exited with $($process.ExitCode). See $log" }
}
if ($RebuildScene) {
    Invoke-Unity '-executeMethod CoreSystem.Editor.CoffeeShopBuilder.CreateGame -quit' 'scene.log'
}
$results = Join-Path $logs 'editmode-results.xml'
Invoke-Unity "-runTests -testPlatform EditMode -testResults `"$results`"" 'tests.log' -Rendered
[xml]$report = Get-Content -LiteralPath $results
if ([int]$report.'test-run'.failed -gt 0 -or [int]$report.'test-run'.passed -lt 17) {
    throw "Incomplete or failed tests. See $results"
}
Write-Output "Passed $($report.'test-run'.passed) Unity tests."
if ($Build) {
    Invoke-Unity '-executeMethod CoreSystem.Editor.CoffeeShopBuilder.BuildWindows -quit' 'build-windows.log'
    Write-Output (Join-Path $project 'Builds\Windows-DX11\OnePrice.exe')
}



