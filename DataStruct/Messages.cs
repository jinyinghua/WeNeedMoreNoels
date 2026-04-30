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
        NotifyNoelMagic
    }
}
