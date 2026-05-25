using HarmonyLib;
using nel.mgm.smncr;
using PixelLiner.PixelLinerLib;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmncBattleConfirm), nameof(UiSmncBattleConfirm.activate))]
    public class Patch_UiSmncBattleConfirm_activate
    {
        static void Prefix(SmncFile _CurFile)
        {
            ByteArray array = new();
            _CurFile.writeBinaryTo(array);
            DB.SyncSmnContentBuffer = array.bytes;
        }
    }
}
