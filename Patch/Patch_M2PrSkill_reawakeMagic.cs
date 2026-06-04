using HarmonyLib;
using nel;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2PrSkill), nameof(M2PrSkill.reawakeMagic))]
    public class Patch_M2PrSkill_reawakeMagic
    {
        [HarmonyPostfix]
        static void Postfix(object __instance, MGKIND kind)
        {
            M2PrSkill skill = (M2PrSkill)__instance;
            if (skill.Pr is PRNoel)
            {
                if (skill.CurMg == null)
                {
                    return;
                }
                NotifyNoelMagic mg = new()
                {
                    Type = NotifyMagicTpe.Reawake,
                    Kind = (int)kind,
                    T = skill.CurMg.t
                };
                WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg);
            }
        }
    }
}
