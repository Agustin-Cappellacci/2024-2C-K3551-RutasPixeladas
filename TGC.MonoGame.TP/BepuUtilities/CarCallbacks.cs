using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
        var cant = 0;
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
            for (int i = 1; i < AutoJugadorWrapper.autoEnemigos.Count; i++)
            {
                if (pair.A.BodyHandle == TGCGame.listaBodyHandle[i] || pair.B.BodyHandle == TGCGame.listaBodyHandle[i])
                {

                    Console.Write("choca con" + i);
                    var car1 = AutoJugadorWrapper.AutoJugador;
                    var carsEnemigos = AutoJugadorWrapper.autoEnemigos[i];
                    var relativeVelocity = Vector3.Distance(car1.CarSpeed, carsEnemigos.CarSpeed);

                    Console.Write("relativeVelocity: " + relativeVelocity + "\n");

                    if (!carsEnemigos.estaMuerto)
                    {

                        if (AutoJugadorWrapper.autoEnemigos[i].ColisionCaja.Intersects(AutoJugadorWrapper.AutoJugador.ColisionCaja))
                        {
                            cant++;

                            if (cant == 1)
                            {
                                if (relativeVelocity >= 1f)
                                {
                                    float baseDamage = Math.Min(relativeVelocity, 10);

                                    Console.Write("danio: " + baseDamage + "\n");

                                    car1.recibirDanio((int)baseDamage);
                                    carsEnemigos.recibirDanio(car1, (int)baseDamage);
                                }

                            }
                        }
                        else
                        {
                            cant = 0;
                        }
                    }

                    //Calcula el daño base a partir de la velocidad relativa

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