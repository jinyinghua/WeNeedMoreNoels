using HarmonyLib;
using nel.mgm.smncr;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmncBattleConfirm), nameof(UiSmncBattleConfirm.activate))]
    public class Patch_UiSmncBattleConfirm_activate
    {
        [HarmonyPrefix]
        static void Prefix(UiSmncBattleConfirm __instance, SmncFile _CurFile)
        {
            UiMenuMul.BxSSI.deactivate();
            WNMNTools.USBC = __instance;
            WNMNTools.CurSimFile = _CurFile;
            if (WNMNTools.SpawnDic.ContainsKey(-1))
            {
                WNMNTools.SpawnDic[-1] = new(WNMNTools.CurSimFile.Astgo[0].x, WNMNTools.CurSimFile.Astgo[0].y);
            }
            else
            {
                WNMNTools.SpawnDic.Add(-1, new(WNMNTools.CurSimFile.Astgo[0].x, WNMNTools.CurSimFile.Astgo[0].y));
            }
        }
    }
}
