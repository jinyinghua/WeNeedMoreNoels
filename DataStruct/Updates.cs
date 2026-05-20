using nel;
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
        [ProtoMember(18)]
        public bool IsEvadePD;
        [ProtoMember(19)]
        public bool IsEvadeO;
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
        [ProtoMember(4)]
        public float da;
    }

    [ProtoContract]
    public enum NotifyMagicTpe
    {
        [ProtoEnum]
        Reawake,
        [ProtoEnum]
        Sleep,
        [ProtoEnum]
        Kill,
        [ProtoEnum]
        Turn
    }

    [ProtoContract]
    public class UpdatePeerInfo
    {
        [ProtoMember(1)]
        public UpdatePeerType Type;
        [ProtoMember(2)]
        public string NickName;
        [ProtoMember(3)]
        public int PartyID;
    }

    [ProtoContract]
    public enum UpdatePeerType
    {
        Nickname,
        Party
    }

    [ProtoContract]
    public class NotifyNoelTransfer
    {
        [ProtoMember(1)]
        public string Key;
        [ProtoMember(2)]
        public float X;
        [ProtoMember(3)]
        public float Y;
    }

    [ProtoContract]
    public class NotifyShortMsg
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public string key;
    }

    [ProtoContract]
    public class NotifyItemChanged
    {
        [ProtoMember(1)]
        public int PartyID;
        [ProtoMember(2)]
        public string key;
        [ProtoMember(3)]
        public int count;
        [ProtoMember(4)]
        public int grade;
    }

    [ProtoContract]
    public class NotifyCoinChanged
    {
        [ProtoMember(1)]
        public int PartyID;
        [ProtoMember(2)]
        public CoinStorage.CTYPE coinType;
        [ProtoMember(3)]
        public int count;
    }

    [ProtoContract]
    public class NotifyRoomUpdate
    {
        [ProtoMember(1)]
        public bool EnablePVP;
    }

    [ProtoContract]
    public class UpdateEnemyInfo
    {
        [ProtoMember(1)]
        public string Key;
        [ProtoMember(2)]
        public float PositionX;
        [ProtoMember(3)]
        public float PositionY;
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
        public float T;
    }

    [ProtoContract]
    public class NotifyEnemyDamage
    {
        [ProtoMember(1)]
        public string Key;
        [ProtoMember(2)]
        public int Hp;
        [ProtoMember(3)]
        public int Mp;
    }
}
