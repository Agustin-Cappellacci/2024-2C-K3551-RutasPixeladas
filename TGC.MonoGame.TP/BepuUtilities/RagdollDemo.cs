using BepuUtilities;
using BepuPhysics;
using BepuPhysics.Collidables;
using System.Numerics;
using System;
using BepuPhysics.CollisionDetection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using BepuPhysics.Constraints;
using TGC.MonoGame.TP;

namespace TGC.MonoGame.TP;

/// <summary>
/// Bit masks which control whether different members of a group of objects can collide with each other.
/// </summary>
public struct SubgroupCollisionFilter
{
    /// <summary>
    /// A mask of 16 bits, each set bit representing a collision group that an object belongs to.
    /// </summary>
    public ushort SubgroupMembership;
    /// <summary>
    /// A mask of 16 bits, each set bit representing a collision group that an object can interact with.
    /// </summary>
    public ushort CollidableSubgroups;
    /// <summary>
    /// Id of the owner of the object. Objects belonging to different groups always collide.
    /// </summary>
    public int GroupId;

    /// <summary>
    /// Initializes a collision filter that collides with everything in the group.
    /// </summary>
    /// <param name="groupId">Id of the group that this filter operates within.</param>
    public SubgroupCollisionFilter(int groupId)
    {
        GroupId = groupId;
        SubgroupMembership = ushort.MaxValue;
        CollidableSubgroups = ushort.MaxValue;
    }

    /// <summary>
    /// Initializes a collision filter that belongs to one specific subgroup and can collide with any other subgroup.
    /// </summary>
    /// <param name="groupId">Id of the group that this filter operates within.</param>
    /// <param name="subgroupId">Id of the subgroup to put this collidable into.</param>
    public SubgroupCollisionFilter(int groupId, int subgroupId)
    {
        GroupId = groupId;
        Debug.Assert(subgroupId >= 0 && subgroupId < 16, "The subgroup field is a ushort; it can only hold 16 distinct subgroups.");
        SubgroupMembership = (ushort)(1 << subgroupId);
        CollidableSubgroups = ushort.MaxValue;
    }

    /// <summary>
    /// Disables a collision between this filter and the specified subgroup.
    /// </summary>
    /// <param name="subgroupId">Subgroup id to disable collision with.</param>
    public void DisableCollision(int subgroupId)
    {
        Debug.Assert(subgroupId >= 0 && subgroupId < 16, "The subgroup field is a ushort; it can only hold 16 distinct subgroups.");
        CollidableSubgroups ^= (ushort)(1 << subgroupId);
    }

    /// <summary>
    /// Modifies the interactable subgroups such that filterB does not interact with the subgroups defined by filter a and vice versa.
    /// </summary>
    /// <param name="a">Filter from which to remove collisions with filter b's subgroups.</param>
    /// <param name="b">Filter from which to remove collisions with filter a's subgroups.</param>
    public static void DisableCollision(ref SubgroupCollisionFilter filterA, ref SubgroupCollisionFilter filterB)
    {
        filterA.CollidableSubgroups &= (ushort)~filterB.SubgroupMembership;
        filterB.CollidableSubgroups &= (ushort)~filterA.SubgroupMembership;
    }

    /// <summary>
    /// Checks if the filters can collide by checking if b's membership can be collided by a's collidable groups.
    /// </summary>
    /// <param name="a">First filter to test.</param>
    /// <param name="b">Second filter to test.</param>
    /// <returns>True if the filters can collide, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllowCollision(in SubgroupCollisionFilter a, in SubgroupCollisionFilter b)
    {
        return a.GroupId != b.GroupId || (a.CollidableSubgroups & b.SubgroupMembership) > 0;
    }

}

/// <summary>
/// Narrow phase callbacks that prune out collisions between members of a group of objects.
/// </summary>
struct SubgroupFilteredCallbacks : INarrowPhaseCallbacks
{
    public CollidableProperty<SubgroupCollisionFilter> CollisionFilters;
    public PairMaterialProperties Material;

