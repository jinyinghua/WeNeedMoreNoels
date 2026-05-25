using HarmonyLib;
using nel;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(CoinStorage), nameof(CoinStorage.reduceCount))]
    public class Patch_CoinStorage_reduceCount
    {
        [HarmonyPostfix]
        static void Postfix(int v, CoinStorage.CTYPE ctype)
        {
            if (!DB.IsMultiplayer)
            {
                return;
            }
            WNMNTools.SendLoseCoinToAllPeers(ctype, v);
        }
    }
}
