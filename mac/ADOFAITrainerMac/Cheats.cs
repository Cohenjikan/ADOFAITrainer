namespace ADOFAITrainerMac
{
    // Shared runtime state. Read each frame by Trainer.ApplyState() (and by the Harmony patch
    // in Patches.cs) and drawn by OnGUI(). Every field maps to ONE switch the game already
    // exposes — no memory offsets, so it survives game updates. (Ported verbatim from the
    // Windows/BepInEx trainer's Cheats.cs — platform-independent.)
    internal static class Cheats
    {
        // ---- 普通玩家 ----
        public static bool autoplay = false;   // RDC.auto  —— 全程满分自动演奏（主游戏无水印）
        public static bool noHud = false;      // RDC.noHud —— 隐藏 HUD，干净录制（= 游戏自带 d_recording 的一半）
        public static bool noFail = false;     // GCS.useNoFail + scrController.noFail —— 不死/不被打断

        // 变速：GCS.speedTrialMode + GCS.nextSpeedRun，在关卡 Start 时应用（含音高），需重开生效。
        public static bool speedOverride = false;
        public static float speed = 1.0f;      // 0.5–3.0

        // 放宽判定：Harmony 后缀放大 scrMisc.GetAdjustedAngleBoundaryInDeg 的返回值，
        // 同时加宽 Counted / Perfect / Pure 三个窗口（连"完美"一起放宽）。
        public static bool widenJudge = false;
        public static float judgeMult = 3.0f;  // 1.0–5.0

        // ---- 开发者 ----
        public static bool forceUnlockAll = false; // RDC.forceUnlockAllLevels —— 临时解锁，不写存档
        public static bool skipCutscenes = false;  // RDC.skipCutscenes —— 跳过过场动画
        public static bool showFps = false;        // GCS.showFPS —— 显示帧率
        public static bool staticColors = false;   // GCS.staticPlanetColors —— 固定星球颜色（录制更稳）
    }
}
