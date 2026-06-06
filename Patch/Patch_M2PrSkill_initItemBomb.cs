using HarmonyLib;
using nel;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2PrSkill), nameof(M2PrSkill.initItemBomb))]
    public class Patch_M2PrSkill_initItemBomb
    {
        [HarmonyPostfix]
        static void Postfix(M2PrSkill __instance, NelItem Itm, int grade)
        {
            if (__instance.Pr is not PRNoel)
            {
                return;
            }
            NotifyNoelMagic mg = new()
            {
                Type = NotifyMagicTpe.InitBomb,
                Key = Itm.key,
                Grade = grade
            };
            WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg);
        }
    }
}
