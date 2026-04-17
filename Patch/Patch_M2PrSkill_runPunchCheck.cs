using HarmonyLib;
using m2d;
using nel;
using System.Reflection;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(M2PrSkill), nameof(M2PrSkill.runPunchCheck))]
    public class Patch_M2PrSkill_runPunchCheck
    {
        [HarmonyPostfix]
        static void Postfix(object __instance, ref object __result)
        {
            //M2PrSkill skill = __instance as M2PrSkill;
            //if (skill.Pr is PRNoel)
            //{
            //    return;
            //}
            //MethodInfo property = AccessTools.PropertySetter(typeof(M2PrSkill), "punch_decline_time");
            //PropertyInfo num = AccessTools.Property(typeof(M2PrSkill), "magic_t");
            //property.Invoke(__instance, [(((float)num.GetValue(__instance) > 0f) ? 2 : 26)]);
            //__result = (M2MoverPr.PR_MNP)0;
        }
    }
}
