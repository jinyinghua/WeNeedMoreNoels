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
        [ProtoMember(4)]
        public bool EmptyNickname;
    }

    [ProtoContract]
    public class PartyConfig
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public float R;
        [ProtoMember(3)]
        public float G;
        [ProtoMember(4)]
        public float B;
        [ProtoMember(5)]
        public float A;
        [ProtoMember(6)]
        public string Name;
    }

    [ProtoContract]
    public class IniConfig
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public ClientConfig ClientConfig;
        [ProtoMember(3)]
        public PartyConfig PartyConfig;
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
