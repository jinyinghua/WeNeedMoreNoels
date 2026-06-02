using HarmonyLib;
using nel.mgm.smncr;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.runMain))]
    public class Patch_UiSmnCreator_runMain
    {
        [HarmonyPrefix]
        static void Prefix(UiSmnCreator __instance)
        {
            if (__instance.state == (UiSmnCreator.STATE)9 && IN.isMenuPD(1))
            {
                if (WNMNTools.SimBattleSyncHost == WNMNTools.LocalID)
                {
                    SimBattle battle = new()
                    {
                        Type = NotifySimBattleType.CloseHost
                    };
                    WNMNTools.SendNotifySimBattleToAllPeers(battle);
                    WNMNTools.SimBattleSyncHost = -1;
                }
                else
                {
                    SimBattle battle = new()
                    {
                        Type = NotifySimBattleType.DisconnectHost
                    };
                    WNMNTools.SendNotifySimBattleToAllPeers(battle);
                }
                UiMenuMul.BxSB.deactivate();
                __instance.changeState(UiSmnCreator.STATE.FILESEL);
                WNMNTools.SimBattleSyncList.Clear();
                WNMNTools.SimBattleReadyList.Clear();
            }
        }
    }
}
