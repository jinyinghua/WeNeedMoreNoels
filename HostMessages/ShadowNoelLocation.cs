using System.Numerics;
using XX;

namespace WeNeedMoreNoels.HostMessages
{
    public class ShadowNoelLocation
    {
        public Vector2 Position;

        public bool IsCrouch;

        public string Pose;

        public AIM AIM;

        public override string ToString()
        {
            return $"Location: x:{Position.X} y:{Position.Y} pose:{Pose} aim:{AIM}";
        }
    }
}
