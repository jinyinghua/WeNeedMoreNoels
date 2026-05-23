using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(NAI), nameof(NAI.consider), [typeof(float), typeof(float)])]
    public class Patch_NAI_consider
    {
        [HarmonyPrefix]
        static bool Prefix(NAI __instance)
        {
            if (DB.IsMultiplayer)
            {
                if (__instance.En.gameObject.GetComponent<EnemySynchronizerClient>())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
    }
}
