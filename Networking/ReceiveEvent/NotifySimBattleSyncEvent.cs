using nel.mgm.smncr;
using Newtonsoft.Json;
using PixelLiner.PixelLinerLib;
using WeNeedMoreNoels.DataStruct;

namespace WeNeedMoreNoels.Networking.ReceiveEvent
{
    public class NotifySimBattleSyncEvent : PeerReceiveMessageBase
    {
        public override bool CheckMessage(WNMNPeerMessage message)
        {
            return message.Type == WNMNPeerMessageType.NotifySimBattleSync && message.PeerId == WNMNTools.LocalID;
        }

        public override void ReceiveMessage(WNMNPeerMessage message)
        {
            if (message.SyncSimBattle.SyncID == -1)
            {
                ByteArray array = new(message.SyncSimBattle.SyncSimBattleData);
                SmncFile.readFromFile(array, WNMNTools.USC.M2D, (file, _) =>
                {
                    WNMNTools.CurSimFile = file;
                    WNMNTools.USC.AFiles.Add(file);
                    WNMNTools.USC.initFileSelection(file, true);
                    WNMNTools.SimBattleSynced = true;
                    WNMNTools.UpdateSimUI?.Invoke();
                });
            }
            else
            {
                WNMNTools.SendBackSimBattleSyncData(message.SyncSimBattle.SyncID);
            }
        }

        public override string ToMessageString(WNMNPeerMessage message)
        {
            return $"SimBattleSync:{JsonConvert.SerializeObject(message)}";
        }
    }
}
