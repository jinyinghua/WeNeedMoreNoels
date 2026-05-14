using m2d;
using nel;
using PixelLiner;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
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

        static Dictionary<ColorNoelColor, PrPoseContainer> colorDics = [];

        public static string GetColorNoelName(ColorNoelColor color) => "noel_magic_" + color.ToString().ToLower();

        public static string[][] GetColorNoelPxlsFull(ColorNoelColor color) => [["noel", GetColorNoelName(color), "noel_r18"]];

        public static string[][] GetColorNoelPxls(ColorNoelColor color) => [[GetColorNoelName(color)]];

        public static PrPoseContainer GetPrPoseContainer(ColorNoelColor color) => colorDics[color];

        public static PrPoseContainer PConNoelIAnim;

        public const string LOCALIZATION_FILE_NAME = "_wnmn_localization";

        public static void Load()
        {
            Plugin.Logger.LogInfo("start loading WNMN resources..");
            string assetPath = Path.GetFullPath(Application.streamingAssetsPath);
            string assetOriginPath = Path.Combine(assetPath, "WNMNResources\\");
            string localPxlPath = Path.Combine(assetOriginPath, "pxls\\");
            string localLocalizationPath = Path.Combine(assetPath, "localization\\");
            string L_zhPath = Path.Combine(localLocalizationPath, "zh-cn\\");
            string L_zhtcPath = Path.Combine(localLocalizationPath, "zh-tc\\");
            string L_enPath = Path.Combine(localLocalizationPath, "en\\");
            string L_jpPath = Path.Combine(localLocalizationPath, "_\\");
            string pluginFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx", "plugins");
            string pluginPath = Path.Combine(pluginFolderPath, "WNMN");
            string pluginPxlPath = Path.Combine(pluginPath, "pxls\\");
            string pluginResPath = Path.Combine(pluginPath, "resources\\");
            string zhPath = Path.Combine(pluginPath, "zh-cn\\");
            string zhtcPath = Path.Combine(pluginPath, "zh-tc\\");
            string enPath = Path.Combine(pluginPath, "en\\");
            string jpPath = Path.Combine(pluginPath, "_\\");
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
            foreach (FileInfo file in new DirectoryInfo(pluginResPath).EnumerateFiles())
            {
                string targetFile = assetOriginPath + file.Name;
                File.Copy(file.FullName, targetFile, true);
            }
            File.Copy(zhPath + $"zh-cn{LOCALIZATION_FILE_NAME}.txt", L_zhPath + $"zh-cn{LOCALIZATION_FILE_NAME}.txt", true);
            File.Copy(zhtcPath + $"zh-tc{LOCALIZATION_FILE_NAME}.txt", L_zhtcPath + $"zh-tc{LOCALIZATION_FILE_NAME}.txt", true);
            File.Copy(enPath + $"en{LOCALIZATION_FILE_NAME}.txt", L_enPath + $"en{LOCALIZATION_FILE_NAME}.txt", true);
            File.Copy(jpPath + $"_{LOCALIZATION_FILE_NAME}.txt", L_jpPath + $"_{LOCALIZATION_FILE_NAME}.txt", true);
            Plugin.Logger.LogInfo("WNMN resources load complete!");
        }

        public static void LoadAllPxls()
        {
            PConNoelIAnim = LoadExtenalPxl(Anoel_inverse_pxls, "noel_inverse");
            for (int i = 0; i < 8; i++)
            {
                ColorNoelColor color = (ColorNoelColor)i;
                colorDics.Add(color, LoadExtenalPxl(GetColorNoelPxls(color), GetColorNoelName(color)));
            }
        }

        public static PrPoseContainer LoadExtenalPxl(string[][] pxlPath, string name)
        {
            LoadTicketManager.PrepareLoadManager();
            LoadTicketManager instance = LoadTicketManager.Instance;
            int num = pxlPath.Length;
            for (int i = 0; i < num; i++)
            {
                int num2 = pxlPath[i].Length;
                for (int j = 0; j < num2; j++)
                {
                    string text = "WNMNResources/pxls/" + pxlPath[i][j] + ".pxls";
                    MTIOneImage mtioneImage;
                    PxlCharacter pxlCharacter = MTRX.loadMtiPxc(out mtioneImage, pxlPath[i][j], text, "_", true, true, true);
                    instance.AddTicketInner(pxlCharacter, mtioneImage, 1);
                }
            }
            CaneManager.reloadScript(false);
            return new PrPoseContainer(name, delegate (PxlFrame F, float rCLENB)
            {
                float num3;
                float num4;
                return M2PxlAnimator.getRodPosS(rCLENB, F, out num3, out num4, "rod", "ROD", 0.5f, 0f, ALIGN.LEFT, ALIGNY.MIDDLE, 2, "rodeff");
            });
        }
    }
}
