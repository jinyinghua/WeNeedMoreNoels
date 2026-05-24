using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpSummon), nameof(M2LpSummon.openSummoner))]
    public class Patch_M2LpSummon_openSummoner
    {
        [HarmonyPrefix]
        static void Prefix(M2LpSummon __instance)
        {
            WNMNTools.TotalBattleNoelCount = WNMNTools.GetBattleNoelCounts(__instance);
        }

        [HarmonyPostfix]
        static void Postfix(M2LpSummon __instance)
        {
            if (WNMNTools.BattleStarterID == -1)
            {
                WNMNTools.BattleStarterID = WNMNTools.LocalID;
                WNMNTools.SendBattleStartToAllPeers(WNMNTools.LocalID);
            }
            DB.IsInBattle = true;
            DB.CurEnemies.Clear();
        }
    }
}
