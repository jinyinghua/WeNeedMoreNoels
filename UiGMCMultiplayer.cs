using nel.gm;
using System;
using System.ComponentModel.Design;
using UnityEngine;
using XX;

namespace WeNeedMoreNoels
{
    public class UiGMCMultiplayer : UiGMC
    {
        public UiGMCMultiplayer(UiGameMenu _GM, CATEG _categ)
            : base(_GM, _categ, true, 0, 0, 0, 0, 1f, 1f)
        {
        }

        public override bool initAppearMain()
        {
            if (base.initAppearMain())
            {
                return true;
            }
            BxR.init();
            BxR.alignx = ALIGN.CENTER;
            BxR.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = TX.Get("multiplayer_menu_title")
            });
            BxR.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            return true;
        }

        public override void initEdit()
        {
            UiBenchMenu.fineEpEvent(this.GM.Pr);
        }

        public override void quitEdit()
        {
        }

        static Color ColorDefault => Color.HSVToRGB(0, 0, 0.219f);
    }
}
