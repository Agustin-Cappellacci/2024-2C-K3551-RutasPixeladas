using BepuPhysics;

namespace TGC.MonoGame.TP.Content.Models;

struct WheelHandles
{
    public BodyHandle Wheel;
    public ConstraintHandle SuspensionSpring;
    public ConstraintHandle SuspensionTrack;
    public ConstraintHandle Hinge;
    public ConstraintHandle Motor;
}