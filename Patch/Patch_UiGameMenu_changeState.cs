using HarmonyLib;
using nel.gm;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiGameMenu), nameof(UiGameMenu.changeState))]
    public class Patch_UiGameMenu_changeState
    {
        [HarmonyPrefix]
        static bool Prefix(UiGameMenu __instance)
        {
            if (UiMenuMul.IsMulCata)
            {
                __instance.BxCategory.getBtn(10).Select(true);
                UiMenuMul.IsMulCata = false;
                return false;
            }
            return true;
        }
    }
}
