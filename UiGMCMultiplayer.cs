using nel;
using nel.gm;
using System;
using System.Collections.Generic;
using System.Linq;
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
            BxR.alignx = ALIGN.CENTER;
            BxR.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = TX.Get("multiplayer_menu_utilities")
            });
            BxR.Br();
            BxR.addButton(new()
            {
                title = TX.Get("multiplayer_menu_modify_nickname"),
                fnClick = B =>
                {
                    UiBoxDesigner BxCmd = UiMenuMul.BxP;
                    BxCmd.activate();
                    IN.setZ(BxCmd.transform, BxR.transform.position.z - 1f);
                    BxCmd.Clear();
                    BxCmd.getBox().frametype = UiBox.FRAMETYPE.ONELINE;
                    BxCmd.WH(300f, 150f);
                    BxCmd.margin_in_lr = 10f;
                    BxCmd.margin_in_tb = 10f;
                    BxCmd.init();
                    BxCmd.alignx = ALIGN.CENTER;
                    var nicknameInput = BxCmd.addInput(new()
                    {
                        h = 20f
                    });
                    nicknameInput.text = DB.InitConfig.nickName;
                    BxCmd.Br();
                    BxCmd.addButton(new()
                    {
                        title = TX.Get("Submit"),
                        fnClick = B =>
                        {
                            WNMNTools.SendUpdatePeerInfoToAllPeers(WNMNTools.LocalID, nicknameInput.text);
                            BxCmd.deactivate();
                            BxR.Focus();
                            return true;
                        }
                    });
                    BxCmd.addButton(new()
                    {
                        title = TX.Get("Cancel"),
                        fnClick = B =>
                        {
                            BxCmd.deactivate();
                            BxR.Focus();
                            return true;
                        }
                    });
                    Vector3 btnPos = B.transform.position;
                    float targetX = btnPos.x * 64f + 300f;
                    float targetY = btnPos.y * 64f;
                    BxCmd.posSetDA(targetX, targetY, 0, 20f, true);
                    BxCmd.Focusable(true, true);
                    BxCmd.Focus();
                    return true;
                }
            });
            BxR.addP(new()
            {
                text = "  "
            });
            BxR.addButton(new()
            {
                title = TX.Get("multiplayer_menu_modify_party"),
                fnClick = B =>
                {
                    OnPlayerSelect(B, i =>
                    {
                        WNMNTools.SendUpdatePeerInfoToAllPeers(WNMNTools.LocalID, i);
                    });
                    return true;
                }
            });
            BxR.Br();
            BxR.addButton(new()
            {
                title = TX.Get("multiplayer_menu_teleport"),
                fnClick = B =>
                {
                    OnPlayerSelect(B, i =>
                    {
                        string targetKey = DB.noelIns[i].MpKey;
                        float x = DB.noelIns[i].NoelInfo.PositionX;
                        float y = DB.noelIns[i].NoelInfo.PositionY;
                        WNMNTools.TransferMainNoel(targetKey, x, y);
                        GM.deactivate();
                    });
                    return true;
                }
            });
            BxR.addP(new()
            {
                text = "  "
            });
            BxR.addButton(new()
            {
                title = TX.Get("multiplayer_menu_syncbackpackandmony"),
                fnClick = B =>
                {
                    OnPlayerSelect(B, i =>
                    {
                        UiBoxDesigner BxCmd = UiMenuMul.BxP;
                        BxCmd.activate();
                        IN.setZ(BxCmd.transform, BxR.transform.position.z - 1f);
                        BxCmd.Clear();
                        BxCmd.getBox().frametype = UiBox.FRAMETYPE.ONELINE;
                        BxCmd.WH(300f, 150f);
                        BxCmd.margin_in_lr = 10f;
                        BxCmd.margin_in_tb = 10f;
                        BxCmd.init();
                        BxCmd.alignx = ALIGN.CENTER;
                        BxCmd.addP(new()
                        {
                            text = "multiplayer_menu_syncbackpackandmony_warn",
                            TxCol = Color.HSVToRGB(0, 0.95f, 0.91f)
                        });
                        BxCmd.Br();
                        BxCmd.addButton(new()
                        {
                            title = TX.Get("Submit"),
                            fnClick = B =>
                            {
                                BxCmd.deactivate();
                                BxR.Focus();
                                return true;
                            }
                        });
                        BxCmd.addButton(new()
                        {
                            title = TX.Get("Cancel"),
                            fnClick = B =>
                            {
                                BxCmd.deactivate();
                                BxR.Focus();
                                return true;
                            }
                        });
                        Vector3 btnPos = B.transform.position;
                        float targetX = btnPos.x * 64f + 300f;
                        float targetY = btnPos.y * 64f;
                        BxCmd.posSetDA(targetX, targetY, 0, 20f, true);
                        BxCmd.Focusable(true, true);
                        BxCmd.Focus();
                    });
                    return true;
                }
            });
            BxR.Br();
            BxR.alignx = ALIGN.CENTER;
            UiMenuMul.SendMsgButton = BxR.addButton(new()
            {
                title = TX.Get("multiplayer_menu_sendmsg"),
                fnClick = B =>
                {
                    string[] titles = [.. msgs.Select(x => TX.Get(x)), TX.Get("Cancel")];
                    if (DB.Mute)
                    {
                        Mutted(B, UiMenuMul.BxP);
                        return true;
                    }
                    UiBoxDesigner BxCmd = UiMenuMul.BxP;
                    BxCmd.activate();
                    IN.setZ(BxCmd.transform, BxR.transform.position.z - 1f);
                    BxCmd.Clear();
                    BxCmd.getBox().frametype = UiBox.FRAMETYPE.ONELINE;
                    BxCmd.WH(200f, 48f * titles.Length);
                    BxCmd.margin_in_lr = 10f;
                    BxCmd.margin_in_tb = 10f;
                    BxCmd.init();
                    var BCon = BxCmd.addButtonMultiT<aBtnNel>(new DsnDataButtonMulti
                    {
                        name = "sub_menu",
                        titles = titles,
                        skin = "row_center",
                        clms = 1,
                        w = BxCmd.use_w,
                        h = 30f,
                        fnClick = (aBtn BSub) =>
                        {
                            if (BSub.title != TX.Get("Cancel"))
                            {
                                WNMNTools.BroadcastMsg(msgs[titles.IndexOf(BSub.title)]);
                                BxCmd.deactivate();
                                BSub.Select(true);
                                BxR.Focus();
                                GM.deactivate();
                            }
                            else
                            {
                                BxCmd.deactivate();
                                BSub.Select(true);
                                BxR.Focus();
                            }
                            return true;
                        }
                    });
                    BCon.Get(0).Select(true);
                    Vector3 btnPos = B.transform.position;
                    float targetX = btnPos.x * 64f + 300f;
                    float targetY = btnPos.y * 64f;
                    BxCmd.posSetDA(targetX, targetY, 0, 20f, true);
                    BxCmd.Focusable(true, true);
                    BxCmd.Focus();
                    return true;
                }
            });
            BxR.Br();
            if (WNMNTools.Type == NetWorkType.Host)
            {
                BxR.addHr(new()
                {
                    margin_t = 5f,
                    margin_b = 5f
                });
                BxR.alignx = ALIGN.CENTER;
                BxR.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 30,
                    text = TX.Get("multiplayer_menu_host_utilities")
                });
                BxR.Br();
                BxR.addButton(new()
                {
                    title = TX.Get("multiplayer_menu_host_kick"),
                    fnClick = B =>
                    {
                        OnPlayerSelect(B, i =>
                        {
                            WNMNTools.Kick(i);
                            GM.deactivate();
                        });
                        return true;
                    }
                });
                BxR.addP(new()
                {
                    text = "  "
                });
                BxR.addButton(new()
                {
                    title = TX.Get("multiplayer_menu_host_mute"),
                    fnClick = B =>
                    {
                        OnPlayerSelect(B, i =>
                        {
                            WNMNTools.Mute(i);
                            GM.deactivate();
                        });
                        return true;
                    }
                });
                BxR.Br();
                BxR.addButton(new()
                {
                    title = TX.Get("multiplayer_menu_host_teleportallself"),
                    fnClick = B =>
                    {
                        WNMNTools.SendNotifyNoelTransferToAllPeers(0);
                        GM.deactivate();
                        return true;
                    }
                });
                BxR.addP(new()
                {
                    text = "  "
                });
                BxR.addButton(new()
                {
                    title = TX.Get("multiplayer_menu_host_modifyroomconfig")
                });
                BxR.addFocusFn(_B =>
                {
                    UiMenuMul.BxP.deactivate();
                    return true;
                });
            }
            return true;
        }

        public override void initEdit()
        {
        }

        public override void quitEdit()
        {
        }

        void Mutted(aBtn B, UiBoxDesigner BxCmd)
        {
            BxCmd.activate();
            IN.setZ(BxCmd.transform, BxR.transform.position.z - 1f);
            BxCmd.Clear();
            BxCmd.getBox().frametype = UiBox.FRAMETYPE.ONELINE;
            BxCmd.WH(300f, 150f);
            BxCmd.margin_in_lr = 10f;
            BxCmd.margin_in_tb = 10f;
            BxCmd.init();
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                text = TX.Get("multiplayer_menu_mute"),
                TxCol = Color.HSVToRGB(0, 0.95f, 0.91f)
            });
            BxCmd.Br();
            BxCmd.addButton(new()
            {
                title = TX.Get("Submit"),
                fnClick = B =>
                {
                    BxCmd.deactivate();
                    BxR.Focus();
                    return true;
                }
            });
            Vector3 btnPos = B.transform.position;
            float targetX = btnPos.x * 64f + 300f;
            float targetY = btnPos.y * 64f;
            BxCmd.posSetDA(targetX, targetY, 0, 20f, true);
            BxCmd.Focusable(true, true);
            BxCmd.Focus();
        }

        void OnPlayerSelect(aBtn B, Action<int> OnSelectedIndex)
        {
            List<KeyValuePair<int, string>> btns = [.. WNMNTools.AllNicknames, new(0, TX.Get("Cancel"))];
            UiBoxDesigner BxCmd = UiMenuMul.BxP;
            BxCmd.activate();
            IN.setZ(BxCmd.transform, BxR.transform.position.z - 1f);
            BxCmd.Clear();
            BxCmd.getBox().frametype = UiBox.FRAMETYPE.ONELINE;
            BxCmd.WH(200f, 48f * btns.Count);
            BxCmd.margin_in_lr = 10f;
            BxCmd.margin_in_tb = 10f;
            BxCmd.init();
            BxCmd.addButtonMultiT<aBtnNel>(new DsnDataButtonMulti
            {
                name = "sub_menu",
                titles = [.. btns.Select(x => x.Value)],
                skin = "row_center",
                clms = 1,
                w = BxCmd.use_w,
                h = 30f,
                fnClick = (aBtn BSub) =>
                {
                    if (BSub.title != TX.Get("Cancel"))
                    {
                        OnSelectedIndex?.Invoke(btns.Find(x => x.Value == BSub.title).Key);
                    }
                    BxCmd.deactivate();
                    BSub.Select(true);
                    BxR.Focus();
                    return true;
                }
            });
            Vector3 btnPos = B.transform.position;
            float targetX = btnPos.x * 64f + 300f;
            float targetY = btnPos.y * 64f;
            BxCmd.posSetDA(targetX, targetY, 0, 20f, true);
            BxCmd.Focusable(true, true);
            BxCmd.Focus();
        }

        static Color ColorDefault => Color.HSVToRGB(0, 0, 0.219f);

        static readonly string[] msgs = ["multiplayer_msg_greeting", "multiplayer_msg_come_here", "multiplayer_msg_pve_start", "multiplayer_msg_pve_warn", "multiplayer_msg_pve_help", "multiplayer_msg_pvp_start", "multiplayer_msg_pvp_greeting", "multiplayer_msg_sitting", "multiplayer_msg_goodbye"];
    }
}
