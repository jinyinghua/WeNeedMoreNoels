using HarmonyLib;
using nel;
using nel.smnp;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(SummonerPlayer), nameof(SummonerPlayer.summonNewEnemy))]
    public class Patch_SummonerPlayer_summonNewEnemy
    {
        [HarmonyPrefix]
        static bool Prefix(ref NelEnemy __result, out bool __state)
        {
            if (WNMNTools.SyncType == EnemySyncType.StarterOnly && WNMNTools.BattleStarterID != WNMNTools.LocalID)
            {
                __result = null;
                __state = false;
                return false;
            }
            __state = true;
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(ref NelEnemy __result, SmnEnemyKind K, bool __state)
        {
            if (DB.IsMultiplayer && __state)
            {
                DB.CurEnemies.Add(__result);
                int syncID = EnemySynchronizer.Unique_Sync_ID;
                WNMNTools.SendNotifyEnemySummonToAllPeers(K.enemyid, syncID);
                var host = __result.gameObject.AddComponent<EnemySynchronizerSyncHost>();
                host.SyncID = syncID;
                DB.SyncHosts.Add(syncID, host);
            }
        }
    }
}
