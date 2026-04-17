using HarmonyLib;
using m2d;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(Map2d), nameof(Map2d.assignCenterPlayer))]
    public static class Patch_Map2d_assignCenterPlayer
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return !DB.ShadowAppear;
        }
    }
}
