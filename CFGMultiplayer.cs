using nel;
using XX;

namespace WeNeedMoreNoels
{
    public static class CFGMultiplayer
    {
        public static bool showNicknames = true;
        public static bool showDelay = true;

        public static bool OnConfigValueChanged(aBtnMeter _B, float pre_value, float cur_value)
            => !_B.isLocked() && ChangeConfigValue(_B.title, pre_value, cur_value);

        public static void CreateBoxDesignerContentSp(UiCFG cfg, UiBoxDesigner container, Designer tab)
        {
            cfg.AddToggleSwitch("mpconfig_show_nicknames", showNicknames);
            cfg.AddToggleSwitch("mpconfig_show_delay", showDelay);

            cfg.FnDesignerCreateAfter?.Invoke(container, "MP");
        }

        private static bool ChangeConfigValue(string name, float pre_value, float cur_value)
        {
            switch (name)
            {
                case "mpconfig_show_nicknames":
                    showNicknames = cur_value != 0;
                    return true;
                case "mpconfig_show_delay":
                    showDelay = cur_value != 0;
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
