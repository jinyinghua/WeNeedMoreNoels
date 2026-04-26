using HarmonyLib;
using nel;
using WeNeedMoreNoels.SN;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(MTR), nameof(MTR.initPrAnimatorPxl))]
    public class Patch_MTR_initPrAnimatorPxl
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            DB.PreloadResource = true;
            MTRExtension.LoadAllPxls();
            DB.PreloadResource = false;
        }
    }
}
