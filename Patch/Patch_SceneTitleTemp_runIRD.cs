using HarmonyLib;
using LiteNetLib;
using nel;
using nel.title;
using System.IO;
using UnityEngine;
using WeNeedMoreNoels.DataStruct;
using XX;

namespace WeNeedMoreNoels.Patch
{
    [HarmonyPatch(typeof(SceneTitleTemp), nameof(SceneTitleTemp.runIRD))]
    public class Patch_SceneTitleTemp_runIRD
    {
        static UiBoxDesigner BxHC;

        static UiBoxDesigner BxCC;

        static UiBoxDesigner BxHCI;

        static Designer Black;

        static NoelType type;

        static LabeledInputField IpInput;

        static BtnContainerNumCounter<aBtnNumCounter> PortCon;

        static LabeledInputField NickNameInput;

        static FillBlock StateBlock;

        static NetManager client;

        static SceneTitleTemp stt;

        [HarmonyPrefix]
        static bool Prefix(object __instance, ref bool __result)
        {
            client?.PollEvents();
            stt = (SceneTitleTemp)__instance;
            if (DB.WNMNHostClosed)
            {
                if (stt.BxCon is null)
                {
                    __result = true;
                    return true;
                }
                DB.WNMNHostClosed = false;
                BxHCI = stt.BxCon.Create("hostClosedInfo", 0f, 0f, 380f, IN.h - 620f, 0, 0f, UiBoxDesignerFamily.MASKTYPE.BOX);
                BxHCI.Focusable(false, false, null);
                BxHCI.Clear();
                BxHCI.addP(new()
                {
                    TxCol = ColorDefault,
                    size = 40,
                    alignx = ALIGN.CENTER,
                    aligny = ALIGNY.MIDDLE,
                    text = TX.Get("multiplayer_host_closed")
                });
                BxHCI.activate();
                BxHCI.positionD(0f, 40f, 3, 50f);
                BxHCI.margin_in_tb = 30f;
                BxHCI.margin_in_lr = 60f;
                BxHCI.use_scroll = false;
                BxHCI.init();
                stt.remakeSumitCancelButton(true, false);
                stt.SubmitBtn.addClickFn(b =>
                {
                    BxHCI.deactivate();
                    BxHCI = null;
                    return true;
                });
                stt.DsBlack.activate();
                stt.DsBlack.init();
                __result = true;
                return false;
            }
            if (stt.state == SceneTitleTemp.STATE.SVD_SELECT && DB.WNMNHostSelectSVD)
            {
                if (stt.EditSvd is not null && stt.EditSvd.ui_state == UiSVD.STATE.LOAD_SUCCESS)
                {
                    bool ignore_svd_cfg = stt.EditSvd.ignore_svd_cfg;
                    SVD.sFile file = SVD.GetFile(UiSVD.last_focused, true);
                    string[] array = Directory.GetFiles(SVD.getDir(), "*.aicsave", SearchOption.TopDirectoryOnly);
                    byte[] buffer = File.ReadAllBytes(array[UiSVD.last_focused]);
                    DB.SyncSaveContentBuffer = buffer;
                    stt.EditSvd.deactivateDesigner();
                    stt.BxR.deactivate();
                    stt.BxDesc.deactivate();
                    BxHC = stt.BxCon.Create("hostConfirm", 0f, 0f, 540f, IN.h - 360f, 0, 0f, UiBoxDesignerFamily.MASKTYPE.BOX);
                    BxHC.Clear();
                    CreateUIHost(BxHC);
                    BxHC.activate();
                    BxHC.Focusable(true, false, null);
                    BxHC.use_scroll = false;
                    BxHC.init();
                    stt.remakeSumitCancelButton(true, true);
                    stt.SubmitBtn.addClickFn(b =>
                    {
                        WNMNTools.NetworkConfig config = new()
                        {
                            Type = NetWorkType.Host,
                            ip = "",
                            port = PortCon.cnt_val,
                            nickName = NickNameInput.text,
                            NoelType = type
                        };
                        DB.InitConfig = config;
                        BxHC.deactivate();
                        BxHC = null;
                        COOK.clear(false);
                        COOK.save_failure_announce = "";
                        COOK.setLoadTarget(file, ignore_svd_cfg);
                        stt.changeState(SceneTitleTemp.STATE.START_GAME);
                        return true;
                    });
                    stt.CancelBtn.addClickFn(b =>
                    {
                        BxHC.deactivate();
                        BxHC = null;
                        return true;
                    });
                    stt.DsBlack.alpha = 1;
                }
            }
            if (BxHCI is not null || BxCC is not null || DB.WNMNClientTransferNotComplete)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        static void Postfix(object __instance)
        {
            SceneTitleTemp stt = (SceneTitleTemp)__instance;
            if (DB.WNMNEnterNetworkTypeSelected)
            {
                DB.WNMNEnterNetworkTypeSelected = false;
                stt.BxDiff.deactivate();
                if (DB.WNMNEnterNetworkType == NetWorkType.Host)
                {
                    stt.changeState(SceneTitleTemp.STATE.SVD_SELECT);
                    DB.WNMNHostSelectSVD = true;
                }
                else
                {
                    BxCC = stt.BxCon.Create("clientConfirm", 0f, 0f, 540f, IN.h - 360f, 0, 0f, UiBoxDesignerFamily.MASKTYPE.BOX);
                    BxCC.Clear();
                    CreateUIClient(BxCC);
                    BxCC.activate();
                    BxCC.Focusable(true, false, null);
                    BxCC.use_scroll = false;
                    BxCC.init();
                    stt.remakeSumitCancelButton(true, true);
                    stt.SubmitBtn.addClickFn(b =>
                    {
                        EventBasedNetListener listener = new();
                        client = new(listener);
                        listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
                        {
                            WNMNTools.NetworkConfig config = new()
                            {
                                Type = NetWorkType.Client,
                                ip = IpInput.text,
                                port = PortCon.cnt_val,
                                nickName = NickNameInput.text,
                                NoelType = type
                            };
                            DB.InitConfig = config;
                            Plugin.Logger.LogInfo("Client received sync save data.");
                            byte[] receivedData = new byte[reader.UserDataSize];
                            reader.GetBytes(receivedData, reader.UserDataSize);
                            File.WriteAllBytes(SVD.getDir() + "\\" + DB.SYNC_FILE_NAME, receivedData);
                            SVD.sFile file = new(-2, true);//-2为同步存档
                            BxCC.deactivate();
                            BxCC = null;
                            DB.WNMNClientTransferNotComplete = false;
                            client.DisconnectAll();
                            client.Stop();
                            COOK.clear(false);
                            COOK.save_failure_announce = "";
                            COOK.setLoadTarget(file, true);
                            stt?.changeState(SceneTitleTemp.STATE.START_GAME);
                            reader.Recycle();
                        };
                        client.Start();
                        client.Connect(IpInput.text, PortCon.cnt_val + 1, DB.TRANSFER_ACCESS_KEY);
                        Plugin.Logger.LogInfo($"Starting connect {IpInput.text}:{PortCon.cnt_val}");
                        return true;
                    });
                    stt.CancelBtn.addClickFn(b =>
                    {
                        BxCC.deactivate();
                        BxCC = null;
                        return true;
                    });
                    stt.DsBlack.alpha = 1;
                }
            }
        }

        static void CreateUIHost(UiBoxDesigner designer)
        {
            designer.alignx = ALIGN.CENTER;
            designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 30f,
                text = TX.Get("multiplayer_host_title")
            });
            designer.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 30f,
                text = TX.Get("multiplayer_host_port")
            });
            PortCon = designer.addNumCounterT<aBtnNumCounter>(new()
            {
                h = 30f,
                digit = 5,
                maxval = 99999
            });
            PortCon.setValue(47210);
            designer.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            designer.alignx = ALIGN.CENTER;
            NickNameInput = designer.addInput(new()
            {
                h = 30f,
                label = TX.Get("multiplayer_nickname")
            });
            designer.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            designer.alignx = ALIGN.CENTER; 
            designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 20,
                text = TX.Get("multiplayer_select_noel")
            });
            FillBlock b = designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 20,
                text = TX.Get("multiplayer_noel")
            });
            designer.addSlider(new()
            {
                mn = 0,
                mx = 1,
                fnChanged = (_b, p_v, c_v) =>
                {
                    switch (c_v)
                    {
                        case 0:
                            b.text_content = TX.Get("multiplayer_noel");
                            break;
                        case 1:
                            b.text_content = TX.Get("multiplayer_noel_inverse");
                            break;
                    }
                    type = (NoelType)c_v;
                    return true;
                }
            });
        }

        static void CreateUIClient(UiBoxDesigner designer)
        {
            designer.alignx = ALIGN.CENTER;
            designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 30,
                text = TX.Get("multiplayer_client_title")
            });
            designer.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            designer.alignx = ALIGN.CENTER;
            IpInput = designer.addInput(new()
            {
                h = 30f,
                label = "IP:"
            });
            IpInput.text = "localhost";
            designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 20f,
                text = TX.Get("multiplayer_con")
            });
            PortCon = designer.addNumCounterT<aBtnNumCounter>(new()
            {
                h = 30f,
                digit = 4,
                maxval = 99999
            });
            PortCon.setValue(47210);
            designer.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            designer.alignx = ALIGN.CENTER;
            NickNameInput = designer.addInput(new()
            {
                h = 30f,
                label = TX.Get("multiplayer_nickname")
            });
            designer.addHr(new()
            {
                margin_t = 5f,
                margin_b = 5f
            });
            designer.alignx = ALIGN.CENTER;
            designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 20,
                text = TX.Get("multiplayer_select_noel")
            });
            var b = designer.addP(new()
            {
                TxCol = ColorDefault,
                size = 20,
                text = TX.Get("multiplayer_noel")
            });
            designer.addSlider(new()
            {
                mn = 0,
                mx = 1,
                fnChanged = (_b, p_v, c_v) =>
                {
                    switch (c_v)
                    {
                        case 0:
                            b.text_content = TX.Get("multiplayer_noel");
                            break;
                        case 1:
                            b.text_content = TX.Get("multiplayer_noel_inverse");
                            break;
                    }
                    type = (NoelType)c_v;
                    return true;
                }
            });
        }

        static Color ColorDefault => Color.HSVToRGB(0, 0, 0.219f);
    }

    public enum MultiPlayerSTATE
    {
        IDLE,
        Type,
        Host_SVD,
        Host_Confirm,
        Client_Confirm
    }
}
