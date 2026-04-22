using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PR), nameof(PR.changeState), [typeof(PR.STATE)], [ArgumentType.Normal])]
    public class Patch_PR_changeState
    {
        static void Prefix(object __instance, PR.STATE _state)
        {
            if (__instance is PRNoel)
            {
                //TODO:改状态
            }
        }
    }
}
