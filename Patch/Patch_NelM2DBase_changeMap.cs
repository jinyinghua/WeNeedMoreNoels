using HarmonyLib;
using nel;
using WeNeedMoreNoels.CSNetworking;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(NelM2DBase), nameof(NelM2DBase.changeMap))]
    public class Patch_NelM2DBase_changeMap
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            NetworkConnectionTools.NotifyChangeMapBefore();
            ShadowNoelExtensions.DisableAllShadowNoels();
        }
    }
}
