using HarmonyLib;
using nel;
using nel.mgm.smncr;
using System.Collections;
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
                return false;
            }
            return true;
        }

        static void DrawUI()
        {
            UiBoxDesigner BxCmd = UiMenuMul.BxSB;
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
                    text = "暂无模拟战房间可加入"
                });
                BxCmd.deactivate();
                return;
            }
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = WNMNTools.SimBattleSyncHost == WNMNTools.LocalID ? $"主持房间中（{WNMNTools.SimBattleSyncList.Count + 1}/{DB.noelIns.Count + 1} 已连接） 等待中" : $"处于房间中（{WNMNTools.SimBattleSyncList.Count + 1}/{DB.noelIns.Count + 1} 已连接） 等待中"
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
                text = $"{WNMNTools.GetNickname(WNMNTools.SimBattleSyncHost)}的模拟战"
            });
            if (WNMNTools.SimBattleSyncHost != WNMNTools.LocalID)
            {
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 20,
                    text = "当前模拟战地图状态："
                });
                var fill = BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 20,
                    text = WNMNTools.SimBattleSynced ? "已同步" : "未同步"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 15,
                    text = "可点击下方按钮同步模拟战地图"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                BxCmd.addButton(new()
                {
                    title = "同步模拟战地图",
                    fnClick = B =>
                    {
                        WNMNTools.SendSimBattleSync(WNMNTools.SimBattleSyncHost);
                        return true;
                    }
                });
            }
            BxCmd.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            BxCmd.alignx = ALIGN.CENTER;
            BxCmd.addP(new()
            {
                TxCol = ColorDefault,
                size = 15,
                text = $"当前已准备人数（{WNMNTools.SimBattleReadyList.Count + (WNMNTools.SimBattleSyncHost == WNMNTools.LocalID ? 1 : (WNMNTools.SimBattleReady ? 1 : 0))}/{WNMNTools.SimBattleSyncList.Count + 1}）"
            });
            BxCmd.Br();
            BxCmd.alignx = ALIGN.CENTER;
            if (WNMNTools.SimBattleSyncHost == WNMNTools.LocalID)
            {
                BxCmd.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 10,
                    text = "当所有玩家均准备完毕后即可开始多人模拟战"
                });
                BxCmd.Br();
                BxCmd.alignx = ALIGN.CENTER;
                aBtn b = BxCmd.addButton(new()
                {
                    title = "开始战斗！",
                    fnClick = B =>
                    {
                        WNMNTools.OpenSmncBattle(true);
                        return true;
                    }
                });
                if (WNMNTools.SimBattleReadyList.Count != WNMNTools.SimBattleSyncList.Count)
                {
                    b.SetLocked(true);
                }
            }
            else
            {
                if (WNMNTools.SimBattleSynced)
                {
                    BxCmd.addP(new()
                    {
                        TxCol = ColorDefault,
                        size = 15,
                        text = "请点击下方按钮进行准备"
                    });
                    BxCmd.Br();
                    BxCmd.alignx = ALIGN.CENTER;
                    BxCmd.addButton(new()
                    {
                        title = WNMNTools.SimBattleReady ? "已准备" : "准备",
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
                        text = "请先同步模拟战地图"
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
