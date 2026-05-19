using evt;
using LiteNetLib;
using LiteNetLib.Utils;
using m2d;
using nel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using WeNeedMoreNoels.CSNetworking;
using WeNeedMoreNoels.DataStruct;
using WeNeedMoreNoels.Networking;
using WeNeedMoreNoels.SN;
using XX;

namespace WeNeedMoreNoels
{
    public static class WNMNTools
    {
        static WNMNHost host;
        static WNMNClient client;

        public static WNMNPeer peer;

        public static NetWorkType Type;
        public static int LocalID = -1;
        public static bool Inited;
        public static bool PeerInited;

        public static string LocalIP;

        public static Dictionary<int, NetPeer> PeerDic = [];

        static int _unique_id;
        public static int Unique_ID
        {
            get
            {
                _unique_id++;
                return _unique_id;
            }
        }

        public static List<KeyValuePair<int, string>> AllNicknames
        {
            get
            {
                return [.. DB.noelIns.Select(x => new KeyValuePair<int, string>(x.Key, DB.InitConfig.InvisibleNickname ? TX.Get("multiplayer_noel_nickname") + x.Key : x.Value.NickNameStr))];
            }
        }

        public static void InitNetworking(NetworkConfig config)
        {
            InitNetworking(config.Type, out host, out client);
            GameObject gameObject = new("NetworkPeer");
            peer = gameObject.AddComponent<WNMNPeer>();
            int port = peer.StartPeer();
            switch (config.Type)
            {
                case NetWorkType.Host:
                    RunHost(config.port);
                    Type = NetWorkType.Host;
                    DB.peerConfigs.Add(0, new()
                    {
                        Nickname = config.nickName,
                        NoelType = config.NoelType,
                        NoelColor = config.NoelColor
                    });
                    break;
                case NetWorkType.Client:
                    ConnectHost(config.ip, config.port);
                    Type = NetWorkType.Client;
                    break;
            }
            DB.Nickname = config.nickName;
        }

        public static void InitNetworking(NetWorkType type, out WNMNHost host, out WNMNClient client)
        {
            DB.networkType = type;
            if (GameObject.Find("NetworkSource") != null)
            {
                Plugin.Logger.LogWarning("NetworkSource created");
                host = null;
                client = null;
                return;
            }
            GameObject gameObject = new("NetworkSource");
            switch (DB.networkType)
            {
                case NetWorkType.Host:
                    host = gameObject.AddComponent<WNMNHost>();
                    Type = NetWorkType.Host;
                    client = null;
                    break;
                case NetWorkType.Client:
                    client = gameObject.AddComponent<WNMNClient>();
                    Type = NetWorkType.Client;
                    host = null;
                    break;
                default:
                    host = null;
                    client = null;
                    break;
            }
            Inited = true;
        }

        public static void ConnectOtherPeer(List<KeyValuePair<int, ConnectPeerInfo>> peerList, NetPeer hostPeer, int hostPort)
        {
            if (peerList.Count == 0)
            {
                peer.ConnectPeer(hostPeer.EndPoint.Address.ToString(), hostPort);
                return;
            }
            int id = Type == NetWorkType.Host ? 0 : client.peerID;
            foreach (var pair in peerList)
            {
                if (pair.Key != id)
                {
                    peer.ConnectPeer(pair.Value.IP, pair.Value.Port);
                }
            }
        }

        public static void RunHost(int port = 47210)
        {
            if (host == null)
            {
                Plugin.Logger.LogWarning("NetworkSource not initialized");
                return;
            }
            host.StartHost(port);
        }

        public static void ConnectHost(string ip = "localhost", int port = 4721)
        {
            if (client == null)
            {
                Plugin.Logger.LogWarning("NetworkSource not initialized");
                return;
            }
            client.ConnectHost(ip, port);
        }

        public static void UpdateNoel(int id, UpdateNoelInfo info)
        {
            if (!DB.noelIns.ContainsKey(id))
            {
                return;
            }
            DB.noelIns[id].NoelInfo = info;
        }

