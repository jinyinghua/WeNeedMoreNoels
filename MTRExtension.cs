using m2d;
using nel;
using PixelLiner;
using System;
using System.IO;
using UnityEngine;
using XX;

namespace WeNeedMoreNoels
{
    public static class MTRExtension
    {
        public static string[][] Anoel_inverse_pxls
        {
            get =>
            [
                ["noel_inverse", "noel_inverse_r18", "noel_inverse_magic"]
            ];
        }

        public static PrPoseContainer PConNoelIAnim;

        public static void Load()
        {
            Plugin.Logger.LogInfo("start loading WNMN resources..");
            string assetPath = Path.GetFullPath(Application.streamingAssetsPath);
            string assetOriginPath = Path.Combine(assetPath, "WNMNResources\\");
            string localPxlPath = Path.Combine(assetOriginPath, "pxls\\");
            string pluginFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx", "plugins");
            string pluginPath = Path.Combine(pluginFolderPath, "WNMN");
            string pluginPxlPath = Path.Combine(pluginPath, "pxls\\");
            DB.Plugin_local_path = pluginPath;
            DB.Game_streaming_asset = assetOriginPath;
            if (!Directory.Exists(assetOriginPath))
            {
                Directory.CreateDirectory(assetOriginPath);
            }
            if (!Directory.Exists(localPxlPath))
            {
                Directory.CreateDirectory(localPxlPath);
            }
            Plugin.Logger.LogInfo("Directory check success.");
            foreach (FileInfo file in new DirectoryInfo(pluginPxlPath).EnumerateFiles())
            {
                string targetFile = localPxlPath + file.Name;
                File.Copy(file.FullName, targetFile, true);
            }
            Plugin.Logger.LogInfo("WNMN resources load complete!");
        }

        public static void LoadExtenalPxl()
        {
            string[][] anoel_inverse_pxls = Anoel_inverse_pxls;
            LoadTicketManager instance = LoadTicketManager.Instance;
            int num = anoel_inverse_pxls.Length;
            for (int i = 0; i < num; i++)
            {
                int num2 = anoel_inverse_pxls[i].Length;
                for (int j = 0; j < num2; j++)
                {
                    string text = "WNMNResources/pxls/" + anoel_inverse_pxls[i][j] + ".pxls";
                    MTIOneImage mtioneImage;
                    PxlCharacter pxlCharacter = MTRX.loadMtiPxc(out mtioneImage, anoel_inverse_pxls[i][j], text, "_", true, true, true);
                    instance.AddTicketInner(pxlCharacter, mtioneImage, 0);
                }
            }
            PConNoelIAnim = new PrPoseContainer("noel_inverse", delegate (PxlFrame F, float rCLENB)
            {
                float num3;
                float num4;
                return M2PxlAnimator.getRodPosS(rCLENB, F, out num3, out num4, "rod", "ROD", 0.5f, 0f, ALIGN.LEFT, ALIGNY.MIDDLE, 2, "rodeff");
            });
        }
    }
}
