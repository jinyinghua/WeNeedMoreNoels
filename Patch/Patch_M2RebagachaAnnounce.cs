using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2RebagachaAnnounce), nameof(M2RebagachaAnnounce.initG))]
    public static class Patch_M2RebagachaAnnounce_initG
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return !DB.ShadowAppear;
        }
    }
}
