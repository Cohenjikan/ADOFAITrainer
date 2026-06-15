<!-- Language switch -->
[简体中文](README.md) | **English**

<div align="center">

<img src="docs/assets/hero.png" alt="ADOFAI Trainer — an in-game GUI trainer for A Dance of Fire and Ice with autoplay, hide-HUD, speed, widened judgement, no-fail and unlock-all, opened with F3" width="100%">

<sub> <a href="docs/assets/promo.mp4">Watch the 30-second promo</a></sub>

# ADOFAI Trainer · 冰与火之舞 修改器

**Press F3. Get a flawless run. It's the game's own autoplay, just exposed.**

An **in-game GUI trainer** for *A Dance of Fire and Ice*, built on BepInEx 5. Frame-perfect autoplay, HUD-free recording, speed control, no-fail, widened judgement, and level unlock — **by toggling the game's own switches, with no memory hacking**.

[![release](https://img.shields.io/github/v/release/Cohenjikan/ADOFAITrainer?label=release&sort=semver&color=ff6b6b)](https://github.com/Cohenjikan/ADOFAITrainer/releases)
[![license](https://img.shields.io/github/license/Cohenjikan/ADOFAITrainer?color=4ecdc4)](LICENSE)
[![BepInEx](https://img.shields.io/badge/BepInEx-5.4.23.x-7048e8)](https://github.com/BepInEx/BepInEx)
[![game](https://img.shields.io/badge/ADOFAI-Steam%20977950-1b2838?logo=steam)](https://store.steampowered.com/app/977950/)
[![price](https://img.shields.io/badge/free%20%26%20open%20source-FREE-2ea44f)](https://github.com/Cohenjikan/ADOFAITrainer)
[![stars](https://img.shields.io/github/stars/Cohenjikan/ADOFAITrainer?style=social)](https://github.com/Cohenjikan/ADOFAITrainer/stargazers)

[Features](#features) · [Quickstart](#quickstart-3-steps) · [Usage](#usage) · [Trade-offs](#trade-offs--gotchas-read-this) · [Build](#build-from-source) · [简体中文](README.md)

</div>

> [!NOTE]
> **Single-player only** (e.g. for recording flawless-clear videos). Please don't use it for online leaderboards or competitive play. Not affiliated with 7th Beat Games.

> [!IMPORTANT]
> **This tool is completely free and open-source — reselling is forbidden.** The title bar, the top of the menu, and the load log all carry the project URL as a watermark, and a built-in integrity check **disables the trainer if that watermark / URL is removed or altered** (even the one Harmony patch is skipped). If you paid for this, you were scammed — get it for free at [this repository](https://github.com/Cohenjikan/ADOFAITrainer).

---

## Install

### macOS / Linux (native, no BepInEx)

Paste one line into a terminal (auto-installs the .NET SDK → builds → weaves the loader into the game's startup):

```bash
curl -fsSL https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/install.sh | bash
```

Launch the game via Steam and press **F3** anywhere to toggle the menu. Uninstall (one line too):

```bash
curl -fsSL https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/uninstall.sh | bash
```
> A game update or Steam "Verify integrity of game files" restores the DLL — just re-run the install command.

### Windows

Open **PowerShell** and paste one line (auto-locates the game → installs BepInEx if missing → downloads the latest DLL; existing users get a smooth upgrade by running it too):

```powershell
irm https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/win_install.ps1 | iex
```

Press **F3** in-game. Uninstall the same way:

```powershell
irm https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/win_uninstall.ps1 | iex
```
> Or download manually from **[Releases](https://github.com/Cohenjikan/ADOFAITrainer/releases)**; full steps in [Windows install (detailed)](#windows-install-bepinex-detailed) below.

---

## Why this exists

The hardest part of recording a flawless ADOFAI clear is **manual precision** — the game is "tap on the beat," and a zero-mistake run by hand is nearly impossible.

Instead of grinding your fingers raw or scripting a macro, this trainer borrows the engine's **built-in autoplay**: it fires on the chart's floating-point beat times with no input lag, so it's frame-perfect by nature and looks identical to a real run — and **there's no autoplay watermark in the main game**.

It's fundamentally different from a memory-scanning cheat:

| | Typical memory cheat | This trainer |
|---|---|---|
| How it works | Scans / rewrites memory offsets | Only flips the game's **own existing** flags (`RDC` / `GCS` / `scrController` / `Persistence`) |
| After a game update | Breaks the moment offsets shift | Calls the game's own logic, so it **usually keeps working** |
| Custom patches | Many hooks | **Exactly one Harmony patch** in the whole project (widen judgement); everything else is plain field sets |
| UI | Usually a config file you edit blind | An **in-game IMGUI overlay** with a CJK font and three tabs |

> Shares its engine and approach with the author's sibling project, the *Rhythm Doctor Trainer* — both games are by 7th Beat Games with a highly shared codebase. (See the author's other projects on [GitHub](https://github.com/Cohenjikan).)

---

## Features

Press **F3** to open the overlay. Three tabs: **Normal** / **Developer** / **About**.

### Normal

#### Autoplay — frame-perfect, no watermark
The engine auto-plays every tile perfectly; on screen it looks identical to a real run, with **no autoplay watermark in the main game**. Toggle any time inside a level.
> How: `RDC.auto` is set only while in a level — it's the engine's native autoplay flag (only the editor's Otto mascot shows a watermark, which is why it's gated to in-level).

<img src="docs/assets/feature-1.png" alt="Normal tab: Autoplay and Hide HUD toggles with explanatory captions" width="70%">

#### Hide HUD (clean recording)
Strip the on-screen HUD for clean OBS captures; together with Autoplay this reproduces the game's own dev recording mode.
> How: sets `RDC.noHud`; the game's dev recording mode is exactly `RDC.auto = true; RDC.noHud = true`.

#### Game speed 0.5×–3× (pitch included)
Slow-mo practice or speed-up. A continuous slider plus `0.75×` / `1×` / `1.5×` / `2×` quick buttons.
> How: arms `GCS.speedTrialMode` + `GCS.nextSpeedRun` (read at level Start); "Apply speed & restart level" calls `scrController.instance.Restart(true)` to apply it.

<img src="docs/assets/feature-2.png" alt="Speed section: speed toggle, slider, 0.75x/1x/1.5x/2x quick buttons, and Apply speed and restart level button" width="70%">

#### No-Fail
The game's built-in practice mode — never fail or get interrupted, live-toggleable inside a level.
> How: sets `GCS.useNoFail` and also pushes `scrController.instance.noFail` onto the live level.

#### Widen judgement (incl. Perfect window)
The project's **only Harmony patch**. Manual play scores Perfect even when slightly off-beat — multiplier `1.0–5.0`, widening Perfect/Pure too.
> How: `JudgeWindowPatch` is a Postfix on `scrMisc.GetAdjustedAngleBoundaryInDeg` that multiplies `__result` by `max(1.0, judgeMult)`, widening Counted / Perfect / Pure together. With Autoplay on you're already perfect, so this is purely for manual practice.

<img src="docs/assets/feature-3.png" alt="Convenience section: No-Fail toggle and widen-judgement multiplier slider" width="70%">

### Developer

| Feature | Notes |
|---|---|
| **Unlock all levels** (temporary, no save write) + **Go to level select** | Together = jump to any level; turning it off restores everything, leaving the save untouched (`RDC.forceUnlockAllLevels`) |
| **Permanent unlock all** | Writes the game's own `Persistence.unlockAllLevels` and saves; **reversible at any time, deletes no existing progress** |
| **Skip cutscenes** / **Show FPS** / **Static planet colors** | Steadier for recording (`RDC.skipCutscenes` / `GCS.showFPS` / `GCS.staticPlanetColors`) |
| **Open save folder** | Opens Explorer at `Persistence.DataPath` |

---

## Windows install (BepInEx, detailed)

> For the **Steam release** (Unity 6 / x64 / Mono). BepInEx 5 is required first.

**① Install BepInEx 5 (x64, Mono)**

1. Download **`BepInEx_win_x64_5.4.23.x.zip`** from [BepInEx Releases](https://github.com/BepInEx/BepInEx/releases).
2. **Extract its contents into the game root** (next to `A Dance of Fire and Ice.exe`; you should then see `winhttp.dll`, `BepInEx/`, etc.).
   > Find the game folder: Steam → right-click *A Dance of Fire and Ice* → Manage → Browse local files.
3. **Launch the game once, then quit**, so BepInEx generates `BepInEx/plugins`, `BepInEx/config`, etc.

**② Drop in the trainer**

- **Option A (manual, recommended):** download [`dist/ADOFAITrainer.dll`](dist/ADOFAITrainer.dll) and drop it into `<game>\BepInEx\plugins\`.
- **Option B (script):** clone this repo → edit the `GAME=` path in [`tools/install.bat`](tools/install.bat) → run it; it copies `dist\ADOFAITrainer.dll` into `BepInEx\plugins`.

**③ Verify**

After launching, open `<game>\BepInEx\LogOutput.log` and look for a line containing (the full line has a longer prefix and a trailing `Switches: ...` list):

```text
ADOFAI Trainer (冰与火之舞修改器) v1.41 · 免费开源 FREE · github.com/Cohenjikan/ADOFAITrainer · loaded. Menu key = F3.
```

Enter any level and press **F3** to open the menu.

---

## Usage

1. In any level, press **F3** to open/close the menu.
2. **Record a flawless run:** enable **Autoplay** on the *Normal* tab (optionally **Hide HUD**) → enter a level → capture with OBS, etc.
3. **Record a locked level:** on the *Developer* tab enable **Unlock all levels** → **Go to level select** and enter it.
4. **Speed:** after moving the slider (or hitting a preset), click **Apply speed & restart level** to apply it (the engine can't change speed mid-song); use **Reset speed & restart** to revert.
5. **Play by hand again:** turn Autoplay off; for easy manual Perfects, enable **Widen judgement**.

---

## Trade-offs & gotchas (read this)

- **Speed does NOT preserve pitch.** The engine scales `song.pitch` directly with no time-stretch, so faster/slower also raises/lowers pitch. This is a known trade-off.
- **Speed only applies at level start/restart.** The engine can't change speed mid-song, so always hit "Apply speed & restart level" after changing it.
- **The "speed trial" mechanic auto-bumps +0.1.** The game adds 0.1 to speed after you win a level, so **re-set the speed before recording at a fixed rate**.
- **You must install BepInEx 5 (x64, Mono) yourself first** — this is not a one-click installer.
- **Version-pinned to the current engine:** targets Unity 6 (6000.3.x), BepInEx 5.4.23.x, `netstandard2.1`; a major game update may require adjustments.
- **The integrity / anti-resale gate is not DRM.** It's a same-DLL string check on const values — it deters casual resale but is **trivially defeated by recompiling**, so don't treat it as real protection.
- **Strictly single-player / offline.** Don't use it online, for leaderboards, or in competition; modding may violate the game's EULA — **use at your own risk**.

---

## Build from source

Requires the .NET SDK (targets `netstandard2.1`) and a copy of the game with **BepInEx already installed** (for the reference DLLs, including `RDTools.dll`). This repo ships **no game assets**.

```bash
# Defaults to D:\steam\steamapps\common\A Dance of Fire and Ice
# Override with -p:GameDir=...
dotnet build src/ADOFAITrainer.csproj -c Release -p:GameDir="X:\path\to\A Dance of Fire and Ice"
```

Output: `src/bin/Release/ADOFAITrainer.dll`.

---

## Uninstall

- **Remove only the trainer:** delete `<game>\BepInEx\plugins\ADOFAITrainer.dll` (or run [`tools/uninstall.bat`](tools/uninstall.bat)).
- **Remove BepInEx too / restore vanilla:** delete `winhttp.dll` from the game root (fastest way to disable BepInEx), or delete `winhttp.dll` + the `BepInEx/` folder + `doorstop_config.ini`.
- You can also use Steam's "Verify integrity of game files" to restore everything.

> The config file is at `<game>\BepInEx\config\com.cohen.adofaitrainer.cfg` (you can rebind the menu key); delete it too when uninstalling.

---

## Compatibility

| Item | Value |
|---|---|
| Game | A Dance of Fire and Ice (Steam release, appid 977950) |
| Engine | Unity 6 (6000.3.x) / x64 / Mono |
| Loader | BepInEx 5.4.23.x |
| Target framework | netstandard2.1 |

> A major game update may require adjustments; if it fails to load, first confirm your BepInEx version matches this guide.

---

## Disclaimer

- **Unofficial.** This is an unofficial, fan-made third-party tool, **not affiliated with, authorized, or endorsed by** the game's developer [7th Beat Games](https://7thbeat.com/). *A Dance of Fire and Ice* and all related names, trademarks, art, and music are the property of 7th Beat Games.
- **No game content.** This repository contains **only the author's own plugin code** — it includes and distributes no game source, DLLs, audio, images, or other assets. At runtime it only calls the game's **own existing** public functions via BepInEx / HarmonyX; no memory scanning.
- **Single-player only.** For **offline single-player** fun, practice, and recording only. Do **not** use it online, for leaderboards, in competition, or in any way that affects fairness for other players.
- **Respect the EULA.** Modding the game may violate its End-User License Agreement / Terms of Service. Use is entirely at your own discretion, and you are responsible for complying with those terms; any consequences (account penalties, save corruption, etc.) are your own.
- **Use at your own risk.** Provided "as is", without warranty of any kind. The author is not liable for any direct or indirect damage arising from its use.
- **Free.** Free and open-source ([MIT](LICENSE)); **reselling is forbidden.** If you paid for it, you were scammed — get it free from this repository.
- **Rights holders.** If a rights holder considers anything here improper, please reach out via a GitHub Issue and the author will comply with takedown or changes.

---

## Credits

- Mod frameworks: [BepInEx](https://github.com/BepInEx/BepInEx) / [HarmonyX](https://github.com/BepInEx/HarmonyX).
- Shares its approach with the sibling project *Rhythm Doctor Trainer* (also by 7th Beat Games).

Licensed under [MIT](LICENSE). Made by Cohenjikan.