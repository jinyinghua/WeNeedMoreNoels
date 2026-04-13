using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;

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
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            string logo = @"""
 \ \        /                                  |                                          \  |               |             |  | 
  \ \  \   /  _ \       __ \    _ \   _ \   _` |       __ `__ \    _ \    __|  _ \         \ |   _ \    _ \  |   __|       |  | 
   \ \  \ /   __/       |   |   __/   __/  (   |       |   |   |  (   |  |     __/       |\  |  (   |   __/  | \__ \      _| _| 
    \_/\_/  \___|      _|  _| \___| \___| \__,_|      _|  _|  _| \___/  _|   \___|      _| \_| \___/  \___| _| ____/      _) _) 
            """;
            Logger.LogInfo(logo);
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
