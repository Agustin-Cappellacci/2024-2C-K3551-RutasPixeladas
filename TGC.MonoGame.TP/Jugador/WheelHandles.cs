using BepuPhysics;

namespace TGC.MonoGame.TP.Content.Models
{

    public struct WheelHandles
    {
        public BodyHandle Wheel;
        public ConstraintHandle SuspensionSpring;
        public ConstraintHandle SuspensionTrack;
        public ConstraintHandle Hinge;
        public ConstraintHandle Motor;
    }

}