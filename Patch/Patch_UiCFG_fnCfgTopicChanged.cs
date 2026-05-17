using HarmonyLib;
using nel;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiCFG), nameof(UiCFG.fnCfgTopicChanged), [typeof(BtnContainerRadio<aBtn>), typeof(int), typeof(int)])]
    public static class Patch_UiCFG_fnCfgTopicChanged
    {
        [HarmonyPrefix]
        static bool Prefix(UiCFG __instance, ref object __result, int cur_value)
        {
            string key;
            if (cur_value == 0) key = "MAIN";
            else if (cur_value == 1)
            {
                if (CFGSP.isSpActivated()) key = "SP";
                else key = "MP";
            }
            else key = "MP";
            __instance.fineTabVisibility(key, true, true);
            __result = true;
            return false;
        }
    }
}
