using HarmonyLib;
using nel;
using nel.mgm.smncr;
using UnityEngine;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.createMainBox))]
    public class Patch_UiSmnCreator_createMainBox
    {
        [HarmonyPostfix]
        static void Postfix(UiSmnCreator __instance)
        {
            UiMenuMul.BxSB = __instance.DsFam.Create("BxSB", 0f, 0f, IN.w * 0.6f, IN.h * 0.5f, 1, 40f, UiBoxDesignerFamily.MASKTYPE.BOX);
            UiBoxDesigner BxCmd = UiMenuMul.BxSB;
            BxCmd.Clear();
            BxCmd.Focusable(true, true, null);
            BxCmd.init();
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = TX.Get("multiplayer_simbattle_title")
            });
            BxCmd.deactivate();
        }

        static Color ColorDefault => Color.HSVToRGB(0, 0, 0.219f);
    }
}
