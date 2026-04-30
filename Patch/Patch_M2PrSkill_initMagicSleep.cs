using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2PrSkill), nameof(M2PrSkill.initMagicSleep))]
    public class Patch_M2PrSkill_initMagicSleep
    {
        [HarmonyPrefix]
        static void Prefix(object __instance)
        {
            M2PrSkill skill = (M2PrSkill)__instance;
            if (skill.Pr is PRNoel)
            {
                DB.IsMainPR = true;
            }
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            DB.IsMainPR = false;
        }
    }
}
