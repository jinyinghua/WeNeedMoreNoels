using HarmonyLib;
using nel;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(MagicItem), nameof(MagicItem.Sleep))]
    public class Patch_MagicItem_Sleep
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            if (DB.IsMainPR)
            {
                NotifyNoelMagic mg = new()
                {
                    Type = NotifyMagicTpe.Sleep
                };
                WNMNTools.SendMagicToAllPeers(WNMNTools.LocalID, mg);
            }
        }
    }
}
