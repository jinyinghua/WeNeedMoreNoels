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
    }

    [ProtoContract] 
    public class NotifyNoelDamage
    {
        [ProtoMember(1)]
        public int Hp;
        [ProtoMember(2)]
        public int Mp;
    }
}
