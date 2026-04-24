using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using WeNeedMoreNoels.Networking;

namespace WeNeedMoreNoels
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private Harmony _harmony;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            string logo = @"""
 \ \          \    \   \  |  |  | 
  \ \  \   /   \ | \/   \ |  |  | 
   \ \  \ /  |\  |    |\  | _| _| 
    \_/\_/  _| \_|   _| \_| _) _)
            """;
            Logger.LogMessage(logo);
            Logger.LogMessage("Created by Alon_, Created at 2026-4-13, Happy birthday to myself");
            MTRExtension.Load();
            ReceiveMessageManager.Init();
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
