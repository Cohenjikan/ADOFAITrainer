@echo off
setlocal
rem ===== ADOFAI Trainer installer (BepInEx must already be installed) =====
set "GAME=D:\steam\steamapps\common\A Dance of Fire and Ice"

echo Installing ADOFAITrainer.dll into:
echo   "%GAME%\BepInEx\plugins"
echo.
if not exist "%GAME%\A Dance of Fire and Ice.exe" (
  echo [ERROR] "A Dance of Fire and Ice.exe" not found.
  echo Edit this .bat and set GAME= to your install folder.
  pause & exit /b 1
)
if not exist "%GAME%\BepInEx\plugins" (
  echo [ERROR] BepInEx is not installed yet ^(no BepInEx\plugins folder^).
  echo Install BepInEx 5 first, launch the game once, then re-run this. See README.
  pause & exit /b 1
)

copy /Y "%~dp0..\dist\ADOFAITrainer.dll" "%GAME%\BepInEx\plugins\" >nul
if errorlevel 1 ( echo [ERROR] copy failed. & pause & exit /b 1 )

echo Done. Launch the game and press F3 in any level.
pause
