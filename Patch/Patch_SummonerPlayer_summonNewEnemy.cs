using HarmonyLib;
using nel;
using nel.smnp;
using UnityEngine.InputSystem;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(SummonerPlayer), nameof(SummonerPlayer.summonNewEnemy))]
    public class Patch_SummonerPlayer_summonNewEnemy
    {
        [HarmonyPrefix]
        static bool Prefix(ref NelEnemy __result, SmnEnemyKind K, out SyncState __state)
        {
            if (!DB.IsMultiplayer)
            {
                __state = null;
                return true;
            }
            if (CheckBoss(K.enemyid))
            {
                __state = new()
                {
                    IsBoss = true,
                    CanSync = true
                };
                return true;
            }
            if (WNMNTools.SyncType == EnemySyncType.StarterOnly && WNMNTools.BattleStarterID != WNMNTools.LocalID)
            {
                if (CheckBelongBoss(K.enemyid))
                {
                    __state = new()
                    {
                        IsBoss = true,
                        CanSync = false
                    };
                    return true;
                }
                __result = null;
                __state = new()
                {
                    IsBoss = false,
                    CanSync = false
                };
                return false;
            }
            if (!CheckBelongBoss(K.enemyid))
            {
                __state = new()
                {
                    IsBoss = false,
                    CanSync = true
                };
            }
            else
            {
                __state = new()
                {
                    IsBoss = false,
                    CanSync = false
                };
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(ref NelEnemy __result, SmnEnemyKind K, SyncState __state)
        {
            if (DB.IsMultiplayer && __state.CanSync)
            {
                int syncID;
                EnemySynchronizerSyncHost host;
                if (__state.IsBoss)
                {
                    if (WNMNTools.BattleStarterID == WNMNTools.LocalID && CheckBoss(K.enemyid))
                    {
                        DB.CurEnemies.Add(__result);
                        syncID = EnemySynchronizer.Unique_Sync_ID;
                        WNMNTools.SendNotifyEnemySummonToAllPeers(K.enemyid, syncID, true);
                        host = __result.gameObject.AddComponent<EnemySynchronizerBossHost>();
                        host.SyncID = syncID;
                        DB.SyncHosts.Add(syncID, host);
                    }
                    return;
                }
                DB.CurEnemies.Add(__result);
                syncID = EnemySynchronizer.Unique_Sync_ID;
                WNMNTools.SendNotifyEnemySummonToAllPeers(K.enemyid, syncID);
                host = __result.gameObject.AddComponent<EnemySynchronizerSyncHost>();
                host.SyncID = syncID;
                DB.SyncHosts.Add(syncID, host);
            }
        }

        static bool CheckBoss(string key)
        {
            NDAT.EnemyDescryption typeAndId = NDAT.getTypeAndId(key);
            if (typeAndId.EnemyType == typeof(NelNBoss_Nusi))
            {
                return true;
            }
            else if (typeAndId.EnemyType == typeof(NelNBossSpider))
            {
                return true;
            }
            return false;
        }

        static bool CheckBelongBoss(string key)
        {
            return key.ToLower().Contains("boss");
        }

        class SyncState
        {
            public bool IsBoss;

            public bool CanSync;
        }
    }
}
