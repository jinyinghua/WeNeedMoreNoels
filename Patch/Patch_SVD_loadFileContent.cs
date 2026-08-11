using HarmonyLib;
using nel;
using PixelLiner.PixelLinerLib;

using System.IO;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(SVD), nameof(SVD.loadFileContent))]
    public class Patch_SVD_loadFileContent
    {
        [HarmonyPrefix]
        static bool Prefix(SVD.sFile Sf, ref object __result)
        {
            if (Sf.index == -2)
            {
                __result = new ByteArray(NKT.readSpecificFileBinary(Path.Combine(SVD.getDir(), DB.SYNC_FILE_NAME), 0, 0, false), false, false);
                return false;
            }
            return true;
        }
    }
}
