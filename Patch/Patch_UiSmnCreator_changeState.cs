using HarmonyLib;
using nel;
using nel.mgm.smncr;
using System.Collections;
using System.Linq;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(UiSmnCreator), nameof(UiSmnCreator.changeState))]
    public class Patch_UiSmnCreator_changeState
    {
        static Patch_UiSmnCreator_changeState()
        {
            WNMNTools.UpdateSimUI += DrawUI;
        }

        [HarmonyPrefix]
        static bool Prefix(UiSmnCreator __instance, UiSmnCreator.STATE stt)
        {
            if ((int)stt == 9)
            {
                if (WNMNTools.SimBattleSyncHost != WNMNTools.LocalID)
                {
                    SimBattle battle = new()
                    {
                        Type = NotifySimBattleType.ConnectHost
                    };
                    WNMNTools.SendNotifySimBattleToAllPeers(battle);
                    WNMNTools.SimBattleReadyList.Add(WNMNTools.SimBattleSyncHost);
                }
                __instance.state = stt;
                DrawUI();
                UiMenuMul.BxSB.activate();
                UiMenuMul.BxSB.Focus();
                __instance.BxFile.deactivate();
                return false;
            }
            return true;
        }

        static void DrawUI()
        {
            UiBoxDesigner BxCmd = UiMenuMul.BxSB;
            BxCmd.selectable_loop = 3;
            BxCmd.Clear();
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = TX.Get("multiplayer_simbattle_title")
            });
            BxCmd.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            if (WNMNTools.SimBattleSyncHost == -1)
            {
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 30,
                    text = TX.Get("multiplayer_simbattle_noroom")
                });
                return;
            }
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = WNMNTools.SimBattleSyncHost == WNMNTools.LocalID ? TX.GetA("multiplayer_simbattle_hostsubtitle", (WNMNTools.SimBattleSyncList.Count + 1).ToString(), (DB.noelIns.Count + 1).ToString()) : TX.GetA("multiplayer_simbattle_clientsubtitle", (WNMNTools.SimBattleSyncList.Count + 1).ToString(), (DB.noelIns.Count + 1).ToString())
            });
            var block1 = BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = "."
            });
            var block2 = BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = "."
            });
            var block3 = BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = "."
            });
            BxCmd.Br();
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 20,
                text = TX.GetA("multiplayer_simbattle_name", (WNMNTools.GetNickname(WNMNTools.SimBattleSyncHost)).ToString())
            });
            if (WNMNTools.SimBattleSyncHost != WNMNTools.LocalID)
            {
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 20,
                    text = TX.Get("multiplayer_simbattle_simstate")
                });
                var fill = BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 20,
                    text = WNMNTools.SimBattleSynced ? TX.Get("multiplayer_simbattle_synced") : TX.Get("multiplayer_simbattle_unsynced")
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 15,
                    text = TX.Get("multiplayer_simbattle_synchint")
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                var b = BxCmd.addButton(new()
                {
                    title = TX.Get("multiplayer_simbattle_syncmap"),
                    fnClick = B =>
                    {
                        WNMNTools.SendSimBattleSync(WNMNTools.SimBattleSyncHost);
                        return true;
                    }
                });
                b.Select(true);
            }
            BxCmd.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            BxCmd.alignx = ALIGN.CENTER;
            if (WNMNTools.SimBattleSyncHost == WNMNTools.LocalID)
            {
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 20,
                    text = "默认出生点"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 10,
                    text = $"( {WNMNTools.SpawnDic[-1].x}, {WNMNTools.SpawnDic[-1].y} )"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addButton(new()
                {
                    title = "设置默认出生点",
                    fnClick = B =>
                    {
                        WNMNTools.IsSettingSpawnLocation = true;
                        WNMNTools.CurrentSetID = -1;
                        UiMenuMul.BxSL.deactivate();
                        UiMenuMul.BxSB.deactivate();
                        WNMNTools.USC.changeState(UiSmnCreator.STATE.SCREATE);
                        WNMNTools.SSE.changeState(SmncStageEditor.STATE.LIST);
                        WNMNTools.SSE.fnChangedListStgo(WNMNTools.SSE.BConL, WNMNTools.SSE.BConL.selected, 0);
                        if (WNMNTools.SpawnDic.ContainsKey(WNMNTools.CurrentSetID))
                        {
                            SmncStageEditorManager.StgObject @object = WNMNTools.SSE.CurFile.Astgo[0];
                            @object.x = (int)WNMNTools.SpawnDic[WNMNTools.CurrentSetID].x;
                            @object.y = (int)WNMNTools.SpawnDic[WNMNTools.CurrentSetID].y;
                            WNMNTools.CurSimFile.Astgo[0] = @object;
                            WNMNTools.SSE.drawCheck();
                        }
                        return true;
                    }
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 20,
                    text = "个人出生点"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 15,
                    text = WNMNTools.SpawnDic.ContainsKey(WNMNTools.LocalID) && WNMNTools.SimBattleSyncList.All(WNMNTools.SpawnDic.ContainsKey) ? "所有玩家都已配置个人出生点✓" : "发现未配置的个人出生点！\n点击下方右侧按钮进行配置，所有未配置的玩家将为默认出生点"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                var b1 = BxCmd.addButton(new()
                {
                    name = "checkspawn",
                    title = "查看当前出生点",
                    fnClick = B =>
                    {
                        UiBoxDesigner BxCmd = UiMenuMul.BxSS;
                        BxCmd.Clear();
                        IN.setZ(BxCmd.transform, UiMenuMul.BxSB.transform.position.z - 1f);
                        BxCmd.init();
                        BxCmd.alignx = ALIGN.CENTER;
                        BxCmd.addP(new()
                        {
                            TxCol = ColorDefault,
                            size = 20,
                            text = "玩家#" + WNMNTools.GetNickname(WNMNTools.LocalID)
                        });
                        BxCmd.addP(new()
                        {
                            TxCol = ColorDefault,
                            size = 20,
                            text = " - "
                        });
                        if (WNMNTools.SpawnDic.ContainsKey(WNMNTools.LocalID))
                        {
                            BxCmd.addP(new()
                            {
                                TxCol = ColorDefault,
                                size = 20,
                                text = $"( {WNMNTools.SpawnDic[WNMNTools.LocalID].x}, {WNMNTools.SpawnDic[WNMNTools.LocalID].y} )"
                            });
                        }
                        else
                        {
                            BxCmd.addP(new()
                            {
                                TxCol = ColorDefault,
                                size = 20,
                                text = "未配置，缺省为默认值"
                            });
                        }
                        BxCmd.Br();
                        foreach (int id in WNMNTools.SimBattleSyncList)
                        {
                            BxCmd.alignx = ALIGN.CENTER;
                            BxCmd.addP(new()
                            {
                                TxCol = ColorDefault,
                                size = 20,
                                text = "玩家#" + WNMNTools.GetNickname(id)
                            });
                            BxCmd.addP(new()
                            {
                                TxCol = ColorDefault,
                                size = 20,
                                text = " - "
                            });
                            if (WNMNTools.SpawnDic.ContainsKey(id))
                            {
                                BxCmd.addP(new()
                                {
                                    TxCol = ColorDefault,
                                    size = 20,
                                    text = $"( {WNMNTools.SpawnDic[id].x}, {WNMNTools.SpawnDic[id].y} )"
                                });
                            }
                            else
                            {
                                BxCmd.addP(new()
                                {
                                    TxCol = ColorDefault,
                                    size = 20,
                                    text = "未配置，缺省为默认值"
                                });
                            }
                            BxCmd.Br();
                        }
                        var b = BxCmd.addButton(new()
                        {
                            title = TX.Get("Submit"),
                            fnClick = B =>
                            {
                                BxCmd.deactivate();
                                UiMenuMul.BxSB.Focus();
                                UiMenuMul.BxSB.getBtn("checkspawn").Select(true);
                                return true;
                            }
                        });
                        b.Select(false);
                        BxCmd.activate();
                        BxCmd.Focusable(true, true);
                        BxCmd.Focus();
                        return true;
                    }
                });
                var b2 = BxCmd.addButton(new()
                {
                    name = "configspawn",
                    title = "选择一名角色...",
                    fnClick = B =>
                    {
                        string[] btns = [WNMNTools.GetNickname(WNMNTools.LocalID), ..WNMNTools.SimBattleSyncList.Select(x => DB.noelIns[x].NickNameStr), TX.Get("Cancel")];
                        UiBoxDesigner BxCmd = UiMenuMul.BxSL;
                        BxCmd.activate();
                        IN.setZ(BxCmd.transform, UiMenuMul.BxSB.transform.position.z - 1f);
                        BxCmd.Clear();
                        BxCmd.getBox().frametype = UiBox.FRAMETYPE.ONELINE;
                        BxCmd.WH(200f, 48f * btns.Length);
                        BxCmd.margin_in_lr = 10f;
                        BxCmd.margin_in_tb = 10f;
                        BxCmd.init();
                        var con = BxCmd.addButtonMultiT<aBtnNel>(new DsnDataButtonMulti
                        {
                            name = "sub_menu",
                            titles = btns,
                            skin = "row_center",
                            clms = 1,
                            w = BxCmd.use_w,
                            h = 30f,
                            fnClick = (aBtn BSub) =>
                            {
                                if (BSub.title != TX.Get("Cancel"))
                                {
                                    WNMNTools.IsSettingSpawnLocation = true;
                                    WNMNTools.CurrentSetID = btns.ToList().IndexOf(BSub.title);
                                    UiMenuMul.BxSL.deactivate();
                                    UiMenuMul.BxSB.deactivate();
                                    WNMNTools.USC.changeState(UiSmnCreator.STATE.SCREATE);
                                    WNMNTools.SSE.changeState(SmncStageEditor.STATE.LIST);
                                    WNMNTools.SSE.fnChangedListStgo(WNMNTools.SSE.BConL, WNMNTools.SSE.BConL.selected, 0);
                                    if (WNMNTools.SpawnDic.ContainsKey(WNMNTools.CurrentSetID))
                                    {
                                        SmncStageEditorManager.StgObject @object = WNMNTools.SSE.StgoMaking;
                                        @object.x = (int)WNMNTools.SpawnDic[WNMNTools.CurrentSetID].x;
                                        @object.y = (int)WNMNTools.SpawnDic[WNMNTools.CurrentSetID].y;
                                        WNMNTools.SSE.StgoMaking = @object;
                                        WNMNTools.SSE.drawCheck();
                                    }
                                    return true;
                                }
                                BxCmd.deactivate();
                                UiMenuMul.BxSB.Focus();
                                UiMenuMul.BxSB.getBtn("configspawn").Select(true);
                                return true;
                            }
                        });
                        con.Get(0).Select(true);
                        Vector3 btnPos = B.transform.position;
                        float targetX = btnPos.x * 64f + 300f;
                        float targetY = btnPos.y * 64f;
                        BxCmd.posSetDA(targetX, targetY, 0, 20f, true);
                        BxCmd.Focusable(true, true);
                        BxCmd.Focus();
                        return true;
                    }
                });
                b1.setNaviR(b2, true, true);
                BxCmd.addFocusFn(B =>
                {
                    UiMenuMul.BxSS.deactivate();
                    UiMenuMul.BxSL.deactivate();
                    return true;
                });
                BxCmd.addHr(new()
                {
                    margin_t = 5f,
                    margin_b = 5f
                });
                BxCmd.alignx = ALIGN.CENTER;
            }
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 20,
                text = TX.GetA("multiplayer_simbattle_readytitle", (WNMNTools.SimBattleReadyList.Count + (WNMNTools.SimBattleSyncHost == WNMNTools.LocalID ? 1 : (WNMNTools.SimBattleReady ? 1 : 0))).ToString(), (WNMNTools.SimBattleSyncList.Count + 1).ToString())
            });
            BxCmd.Br();
            BxCmd.alignx = ALIGN.CENTER;
            if (WNMNTools.SimBattleSyncHost == WNMNTools.LocalID)
            {
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 15,
                    text = TX.Get("multiplayer_simbattle_readyhint")
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                aBtn b = BxCmd.addButton(new()
                {
                    title = "&&Smnc_start_battle_submit",
                    fnClick = B =>
                    {
                        WNMNTools.OpenSmncBattle();
                        return true;
                    }
                });
                if (WNMNTools.SimBattleReadyList.Count != WNMNTools.SimBattleSyncList.Count)
                {
                    b.SetLocked(true);
                }
                b.Select(true);
            }
            else
            {
                if (WNMNTools.SimBattleSynced)
                {
                    BxCmd.addP(new()
                    {
                        TxCol = ColorDefault,
                        size = 15,
                        text = TX.Get("multiplayer_simbattle_readyhint1")
                    });
                    BxCmd.Br();
                    BxCmd.alignx = ALIGN.CENTER;
                    var b = BxCmd.addButton(new()
                    {
                        title = WNMNTools.SimBattleReady ? TX.Get("multiplayer_simbattle_ready") : TX.Get("multiplayer_simbattle_unready"),
                        fnClick = B =>
                        {
                            WNMNTools.SimBattleReady = !WNMNTools.SimBattleReady;
                            if (WNMNTools.SimBattleReady)
                            {
                                SimBattle battle = new()
                                {
                                    Type = NotifySimBattleType.ReadyHost
                                };
                                WNMNTools.SendNotifySimBattleToAllPeers(battle);
                            }
                            else
                            {
                                SimBattle battle = new()
                                {
                                    Type = NotifySimBattleType.UnreadyHost
                                };
                                WNMNTools.SendNotifySimBattleToAllPeers(battle);
                            }
                            DrawUI();
                            return true;
                        }
                    });
                }
                else
                {
                    BxCmd.addP(new()
                    {
                        TxCol = ColorDefault,
                        size = 20,
                        text = TX.Get("multiplayer_simbattle_syncfirst")
                    });
                }
            }
            Plugin.PluginInstance.StopAllCoroutines();
            Plugin.PluginInstance.StartCoroutine(RunEffect(block1, block2, block3));
        }

        static float fadeDuration = 2.0f;    // 单个点从亮到灭的总持续时间
        static float interval = 0.25f;        // 两个点启动之间的间隔时间（越小重叠越多）
        static float cycleWait = 0.5f;       // 每一轮播放完后的等待时间

        static IEnumerator RunEffect(FillBlock block1, FillBlock block2, FillBlock block3)
        {
            FillBlock[] blocks = { block1, block2, block3 };

            while (true)
            {
                if (blocks == null)
                {
                    break;
                }
                float timer = 0;
                float totalTime = (blocks.Length - 1) * interval + fadeDuration;
                while (timer < totalTime)
                {
                    timer += Time.deltaTime;
                    for (int i = 0; i < blocks.Length; i++)
                    {
                        float localTime = timer - (i * interval);
                        float alpha = 0;
                        if (localTime >= 0 && localTime <= fadeDuration)
                        {
                            float normalizedTime = localTime / fadeDuration;
                            alpha = X.Sin(normalizedTime * X.PI);
                        }
                        blocks[i].alpha = alpha;
                    }
                    yield return null;
                }
                foreach (var block in blocks) block.alpha = 0;
                yield return new WaitForSeconds(cycleWait);
            }
        }

        static Color ColorDefault => Color.HSVToRGB(0, 0, 0.219f);
    }
}
