using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ADOFAITrainer
{
    // In-game trainer for A Dance of Fire and Ice (冰与火之舞). F3 opens an overlay.
    // Same philosophy as the Rhythm Doctor trainer (both 7th Beat Games, shared engine):
    // only flip switches the game itself already has (RDC / GCS / scrController / Persistence)
    // via BepInEx + HarmonyX — no memory scanning, so it stays stable across updates.
    //
    // FREE & open-source. Reselling is forbidden. The whole cheat engine is gated on the
    // watermark URL below being intact (see Awake / Ok) — blank or rebrand it and every
    // feature turns off.
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "com.cohen.adofaitrainer";
        public const string Name = "ADOFAI Trainer (冰与火之舞修改器)";
        public const string Version = "1.41";
        public const string Repo = "github.com/Cohenjikan/ADOFAITrainer";
        // Watermark, shown right after the version everywhere (title / top of menu / load log /
        // About). It doubles as the integrity key — removing it disables the trainer.
        public const string Mark = "免费开源 FREE · " + Repo;

        internal static ManualLogSource Log;
        internal static bool Ok = true;   // integrity flag; false → every feature disabled

        private static ConfigEntry<KeyboardShortcut> _menuKey;

        private bool _menuOpen;
        private int _tab;
        private Rect _win = new Rect(24, 24, 470, 640);
        private Vector2 _scroll;
        private Font _cjk;
        private bool _lastSpeedOverride;

        private void Awake()
        {
            Log = Logger;
            _menuKey = Config.Bind("General", "MenuKey", new KeyboardShortcut(KeyCode.F3),
                "Hotkey to open/close the trainer overlay.");

            // Integrity / anti-resale gate. The cheat engine only runs while the project URL is
            // intact. If someone blanks or rebrands it to resell the DLL, Ok becomes false and
            // nothing — patches included — is enabled. This tool is free; do not sell it.
            Ok = Repo.Contains("Cohenjikan/ADOFAITrainer") && Mark.Contains(Repo);
            if (!Ok)
            {
                Log.LogError("完整性校验失败：水印 / 项目地址被篡改或删除，修改器已禁用。" +
                             "本工具免费开源，严禁倒卖。请从 " + Repo + " 获取正版。");
                return; // do NOT PatchAll, do NOT enable any feature
            }

            // One Harmony patch (JudgeWindowPatch in Patches.cs); the rest is plain field sets.
            try { new Harmony(Guid).PatchAll(); }
            catch (Exception e) { Log.LogError("PatchAll failed: " + e); }
            Log.LogInfo($"{Name} v{Version} · {Mark} · loaded. Menu key = {_menuKey.Value}. " +
                        "Switches: RDC.auto / RDC.noHud / GCS.useNoFail / speedTrial / judge-window / unlockAllLevels.");
        }

        private void Update()
        {
            try
            {
                if (_menuKey.Value.IsDown()) _menuOpen = !_menuOpen;
                if (_menuOpen) { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }
                ApplyState();
            }
            catch (Exception e) { Log.LogError("Update: " + e); }
        }

        // Are we inside a playable level (singleton exists)? Used to gate level-only switches
        // so we never fight the level editor's "Otto" auto button or the main menu.
        private static bool InLevel()
        {
            try { return scrController.instance != null; }
            catch { return false; }
        }

        private void ApplyState()
        {
            if (!Ok) return;   // integrity check failed → no features
            bool inLevel = InLevel();

            // --- Autoplay ---------------------------------------------------------------
            // RDC.auto is the global autoplay flag; the planet/player reads it live every
            // frame and auto-hits each tile perfectly (HitMargin.Auto, weighted == Perfect).
            // The main game shows NO autoplay watermark (only the editor's Otto mascot does,
            // and we only push this while in a level, so menus/editor are left alone).
            if (inLevel)
            {
                try { if (RDC.auto != Cheats.autoplay) RDC.auto = Cheats.autoplay; } catch (Exception e) { Log.LogError("set RDC.auto: " + e); }
            }

            // --- Hide HUD (clean recording) --------------------------------------------
            // The game's own dev recording mode does exactly `RDC.auto = true; RDC.noHud = true`.
            try { if (RDC.noHud != Cheats.noHud) RDC.noHud = Cheats.noHud; } catch { }

            // --- No-Fail ---------------------------------------------------------------
            // GCS.useNoFail is the game's built-in practice no-fail (drives the menu indicator).
            // scrController caches it at level start, so also push it onto the live controller.
            try { if (GCS.useNoFail != Cheats.noFail) GCS.useNoFail = Cheats.noFail; } catch { }
            if (inLevel)
            {
                try { var c = scrController.instance; if (c != null && c.noFail != Cheats.noFail) c.noFail = Cheats.noFail; } catch { }
            }

            // --- Speed ------------------------------------------------------------------
            // bpm + song.pitch are fixed at level Start (`song.pitch *= currentSpeedTrial`),
            // so changing speed mid-song desyncs audio. We only ARM the next-run config here
            // (speedTrialMode + nextSpeedRun, both read at Start); the "重开" button reloads
            // the scene to apply it. currentSpeedTrial is left untouched on purpose.
            try
            {
                if (Cheats.speedOverride)
                {
                    if (!GCS.speedTrialMode) GCS.speedTrialMode = true;
                    if (GCS.nextSpeedRun != Cheats.speed) GCS.nextSpeedRun = Cheats.speed;
                }
                else if (_lastSpeedOverride)
                {
                    GCS.speedTrialMode = false;
                    GCS.nextSpeedRun = 1f;
                }
            }
            catch { }
            _lastSpeedOverride = Cheats.speedOverride;

            // Widen-judge is handled by JudgeWindowPatch (Patches.cs); no per-frame work here.

            // --- Dev flags (pure static sets, harmless anywhere) ------------------------
            try { if (RDC.forceUnlockAllLevels != Cheats.forceUnlockAll) RDC.forceUnlockAllLevels = Cheats.forceUnlockAll; } catch { }
            try { if (RDC.skipCutscenes != Cheats.skipCutscenes) RDC.skipCutscenes = Cheats.skipCutscenes; } catch { }
            try { if (GCS.showFPS != Cheats.showFps) GCS.showFPS = Cheats.showFps; } catch { }
            try { if (GCS.staticPlanetColors != Cheats.staticColors) GCS.staticPlanetColors = Cheats.staticColors; } catch { }
        }

        // ---------------- IMGUI ----------------
        private void EnsureFont()
        {
            if (_cjk != null) return;
            try
            {
                _cjk = Font.CreateDynamicFontFromOSFont(
                    new[] { "Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "SimSun", "Arial" }, 14);
            }
            catch { }
        }

        private void OnGUI()
        {
            if (!_menuOpen) return;
            EnsureFont();
            var prev = GUI.skin.font;
            if (_cjk != null) GUI.skin.font = _cjk;
            _win = GUILayout.Window(740182, _win, DrawWindow,
                $"冰与火之舞修改器 v{Version}  ·  免费开源 {Repo}  ·  [{_menuKey.Value}] 开/关");
            GUI.skin.font = prev;
        }

        private void DrawWindow(int id)
        {
            // Integrity failed → show only the notice; no controls.
            if (!Ok)
            {
                GUILayout.Label("完整性校验失败：水印被篡改 / 删除，修改器已禁用。", Mark1());
                GUILayout.Label("本工具免费开源，严禁倒卖。请从下方地址获取正版：", Lbl());
                GUILayout.Label(Repo, Mark1());
                GUI.DragWindow(new Rect(0, 0, 10000, 24));
                return;
            }

            // Strong, always-visible watermark (menu-only; the menu is hidden during recording).
            GUILayout.Label("本修改器免费开源 · 严禁倒卖 · " + Repo, Mark1());
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_tab == 0, " 普通 ", "Button")) _tab = 0;
            if (GUILayout.Toggle(_tab == 1, " 开发者 ", "Button")) _tab = 1;
            if (GUILayout.Toggle(_tab == 2, " 关于 ", "Button")) _tab = 2;
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            _scroll = GUILayout.BeginScrollView(_scroll);
            switch (_tab) { case 1: DrawDev(); break; case 2: DrawAbout(); break; default: DrawNormal(); break; }
            GUILayout.EndScrollView();

            bool inLevel = InLevel();
            GUILayout.Label(inLevel ? "● 当前在关卡内" : "○ 当前在菜单 / 编辑器", Lbl());
            GUI.DragWindow(new Rect(0, 0, 10000, 24));
        }

        // ---------------- 普通玩家 ----------------
        private void DrawNormal()
        {
            Section("录制 / 演示");
            Cheats.autoplay = GUILayout.Toggle(Cheats.autoplay, " Autoplay — 全程满分自动演奏");
            Indent(() => GUILayout.Label("进入关卡即自动完美命中每个拍点；主游戏无水印。可在关卡内随时开关。", Lbl()));

            GUILayout.Space(6);
            Cheats.noHud = GUILayout.Toggle(Cheats.noHud, " 隐藏 HUD（干净录制）");
            Indent(() => GUILayout.Label("配合 Autoplay 即为游戏自带的录制模式（auto + noHud）。", Lbl()));

            GUILayout.Space(8);
            Section("变速（含音高）");
            Cheats.speedOverride = GUILayout.Toggle(Cheats.speedOverride, $" 变速：{Cheats.speed:0.00}x");
            Indent(() =>
            {
                Cheats.speed = GUILayout.HorizontalSlider(Cheats.speed, 0.5f, 3.0f);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("0.75x")) Cheats.speed = 0.75f;
                if (GUILayout.Button("1x")) Cheats.speed = 1f;
                if (GUILayout.Button("1.5x")) Cheats.speed = 1.5f;
                if (GUILayout.Button("2x")) Cheats.speed = 2f;
                GUILayout.EndHorizontal();
                GUILayout.Label("⚠ 速度在关卡开始时生效（中途变速会音画不同步）。改完点下面按钮重开本关。\n注意：游戏的「速度试炼」机制会在赢一关后自动 +0.1，要录定速请重新设置。", Lbl());
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("应用变速并重开本关"))
                    Run("ApplySpeed+Restart", () => { var c = scrController.instance; if (c != null) c.Restart(true); });
                if (GUILayout.Button("恢复原速并重开"))
                    Run("ResetSpeed+Restart", () =>
                    {
                        Cheats.speedOverride = false;
                        GCS.speedTrialMode = false; GCS.nextSpeedRun = 1f;
                        var c = scrController.instance; if (c != null) c.Restart(true);
                    });
                GUILayout.EndHorizontal();
            });

            GUILayout.Space(8);
            Section("便利 / 玩法");
            Cheats.noFail = GUILayout.Toggle(Cheats.noFail, " 无敌 No-Fail — 不会失败 / 不被打断");
            Indent(() => GUILayout.Label("即游戏自带的「无判定」练习模式，关卡内开启立即生效。", Lbl()));

            GUILayout.Space(6);
            Cheats.widenJudge = GUILayout.Toggle(Cheats.widenJudge, $" 放宽判定 ×{Cheats.judgeMult:0.0}（含完美窗口）");
            Indent(() =>
            {
                Cheats.judgeMult = GUILayout.HorizontalSlider(Cheats.judgeMult, 1f, 5f);
                GUILayout.Label("放大命中角度窗口（Counted/Perfect/Pure 一起），手打也容易打出完美。开 Autoplay 时本就满分，此项用于手动练习。", Lbl());
            });
        }

        // ---------------- 开发者 ----------------
        private void DrawDev()
        {
            Section("解锁 / 关卡直达");
            Cheats.forceUnlockAll = GUILayout.Toggle(Cheats.forceUnlockAll, " 解锁全部关卡（临时，不写存档）");
            Indent(() => GUILayout.Label("置 RDC.forceUnlockAllLevels；关闭即恢复，不污染存档。", Lbl()));
            if (GUILayout.Button("前往选关界面"))
                Run("GoToLevelSelect", () => ADOBase.GoToLevelSelect());
            Indent(() => GUILayout.Label("配合上面的「解锁全部」：选关界面里任意关卡都能直接进入，等于关卡直达。", Lbl()));

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("永久解锁全部（写存档）"))
                Run("PermaUnlock", () =>
                {
                    Persistence.unlockAllLevels = true;
                    RDC.forceUnlockAllLevels = true;
                    Persistence.Save();
                });
            if (GUILayout.Button("取消永久解锁"))
                Run("PermaUnlockOff", () =>
                {
                    Persistence.unlockAllLevels = false;
                    Cheats.forceUnlockAll = false;
                    RDC.forceUnlockAllLevels = false;
                    Persistence.Save();
                });
            GUILayout.EndHorizontal();
            Indent(() => GUILayout.Label("写入 Persistence.unlockAllLevels（游戏自身的设置项），不删任何已有进度，可随时取消。", Lbl()));

            GUILayout.Space(8);
            Section("过场 / 显示");
            Cheats.skipCutscenes = GUILayout.Toggle(Cheats.skipCutscenes, " 跳过过场动画");
            Cheats.showFps = GUILayout.Toggle(Cheats.showFps, " 显示 FPS");
            Cheats.staticColors = GUILayout.Toggle(Cheats.staticColors, " 固定星球颜色（录制更稳）");

            GUILayout.Space(8);
            Section("存档");
            if (GUILayout.Button("打开存档目录"))
                Run("OpenSaveDir", () =>
                {
                    try { System.Diagnostics.Process.Start("explorer.exe", Persistence.DataPath.Replace('/', '\\')); }
                    catch { Application.OpenURL("file://" + Persistence.DataPath); }
                });
        }

        // ---------------- 关于 ----------------
        private void DrawAbout()
        {
            Section("关于");
            GUILayout.Label(
                "冰与火之舞修改器 v" + Version + "\n" +
                "只调用游戏自身已有的开关（RDC / GCS / scrController / Persistence），\n" +
                "经 BepInEx + HarmonyX 注入，不做内存扫描，游戏更新不易失效。\n" +
                "与「节奏医生修改器」同源同法（同为 7th Beat Games）。", Lbl());

            GUILayout.Space(8);
            Section("免费声明");
            GUILayout.Label("本工具完全免费、开源，严禁倒卖。", Mark1());
            GUILayout.Label(
                "标题、菜单顶部与加载日志显示的项目地址即为完整性水印；删除或篡改会触发\n" +
                "校验，使修改器直接禁用。请始终从官方地址获取：\n" + Repo, Lbl());

            GUILayout.Space(8);
            Section("已知取舍");
            GUILayout.Label(
                "· 调试模式（RDC.debug）：allowDebug 只读需补丁，且会显示调试文字、不利录制，从略。\n" +
                "· 变速无法「保持音高」：引擎用 song.pitch 直接缩放，没有时间拉伸。", Lbl());

            GUILayout.Space(8);
            GUILayout.Label("热键：[" + _menuKey.Value + "] 开 / 关本菜单。", Lbl());
        }

        // ---------------- helpers ----------------
        private static void Run(string what, Action act)
        {
            try { act(); Log.LogInfo("Trainer action OK: " + what); }
            catch (Exception e) { Log.LogError("Trainer action FAILED (" + what + "): " + e); }
        }

        private static void Indent(Action body)
        {
            GUILayout.BeginHorizontal(); GUILayout.Space(18);
            GUILayout.BeginVertical(); body(); GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private static void Section(string title) => GUILayout.Label("── " + title + " ──", Hdr());

        private static GUIStyle _hdr, _lbl, _mark;
        private static GUIStyle Hdr()
        {
            if (_hdr == null) _hdr = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            return _hdr;
        }
        private static GUIStyle Lbl()
        {
            if (_lbl == null) _lbl = new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 11 };
            return _lbl;
        }
        private static GUIStyle Mark1()
        {
            if (_mark == null)
            {
                _mark = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, wordWrap = true, fontSize = 12 };
                _mark.normal.textColor = new Color(1f, 0.85f, 0.2f); // amber, stands out
            }
            return _mark;
        }
    }
}
