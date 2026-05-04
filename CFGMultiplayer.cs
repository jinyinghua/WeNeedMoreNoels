using nel;
using XX;

namespace WeNeedMoreNoels
{
    public static class CFGMultiplayer
    {
        public static bool showNicknames = true;
        public static bool cureAllPlayersOnBench = true;

        public static bool OnConfigValueChanged(aBtnMeter _B, float pre_value, float cur_value)
            => !_B.isLocked() && ChangeConfigValue(_B.title, pre_value, cur_value);

        public static void CreateBoxDesignerContentSp(UiCFG cfg, UiBoxDesigner container, Designer tab)
        {
            cfg.AddToggleSwitch("mpconfig_show_nicknames", showNicknames);
            cfg.AddToggleSwitch("mpconfig_cure_all_players_on_bench", cureAllPlayersOnBench);

            cfg.FnDesignerCreateAfter?.Invoke(container, "MP"); // useless
        }

        private static bool ChangeConfigValue(string name, float pre_value, float cur_value)
        {
            Plugin.Logger.LogInfo($"ConfigValueChanged: {name} from {pre_value} to {cur_value}");
            switch (name)
            {
                case "mpconfig_show_nicknames":
                    showNicknames = cur_value != 0;
                    DB.MainPRNickname.gameObject.SetActive(showNicknames);  // TODO: if the config is persistent...
                    foreach (var noel in DB.noelIns.Values)
                    {
                        noel.NicknameIns.gameObject.SetActive(showNicknames);
                    }
                    return true;

                case "mpconfig_cure_all_players_on_bench":
                    cureAllPlayersOnBench = cur_value != 0;
                    return true;

                default:
                    return false;
            }
        }
    }

    public static class UiCFGExtension
    {
        public static aBtnMeterNel AddToggleSwitch(this UiCFG cfg, string name, bool defaultValue)
            => cfg.P("Config_" + name, true, false).addSliderCT(new DsnDataSlider
            {
                name = name,
                title = name,
                skin_title = "",
                checkbox_mode = 1,
                def = defaultValue ? 1 : 0,
                w = cfg.sliderw_sml,
                Adesc_keys = TX.GetArray("Disabled", "Enabled"),
                fnChanged = CFGMultiplayer.OnConfigValueChanged,
                fnHover = cfg.FD_fnShowDesc
            }, 214f, null, false);
    }
}
