using HarmonyLib;

namespace ADOFAITrainerMac
{
    // Widen the hit-judgement window. scrMisc.GetAdjustedAngleBoundaryInDeg returns the
    // Counted / Perfect / Pure angle boundaries used by GetHitMargin to classify a tap
    // (the Perfect branch is the hardcoded `45.0 * marginMult`). Scaling the returned
    // boundary widens ALL three, so manual play scores Perfect even when slightly off.
    //
    // Autoplay already perfect-hits every tile, so this is purely for manual practice. It is
    // the trainer's only Harmony patch — everything else is a plain field/property set.
    // (Ported from the Windows trainer; only the integrity flag reference changed Plugin→Trainer.)
    [HarmonyPatch(typeof(scrMisc), nameof(scrMisc.GetAdjustedAngleBoundaryInDeg))]
    internal static class JudgeWindowPatch
    {
        private static void Postfix(ref double __result)
        {
            if (Trainer.Ok && Cheats.widenJudge)
                __result *= System.Math.Max(1.0, Cheats.judgeMult);
        }
    }
}
