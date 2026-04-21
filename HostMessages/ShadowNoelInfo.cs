using System.Numerics;
using XX;

namespace WeNeedMoreNoels.HostMessages
{
    public class ShadowNoelInfo
    {
        public Vector2 Position;

        public bool IsCrouch;

        public string Pose;

        public AIM AIM;

        public int HP;

        public int MP;

        public ushort itemID;

        public byte grade;
            
        public override string ToString()
        {
            return $"Location: x:{Position.X} y:{Position.Y} pose:{Pose} aim:{AIM}\nHP:{HP}, MP:{MP}";
        }
    }
}
