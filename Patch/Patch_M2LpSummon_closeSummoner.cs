using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2LpSummon), nameof(M2LpSummon.closeSummoner))]
    public class Patch_M2LpSummon_closeSummoner
    {
        [HarmonyPostfix]
        static void Postfix(bool defeated)
        {
            if (defeated)
            {
                WNMNTools.SendBattleEndToAllPeers(WNMNTools.LocalID);
            }
        }
    }
}
