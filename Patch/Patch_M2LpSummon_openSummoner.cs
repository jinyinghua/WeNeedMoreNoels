using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpSummon), nameof(M2LpSummon.openSummoner))]
    public class Patch_M2LpSummon_openSummoner
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            WNMNTools.SendBattleStartToAllPeers(WNMNTools.LocalID);
            DB.IsInBattle = true;
        }
    }
}
