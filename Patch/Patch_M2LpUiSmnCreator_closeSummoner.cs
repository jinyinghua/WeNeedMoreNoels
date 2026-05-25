using HarmonyLib;
using nel.mgm.smncr;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpUiSmnCreator), nameof(M2LpUiSmnCreator.closeSummoner))]
    public class Patch_M2LpUiSmnCreator_closeSummoner
    {
        [HarmonyPostfix]
        static void Postfix(M2LpUiSmnCreator __instance, bool defeated)
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
        }
    }
}
