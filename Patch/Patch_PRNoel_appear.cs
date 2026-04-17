using HarmonyLib;
using m2d;
using nel;
using WeNeedMoreNoels.CSNetworking;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(PRNoel), nameof(PRNoel.appear))]
    public class Patch_PRNoel_appear
    {
        [HarmonyPostfix]
        static void Postfix(Map2d Mp)
        {
            NetworkConnectionTools.NotifyChangeMapAfter(Mp.key);
            ShadowNoelExtensions.DisableAllShadowNoels();
            ShadowNoelExtensions.DetectShadowNoelInCurrentMap();
        }
    }
}
