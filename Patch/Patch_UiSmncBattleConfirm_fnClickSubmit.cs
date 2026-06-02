using HarmonyLib;
using nel.mgm.smncr;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmncBattleConfirm), nameof(UiSmncBattleConfirm.fnClickSubmit))]
    public class Patch_UiSmncBattleConfirm_fnClickSubmit
    {
        static bool Prefix(UiSmncBattleConfirm __instance, aBtn B)
        {
            if (WNMNTools.SimBattleSyncHost != -1 && B.title == "&&Smnc_start_battle_submit")
            {
                B.SetLocked(true);
                return true;
            }
            else
            {
                if (DB.IsMultiplayer && B.title == "&&Smnc_start_battle_submit")
                {
                    SimBattle battle = new()
                    {
                        Type = NotifySimBattleType.StartHost
                    };
                    WNMNTools.SendNotifySimBattleToAllPeers(battle);
                    SimBattle battle1 = new()
                    {
                        Type = NotifySimBattleType.ConnectHost
                    };
                    WNMNTools.SendNotifySimBattleToAllPeers(battle1);
                    WNMNTools.SimBattleSyncHost = WNMNTools.LocalID;
                    WNMNTools.SimBattleReady = true;
                    __instance.deactivate();
                    __instance.Con.changeState((UiSmnCreator.STATE)9);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
