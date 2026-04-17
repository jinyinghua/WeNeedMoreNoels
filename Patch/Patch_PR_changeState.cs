using HarmonyLib;
using nel;
using WeNeedMoreNoels.CSNetworking;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PR), nameof(PR.changeState), [typeof(PR.STATE)], [ArgumentType.Normal])]
    public class Patch_PR_changeState
    {
        static void Prefix(object __instance, PR.STATE _state)
        {
            if (__instance is PRNoel)
            {
                NetworkConnectionTools.NotifyStateChange(_state);
            }
        }
    }
}
