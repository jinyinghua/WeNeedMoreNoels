using HarmonyLib;
using nel;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PR), nameof(PR.applyGasDamage))]
    public class Patch_PR_applyGasDamage
    {
        [HarmonyPrefix]
        static bool Prefix(PR __instance, ref int __result)
        {
            if (__instance is ShadowNoel)
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}
