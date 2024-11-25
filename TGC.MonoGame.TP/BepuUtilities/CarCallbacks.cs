using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using TGC.MonoGame.TP;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Content.Models;

public struct CarBodyProperties
{
    public SubgroupCollisionFilter Filter;
    public float Friction;
    public bool IsWheel { get; set; } // Nueva propiedad para identificar ruedas.

    public bool isCar { get; set; }
}

/// <summary>
/// For the car demo, we want both wheel-body collision filtering and different friction for wheels versus the car body.
/// </summary>
struct CarCallbacks : INarrowPhaseCallbacks
{
    public CollidableProperty<CarBodyProperties> Properties;
    public CarControllerContainer ControllerContainer; // Contenedor mutable.
    public AutoJugadorWrapper AutoJugadorWrapper;

    public void Initialize(Simulation simulation)
    {
        Properties.Initialize(simulation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        //It's impossible for two statics to collide, and pairs are sorted such that bodies always come before statics.
        if (b.Mobility != CollidableMobility.Static)
        {
            return SubgroupCollisionFilter.AllowCollision(Properties[a.BodyHandle].Filter, Properties[b.BodyHandle].Filter);
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
        pairMaterial.FrictionCoefficient = Properties[pair.A.BodyHandle].Friction;
        if (pair.B.Mobility != CollidableMobility.Static)
        {
            //If two bodies collide, just average the friction.
            pairMaterial.FrictionCoefficient = (pairMaterial.FrictionCoefficient + Properties[pair.B.BodyHandle].Friction) * 0.5f;
        }

        if (pair.B.Mobility != CollidableMobility.Static || pair.A.Mobility != CollidableMobility.Static)
        {
            // Verifica si `pair.A` o `pair.B` son las ruedas
            //if (IsWheel(pair.A) || IsWheel(pair.B))
            //{
            AutoJugadorWrapper.AutoJugador.isGrounded = true;
            Console.Out.WriteLine("Contacto ruedas con piso");
            //
        }
        else
        {
            AutoJugadorWrapper.AutoJugador.isGrounded = false;
            Console.Out.WriteLine("NO HAY Contacto de ruedas con piso");
        }

        if (pair.A.BodyHandle == TGCGame.listaBodyHandle[0] || pair.B.BodyHandle == TGCGame.listaBodyHandle[0])
        {
            for (int i = 1; i < TGCGame.listaBodyHandle.Count; i++)
            {
                if (pair.A.BodyHandle == TGCGame.listaBodyHandle[i] || pair.B.BodyHandle == TGCGame.listaBodyHandle[i])
                {
                    var car1 = TGCGame.listaAutos[0];
                    var car2 = TGCGame.listaAutos[i];
                    var relativeVelocity = Vector3.Distance(car1.CarSpeed, car2.CarSpeed);



                    /*float speed1 = car1.velocity;
                    float speed2 = car2.Velocity.Length();
                    */
                    // Calcula el daño base a partir de la velocidad relativa
                    float baseDamage = relativeVelocity * 2f;

                    /*if (speed1 > speed2)
                    {
                        // El auto 1 golpea al auto 2 más fuerte, por lo que recibe menos daño.
                        car1.ApplyDamage(baseDamage * 0.25f); // Solo recibe el 25% del daño
                        car2.ApplyDamage(baseDamage);        // Recibe el daño completo
                    }
                    else if (speed2 > speed1)
                    {
                        // El auto 2 golpea al auto 1 más fuerte, por lo que recibe menos daño.
                        car2.ApplyDamage(baseDamage * 0.25f); // Solo recibe el 25% del daño
                        car1.ApplyDamage(baseDamage);        // Recibe el daño completo
                    }
                    else
                    {
                        // Ambos autos tienen la misma velocidad, el daño se distribuye equitativamente.
                        car1.ApplyDamage(baseDamage * 0.5f);
                        car2.ApplyDamage(baseDamage * 0.5f);
                    }*/
                    
                    car1.recibirDanio((int)baseDamage);
                    car2.recibirDanio((int)baseDamage);

                }
            }

        }

        // Calcula la velocidad relativa.


        pairMaterial.MaximumRecoveryVelocity = 2f;
        pairMaterial.SpringSettings = new SpringSettings(30, 1);
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {
        Properties.Dispose();
    }

    private bool IsWheel(CollidableReference collidable)
    {
        // Verificar si el `collidable` es una rueda.
        return Properties[collidable.BodyHandle].IsWheel;
    }



}