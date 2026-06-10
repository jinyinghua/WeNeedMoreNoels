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
            UiMenuMul.BxSB = __instance.DsFam.Create("BxSB", 0f, 0f, IN.w * 0.7f, IN.h * 0.7f, 1, 40f, UiBoxDesignerFamily.MASKTYPE.BOX);
            UiMenuMul.BxSB.use_scroll = true;
            UiBoxDesigner BxCmd = UiMenuMul.BxSB;
            BxCmd.Focusable(true, true, null);
            BxCmd.init();
            BxCmd.deactivate();
            UiMenuMul.BxSSI = __instance.DsFam.Create("BxSSI", 0f, 0f, 680f, 250f, 1, 40f, UiBoxDesignerFamily.MASKTYPE.BOX);
            BxCmd = UiMenuMul.BxSSI;
            BxCmd.init();
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30f,
                text = TX.Get("Desc_multiplayer_simbattle_invalidspawn")
            });
            BxCmd.deactivate();
            UiMenuMul.BxSL = __instance.DsFam.Create("BxSL", 0f, 0f, 640f, 130f, 1, 40f, UiBoxDesignerFamily.MASKTYPE.BOX);
            WNMNTools.USC = __instance;
            UiMenuMul.BxSS = __instance.DsFam.Create("BxSS", 0f, 0f, IN.w * 0.5f, IN.h * 0.4f, 1, 40f, UiBoxDesignerFamily.MASKTYPE.BOX);
            BxCmd = UiMenuMul.BxSS;
            BxCmd.use_scroll = true;
        }

        static Color ColorDefault => Color.HSVToRGB(0, 0, 0.219f);
    }
}
