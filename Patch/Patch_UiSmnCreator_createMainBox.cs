using HarmonyLib;
using nel;
using nel.mgm.smncr;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.createMainBox))]
    public class Patch_UiSmnCreator_createMainBox
    {
        [HarmonyPostfix]
        static void Postfix(UiSmnCreator __instance)
        {
            UiMenuMul.BxSB = __instance.DsFam.Create("BxSB", 0f, 0f, IN.w * 0.7f, IN.h * 0.7f, 1, 40f, UiBoxDesignerFamily.MASKTYPE.BOX);
            UiBoxDesigner BxCmd = UiMenuMul.BxSB;
            BxCmd.Focusable(true, true, null);
            BxCmd.init();
            BxCmd.deactivate();
            WNMNTools.USC = __instance;
        }
    }
}
