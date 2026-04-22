using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(NelM2DBase), nameof(NelM2DBase.changeMap))]
    public class Patch_NelM2DBase_changeMap
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ShadowNoelExtensions.DisableAllShadowNoels();
        }
    }
}
