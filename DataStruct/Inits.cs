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
        [ProtoMember(3)]
        public ColorNoelColor NoelColor;
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
        Inverse,
        [ProtoEnum]
        ColorNoel
    }

    [ProtoContract]
    public enum ColorNoelColor
    {
        [ProtoEnum]
        Blue,
        [ProtoEnum]
        Cyan,
        [ProtoEnum]
        Green,
        [ProtoEnum]
        Orange,
        [ProtoEnum]
        Pink,
        [ProtoEnum]
        Purple,
        [ProtoEnum]
        Red,
        [ProtoEnum]
        Yellow
    }
}
