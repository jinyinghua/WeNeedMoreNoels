using HarmonyLib;
using nel.mgm.smncr;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.changeState))]
    public class Patch_UiSmnCreator_changeState
    {
        [HarmonyPrefix]
        static bool Prefix(UiSmnCreator __instance, UiSmnCreator.STATE stt)
        {
            if ((int)stt == 9)
            {
                __instance.state = stt;
                UiMenuMul.BxSB.activate();
                UiMenuMul.BxSB.Focus();
                return false;
            }
            return true;
        }
    }
}
