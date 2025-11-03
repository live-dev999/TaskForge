# DeployRelease.ps1
# Autor: Denis Prokhorchik
# Date: 04.07.2018
# Version: 1.0

Write-Host " => scripts is started" -ForegroundColor Yellow
Write-Host " => supported OS: Windows, MacOs, Linux" -ForegroundColor Blue
write-Host ""
if ($IsLinux) {
    Write-Host "Operation system - Linux" -ForegroundColor Yellow
    . ./unix.ps1
}
elseif ($IsMacOS) {
    Write-Host "Operation system - macOS" -ForegroundColor Yellow
    . ./mac-exec.ps1
}
elseif ($IsWindows) {
    Write-Host "Operation system - Windows" -ForegroundColor Yellow
    . ./win-exec.ps1
}

write-Host ""
Write-Host " => scripts is finished" -ForegroundColor Yellow