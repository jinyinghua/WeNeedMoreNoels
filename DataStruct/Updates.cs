using ProtoBuf;

namespace WeNeedMoreNoels.DataStruct
{
    [ProtoContract]
    public class UpdateNoelInfo
    {
        [ProtoMember(1)]
        public float PositionX;
        [ProtoMember(2)]
        public float PositionY;
        [ProtoMember(3)]
        public bool IsCrouch;
        [ProtoMember(4)]
        public string Pose;
        [ProtoMember(5)]
        public int Aim;
        [ProtoMember(6)]
        public int Hp;
        [ProtoMember(7)]
        public int Mp;
        [ProtoMember(8)]
        public int State;
        [ProtoMember(9)]
        public int CaneItemId;
        [ProtoMember(10)]
        public int CaneGrade;
        [ProtoMember(11)]
        public int PartyID;
        [ProtoMember(12)]
        public string MpKey;
        [ProtoMember(13)]
        public bool ChantMagic;
        [ProtoMember(14)]
        public float MagicAgR;
        [ProtoMember(15)]
        public float MagicHold;
        [ProtoMember(16)]
        public float MagicT;
        [ProtoMember(17)]
        public int MagicHoldAim;
    }

    [ProtoContract] 
    public class NotifyNoelDamage
    {
        [ProtoMember(1)]
        public int Hp;
        [ProtoMember(2)]
        public int Mp;
    }

    [ProtoContract]
    public class NotifyNoelMagic
    {
        [ProtoMember(1)]
        public NotifyMagicTpe Type;
        [ProtoMember(2)]
        public int Kind;
        [ProtoMember(3)]
        public float T;
    }

    [ProtoContract]
    public enum NotifyMagicTpe
    {
        [ProtoEnum]
        Reawake,
        [ProtoEnum]
        Sleep,
        [ProtoEnum]
        Kill
    }
}
