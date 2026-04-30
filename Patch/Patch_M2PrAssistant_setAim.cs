using HarmonyLib;
using nel;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2PrAssistant), nameof(M2PrAssistant.setAim))]
    public class Patch_M2PrAssistant_setAim
    {
        [HarmonyPrefix]
        static bool Prefix(object __instance)
        {
            M2PrAssistant assistant = (M2PrAssistant)__instance;
            if (assistant.Pr is ShadowNoel)
            {
                return false;
            }
            return true;
        }
    }
}
