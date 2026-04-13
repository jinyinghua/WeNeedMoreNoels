using HarmonyLib;
using nel;

namespace WeNeedMoreNoels
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
