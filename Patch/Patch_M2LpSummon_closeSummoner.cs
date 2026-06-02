using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpSummon), nameof(M2LpSummon.closeSummoner))]
    public class Patch_M2LpSummon_closeSummoner
    {
        [HarmonyPostfix]
        static void Postfix(M2LpSummon __instance, bool defeated)
        {
            if (defeated)
            {
                WNMNTools.SendBattleEndToAllPeers(__instance.key, WNMNTools.LocalID);
                DB.IsInBattle = false;
            }
            DB.SyncHosts.Clear();
            DB.SyncClients.Clear();
            DB.peerClients.Clear();
            DB.StartedBattleSummonerKeys.Remove(__instance.key);
            WNMNTools.BattleStarterID = -1;
            WNMNTools.SimBattleSyncList.Clear();
            WNMNTools.SimBattleReadyList.Clear();
            WNMNTools.SimBattleSyncHost = -1;
            WNMNTools.SimBattleReady = false;
        }
    }
}
