using HarmonyLib;
using nel;
using UnityEngine;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpSummon), nameof(M2LpSummon.openSummoner))]
    public class Patch_M2LpSummon_openSummoner
    {
        [HarmonyPrefix]
        static bool Prefix(M2LpSummon __instance)
        {
            WNMNTools.TotalBattleNoelCount = WNMNTools.GetBattleNoelCounts(__instance);
            if (DB.StartedBattleSummonerKeys.Contains(__instance.key))
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(M2LpSummon __instance)
        {
            if (WNMNTools.BattleStarterID == -1)
            {
                WNMNTools.BattleStarterID = WNMNTools.LocalID;
                WNMNTools.SendBattleStartToAllPeers(__instance.key, WNMNTools.LocalID);
            }
            WNMNTools.BattleStartT = Time.time;
            DB.IsInBattle = true;
            DB.CurEnemies.Clear();
            DB.StartedBattleSummonerKeys.Add(__instance.key);
        }
    }
}
