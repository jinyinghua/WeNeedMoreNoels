using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(NelItemManager), nameof(NelItemManager.getItem))]
    public class Patch_NelItemManager_getItem
    {
        [HarmonyPostfix]
        static void Postfix(NelItem Itm, int count, int grade)
        {
            if (!DB.IsMultiplayer)
            {
                return;
            }
            string id = Itm.key;
            WNMNTools.SendGetItemToAllPeers(id, count, grade);
        }
    }
}
