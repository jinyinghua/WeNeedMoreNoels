using ProtoBuf;

namespace WeNeedMoreNoels.DataStruct
{
    [ProtoContract]
    public class ClientConfig
    {
        [ProtoMember(1)]
        public string Nickname;
        [ProtoMember(2)]
        public NoelType NoelType;
    }

    [ProtoContract]
    public class IniConfig
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public ClientConfig ClientConfig;
    }

    [ProtoContract]
    public enum NoelType
    {
        [ProtoEnum]
        Normal,
        [ProtoEnum]
        Inverse
    }
}
