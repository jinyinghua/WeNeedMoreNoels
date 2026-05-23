using ProtoBuf;

namespace WeNeedMoreNoels.DataStruct
{
    [ProtoContract]
    public class WNMNPeerMessage
    {
        [ProtoMember(1)]
        public int PeerId;
        [ProtoMember(2)]
        public WNMNPeerMessageType Type;
        [ProtoMember(3)]
        public IniConfig InitNoelConfig;
        [ProtoMember(4)]
        public UpdateNoelInfo UpdateNoelInfo;
        [ProtoMember(5)]
        public NotifyNoelDamage NotifyNoelDamage;
        [ProtoMember(6)]
        public NotifyNoelMagic NotifyNoelMagic;
        [ProtoMember(7)]
        public UpdatePeerInfo UpdatePeerInfo;
        [ProtoMember(8)]
        public NotifyNoelTransfer NotifyNoelTransfer;
        [ProtoMember(9)]
        public NotifyShortMsg NotifyShortMsg;
        [ProtoMember(10)]
        public NotifyItemChanged NotifyItemChanged;
        [ProtoMember(11)]
        public NotifyCoinChanged NotifyCoinChanged;
        [ProtoMember(12)]
        public NotifyRoomUpdate NotifyRoomUpdate;
        [ProtoMember(13)]
        public NotifyEnemyUpdate NotifyEnemyUpdate;
    }

    [ProtoContract]
    public enum WNMNPeerMessageType
    {
        [ProtoEnum]
        InitNoel,
        [ProtoEnum]
        UpdateNoelInfo,
        [ProtoEnum]
        NotifyNoelDamage,
        [ProtoEnum]
        NotifyNoelMagic,
        [ProtoEnum]
        NotifyNoelStartBattle,
        [ProtoEnum]
        NotifyNoelEndBattle,
        [ProtoEnum]
        UpdatePeerInfo,
        [ProtoEnum]
        NotifyNoelTransfer,
        [ProtoEnum]
        NotifyShortMsg,
        [ProtoEnum]
        NotifyGetItem,
        [ProtoEnum]
        NotifyLoseItem,
        [ProtoEnum]
        NotifyGetCoin,
        [ProtoEnum]
        NotifyLoseCoin,
        [ProtoEnum]
        NotifyRoomUpdate,
        [ProtoEnum]
        NotifyEnemyUpdate
    }
}
