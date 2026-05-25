using HarmonyLib;
using nel.mgm.smncr;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.runMain))]
    public class Patch_UiSmnCreator_runMain
    {
        static void Prefix(UiSmnCreator __instance)
        {
            if (__instance.state == (UiSmnCreator.STATE)9 && IN.isMenuPD(1))
            {
                UiMenuMul.BxSB.deactivate();
                __instance.changeState(UiSmnCreator.STATE.FILESEL);
            }
        }
    }
}