        public static void UpdateAllNoels()
        {
            foreach (var pair in DB.noelIns)
            {
                ShadowNoelExtensions.UpdateShadowNoelInfo(pair.Key);
            }
        }

        public static void GenerateAllNoels(List<KeyValuePair<int, ClientConfig>> list)
        {
            foreach (var pair in list)
            {
                ShadowNoelExtensions.GenerateShadowNoel(pair.Value, pair.Key);
            }
        }

        public static void SendInitToAllPeers(int id)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.InitNoel,
                PeerId = id,
                InitNoelConfig = new()
                {
                    Id = id,
                    ClientConfig = DB.InitConfig,
                    PartyConfig = new()
                    {
                        ID = LocalID,
                        A = DB.partyInfos[LocalID].Color.a,
                        R = DB.partyInfos[LocalID].Color.r,
                        G = DB.partyInfos[LocalID].Color.g,
                        B = DB.partyInfos[LocalID].Color.b,
                        Name = DB.partyInfos[LocalID].Name
                    }
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendUpdateToAllPeers(int id)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.UpdateNoelInfo,
                PeerId = id,
                UpdateNoelInfo = ShadowNoelExtensions.GetSendInfo()
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.Unreliable);
        }

        public static void SendDamageToAllPeers(int id, NotifyNoelDamage dmg)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyNoelDamage,
                PeerId = id,
                NotifyNoelDamage = dmg
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendMagicToAllPeers(int id, NotifyNoelMagic mg)
        {
            if (DB.InitConfig is null)
            {
                return;
            }
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyNoelMagic,
                PeerId = id,
                NotifyNoelMagic = mg
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendBattleStartToAllPeers(int id)
        {
            if (DB.InitConfig is null)
            {
                return;
            }
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyNoelStartBattle,
                PeerId = id
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendBattleEndToAllPeers(int id)
        {
            if (DB.InitConfig is null)
            {
                return;
            }
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyNoelEndBattle,
                PeerId = id
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendUpdatePeerInfoToAllPeers(int id, string nickname)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.UpdatePeerInfo,
                PeerId = id,
                UpdatePeerInfo = new()
                {
                    Type = UpdatePeerType.Nickname,
                    NickName = nickname
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendUpdatePeerInfoToAllPeers(int id, int party)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.UpdatePeerInfo,
                PeerId = id,
                UpdatePeerInfo = new()
                {
                    Type = UpdatePeerType.Party,
                    PartyID = party
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendNotifyNoelTransferToAllPeers(int id)
        {
            DB.MainPR.getPosition(out float x, out float y);
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyNoelTransfer,
                PeerId = id,
                NotifyNoelTransfer = new()
                {
                    Key = DB.MainPR.Mp.key,
                    X = x,
                    Y = y
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendNotifyShortMsgToAllPeers(int id, string key)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyShortMsg,
                PeerId = id,
                NotifyShortMsg = new()
                {
                    ID = id,
                    key = key
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendGetItemToAllPeers(string key, int count, int grade)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyGetItem,
                PeerId = LocalID,
                NotifyItemChanged = new()
                {
                    PartyID = DB.LocalNoelParty,
                    key = key,
                    count = count,
                    grade = grade
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendLoseItemToAllPeers(string key, int count, int grade)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyLoseItem,
                PeerId = LocalID,
                NotifyItemChanged = new()
                {
                    PartyID = DB.LocalNoelParty,
                    key = key,
                    count = count,
                    grade = grade
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendGetCoinToAllPeers(CoinStorage.CTYPE type, int count)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyGetCoin,
                PeerId = LocalID,
                NotifyCoinChanged = new()
                {
                    PartyID = DB.LocalNoelParty,
                    coinType = type,
                    count = count,
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendLoseCoinToAllPeers(CoinStorage.CTYPE type, int count)
        {
            WNMNPeerMessage messageSend = new()
            {
                Type = WNMNPeerMessageType.NotifyLoseCoin,
                PeerId = LocalID,
                NotifyCoinChanged = new()
                {
                    PartyID = DB.LocalNoelParty,
                    coinType = type,
                    count = count,
                }
            };
            using MemoryStream stream = new();
            Serializer.Serialize(stream, messageSend);
            byte[] buffer = stream.ToArray();
            peer.SendToAll(buffer, DeliveryMethod.ReliableOrdered);
        }

        public static void CleanUpClient(int id)
        {
            ShadowNoelExtensions.DisableShadowNoel(id);
            DB.noelIns.Remove(id);
            DB.partyInfos.Remove(id);
            DB.peerInfos.Remove(id);
            DB.peerConfigs.Remove(id);
        }

        public static void SetAllNickNameBgs()
        {
            ShadowNoelNickname nicknameIns = DB.MainPRNickname;
            nicknameIns?.SetBgColor(DB.partyInfos[DB.LocalNoelParty].Color);
            foreach (var pair in DB.noelIns)
            {
                pair.Value.NicknameIns?.SetBgColor(DB.partyInfos[pair.Value.Noel.PartyID].Color);
            }
        }

        public static void UpdatePeer(int id, UpdatePeerInfo info)
        {
            switch (info.Type)
            {
                case UpdatePeerType.Nickname:
                    DB.peerConfigs[id].Nickname = info.NickName;
                    break;
                case UpdatePeerType.Party:
                    DB.noelIns[id].Noel.PartyID = info.PartyID;
                    break;
            }
        }

        public static void TransferMainNoel(NotifyNoelTransfer transfer)
        {
            TransferMainNoel(transfer.Key, transfer.X, transfer.Y);
        }

        public static void TransferMainNoel(string key, float x, float y)
        {
            Map2d map = DB.MainPR.NM2D.Get(key);
            M2LpMapTransferBase.executeTransferFastTravel(map, (int)x, (int)y);
        }

        public static void Kick(int id)
        {
            host.SendKick(id);
            NetDataWriter writer = new();
            writer.Put(true);
            PeerDic[id].Disconnect(writer);
            PeerDic.Remove(id);
            CleanUpClient(id);
        }

        public static void Mute(int id)
        {
            host.SendMute(id);
        }

        public static void ToggleMute()
        {
            DB.Mute = !DB.Mute;
        }

        public static void BroadcastMsg(string id)
        {
            SendMsg(LocalID, id);
            SendNotifyShortMsgToAllPeers(LocalID, id);
        }

        public static void SendMsg(int id, string txtID)
        {
            if (id == LocalID)
            {
                DB.MainPRMsg.ShowMsg(txtID);
            }
            else
            {
                DB.noelIns[id].Noel.MsgIns?.ShowMsg(txtID);
            }
        }

        public static void GetItem(int partyID, string key, int count, int grade)
        {
            if (partyID != DB.LocalNoelParty)
            {
                return;
            }
            NelItem item = NelItem.GetById(key);
            if (item is null)
            {
                DB.MainPR.NM2D.IMNG.getItem(item, count, grade);
            }
        }

        public static void LoseItem(int partyID, string key, int count, int grade)
        {
            if (partyID != DB.LocalNoelParty)
            {
                return;
            }
            NelItem item = NelItem.GetById(key);
            if (item is null)
            {
                DB.MainPR.NM2D.IMNG.reduceItem(item, count, grade);
            }
        }

        public static void GetCoin(int partyID, CoinStorage.CTYPE type, int count)
        {
            if (partyID != DB.LocalNoelParty)
            {
                return;
            }
            CoinStorage.addCount(count, type);
        }

        public static void LoseCoin(int partyID, CoinStorage.CTYPE type, int count)
        {
            if (partyID != DB.LocalNoelParty)
            {
                return;
            }
            CoinStorage.reduceCount(count, type);
        }

        public class NetworkConfig
        {
            public NetWorkType Type;

            public int port;

            public string ip;

            public NoelType NoelType;

            public ColorNoelColor NoelColor;

            public bool InvisibleNickname;

            public string nickName;

            public static implicit operator ClientConfig(NetworkConfig config)
            {
                return new()
                {
                    Nickname = config.nickName,
                    NoelType = config.NoelType,
                    NoelColor = config.NoelColor
                };
            }
        }
    }

    public enum NetWorkType
    {
        Host,
        Client
    }
}
