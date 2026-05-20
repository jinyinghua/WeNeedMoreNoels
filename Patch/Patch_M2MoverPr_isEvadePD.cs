using HarmonyLib;
using m2d;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2MoverPr), nameof(M2MoverPr.isEvadePD))]
    public class Patch_M2MoverPr_isEvadePD
    {
        [HarmonyPrefix]
        static bool Prefix(object __instance, ref bool __result)
        {
            if (__instance is ShadowNoel noel)
            {
                __result = noel.IsEvadePD;
                return false;
            }
            return true;
        }
    }
}
