# ADOFAI Trainer (冰与火之舞修改器) · Windows 一键安装 / 升级 (PowerShell)
#
# 懒人一键（在 PowerShell 粘贴运行）：
#   irm https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/win_install.ps1 | iex
#
# 自动：定位游戏 → 没装 BepInEx 就下载安装 → 下载最新 ADOFAITrainer.dll 放入插件（覆盖旧版即为升级）。
# 老用户（手动从 Releases 装过）直接跑这一行即可平滑更新到最新版。
$ErrorActionPreference = 'Stop'
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
try { [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 } catch {}

$Repo   = 'Cohenjikan/ADOFAITrainer'
$BepVer = '5.4.23.5'
$BepUrl = "https://github.com/BepInEx/BepInEx/releases/download/v$BepVer/BepInEx_win_x64_$BepVer.zip"
$DllUrl = "https://raw.githubusercontent.com/$Repo/refs/heads/main/dist/ADOFAITrainer.dll"

Write-Host ""
Write-Host "==== 冰与火之舞修改器  Windows 一键安装 / 升级 ====" -ForegroundColor Cyan
Write-Host ""

function Find-Game {
    $cands = New-Object System.Collections.Generic.List[string]
    $steam = $null
    foreach ($k in 'HKCU:\Software\Valve\Steam','HKLM:\SOFTWARE\WOW6432Node\Valve\Steam','HKLM:\SOFTWARE\Valve\Steam') {
        try {
            $pp = Get-ItemProperty -Path $k -ErrorAction Stop
            if ($pp.SteamPath) { $steam = $pp.SteamPath } elseif ($pp.InstallPath) { $steam = $pp.InstallPath }
            if ($steam) { break }
        } catch {}
    }
    if ($steam) {
        $steam = $steam -replace '/','\'; $cands.Add($steam)
        $vdf = Join-Path $steam 'steamapps\libraryfolders.vdf'
        if (Test-Path $vdf) { foreach ($line in Get-Content $vdf) { if ($line -match '"path"\s+"(.+?)"') { $cands.Add(($matches[1] -replace '\\\\','\')) } } }
    }
    'C:\Program Files (x86)\Steam','C:\Program Files\Steam','D:\steam','D:\SteamLibrary','D:\Steam','E:\steam','E:\SteamLibrary' | ForEach-Object { $cands.Add($_) }
    foreach ($lib in ($cands | Select-Object -Unique)) {
        $g = Join-Path $lib 'steamapps\common\A Dance of Fire and Ice'
        if (Test-Path (Join-Path $g 'A Dance of Fire and Ice.exe')) { return $g }
    }
    return $null
}

$game = Find-Game
if (-not $game) {
    Write-Host "没能自动找到《A Dance of Fire and Ice》。" -ForegroundColor Yellow
    Write-Host "请打开 Steam → 右键游戏 → 管理 → 浏览本地文件，把地址栏路径粘到这里后回车："
    $game = (Read-Host '游戏目录').Trim().Trim('"')
}
if (-not (Test-Path (Join-Path $game 'A Dance of Fire and Ice.exe'))) {
    Write-Host "[错误] 该目录下没有 A Dance of Fire and Ice.exe：`n  $game" -ForegroundColor Red
    return
}
Write-Host "游戏目录: $game" -ForegroundColor Green

# 1) BepInEx（若未安装）
$hasBep = (Test-Path (Join-Path $game 'winhttp.dll')) -and (Test-Path (Join-Path $game 'BepInEx\core'))
if (-not $hasBep) {
    Write-Host "未检测到 BepInEx，正在下载 BepInEx $BepVer (x64) ..." -ForegroundColor Cyan
    $zip = Join-Path $env:TEMP "BepInEx_ADOFAI_$BepVer.zip"
    Invoke-WebRequest -Uri $BepUrl -OutFile $zip -UseBasicParsing
    Expand-Archive -Path $zip -DestinationPath $game -Force
    Remove-Item $zip -ErrorAction SilentlyContinue
    Write-Host "BepInEx 安装完成。" -ForegroundColor Green
} else {
    Write-Host "BepInEx 已安装，跳过。" -ForegroundColor Green
}

# 2) 下载最新 ADOFAITrainer.dll 放入插件（覆盖旧版 = 升级）
$plugins = Join-Path $game 'BepInEx\plugins'
New-Item -ItemType Directory -Force -Path $plugins | Out-Null
$dest = Join-Path $plugins 'ADOFAITrainer.dll'
Write-Host "下载最新修改器 DLL ..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $DllUrl -OutFile $dest -UseBasicParsing

Write-Host ""
Write-Host "==== 安装 / 更新成功 ====" -ForegroundColor Green
Write-Host "已安装: $dest"
Write-Host "启动游戏，按 [Insert] 呼出修改器。" -ForegroundColor Cyan
Write-Host "卸载：irm https://raw.githubusercontent.com/$Repo/refs/heads/main/win_uninstall.ps1 | iex"
Write-Host "免费开源 · 严禁倒卖 · github.com/$Repo"
Write-Host ""