    public SubgroupFilteredCallbacks(CollidableProperty<SubgroupCollisionFilter> filters)
    {
        CollisionFilters = filters;
        Material = new PairMaterialProperties(1, 2, new SpringSettings(30, 1));
    }
    public SubgroupFilteredCallbacks(CollidableProperty<SubgroupCollisionFilter> filters, PairMaterialProperties material)
    {
        CollisionFilters = filters;
        Material = material;
    }


    public void Initialize(Simulation simulation)
    {
        CollisionFilters.Initialize(simulation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        //It's impossible for two statics to collide, and pairs are sorted such that bodies always come before statics.
        if (b.Mobility != CollidableMobility.Static)
        {
            return SubgroupCollisionFilter.AllowCollision(CollisionFilters[a.BodyHandle], CollisionFilters[b.BodyHandle]);
        }
        return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        pairMaterial = Material;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {
        CollisionFilters.Dispose();
    }
}

public class RagdollDemo : TGCGame
{
    static BodyHandle AddBody<TShape>(TShape shape, float mass, in RigidPose pose, Simulation simulation) where TShape : unmanaged, IConvexShape
    {
        //Note that this always registers a new shape instance. You could be more clever/efficient and share shapes, but the goal here is to show the most basic option.
        //Also, the cost of registering different shapes isn't that high for tiny implicit shapes.
        var shapeIndex = simulation.Shapes.Add(shape);
        var description = BodyDescription.CreateDynamic(pose, shape.ComputeInertia(mass), shapeIndex, 0.01f);
        return simulation.Bodies.Add(description);
    }

    static RigidPose GetWorldPose(Vector3 localPosition, Quaternion localOrientation, RigidPose ragdollPose)
    {
        RigidPose worldPose;
        RigidPose.Transform(localPosition, ragdollPose, out worldPose.Position);
        QuaternionEx.ConcatenateWithoutOverlap(localOrientation, ragdollPose.Orientation, out worldPose.Orientation);
        return worldPose;
    }
    public static void GetCapsuleForLineSegment(Vector3 start, Vector3 end, float radius, out Capsule capsule, out Vector3 position, out Quaternion orientation)
    {
        position = 0.5f * (start + end);

        var offset = end - start;
        capsule.HalfLength = 0.5f * offset.Length();
        capsule.Radius = radius;
        //The capsule shape's length is along its local Y axis, so get the shortest rotation from Y to the current orientation.
        var cross = Vector3.Cross(offset / capsule.Length, new Vector3(0, 1, 0));
        var crossLength = cross.Length();
        orientation = crossLength > 1e-8f ? QuaternionEx.CreateFromAxisAngle(cross / crossLength, (float)Math.Asin(crossLength)) : Quaternion.Identity;
    }

    public static Quaternion CreateBasis(Vector3 z, Vector3 x)
    {
        //For ease of use, don't assume that x is perpendicular to z, nor that either input is normalized.
        Matrix3x3 basis;
        basis.Z = Vector3.Normalize(z);
        basis.Y = Vector3.Normalize(Vector3.Cross(basis.Z, x));
        basis.X = Vector3.Cross(basis.Y, basis.Z);
        QuaternionEx.CreateFromRotationMatrix(basis, out var toReturn);
        return toReturn;
    }

    static AngularMotor BuildAngularMotor()
    {
        //By default, these motors use nonzero softness (inverse damping) to damp the relative motion between ragdoll pieces.
        //If you set the damping to 0 and then set the maximum force to some finite value (75 works reasonably well), the ragdolls act more like action figures.
        //You could also replace the AngularMotors with AngularServos and provide actual relative orientation goals for physics-driven animation.
        return new AngularMotor { TargetVelocityLocalA = new Vector3(), Settings = new MotorSettings(float.MaxValue, 0.01f) };
    }


}


