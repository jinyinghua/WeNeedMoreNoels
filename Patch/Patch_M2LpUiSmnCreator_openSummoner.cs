using HarmonyLib;
using nel.mgm.smncr;
using System.Linq;
using UnityEngine;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpUiSmnCreator), nameof(M2LpUiSmnCreator.openSummoner))]
    public class Patch_M2LpUiSmnCreator_openSummoner
    {
        [HarmonyPrefix]
        static bool Prefix(M2LpUiSmnCreator __instance)
        {
            WNMNTools.TotalBattleNoelCount = DB.noelIns.Count(x => x.Value.Enabled);
            if (DB.StartedBattleSummonerKeys.Contains(__instance.key))
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(M2LpUiSmnCreator __instance)
        {
            if (WNMNTools.BattleStarterID == -1)
            {
                WNMNTools.BattleStarterID = WNMNTools.LocalID;
                WNMNTools.SendSimBattleStartToAllPeers(WNMNTools.LocalID);
            }
            WNMNTools.BattleStartT = Time.time;
            DB.IsInBattle = true;
            DB.CurEnemies.Clear();
            DB.StartedBattleSummonerKeys.Add(__instance.key);
        }
    }
}
