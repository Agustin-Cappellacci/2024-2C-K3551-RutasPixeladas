using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TGC.MonoGame.TP;

namespace TGC.MonoGame.TP.Content.Models
{
    // Nunca olvides mesh.ParentBone.Transform

    class Jugador
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        private Model Model { get; set; }
        private Effect effectAuto { get; set; }

        // Jugabilidad
        private Vector3 direccionFrontal { get; set; }
        private Vector3 carPosition { get; set; }
        private Matrix carRotation = Matrix.CreateRotationY(0f);

        private float carSpeed = 0f;
        private float carVerticalSpeed = 0f;
        private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 1000f;
        private const float carSpeedMin = -700f;
        private const float carJumpSpeed = 50f;
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.4f;
        private float angle = 0f;

        // Colisiones
        private BodyHandle bodyHandle;
        private Simulation simulation;


        public Jugador(ContentManager content, Simulation simulation)
        {
            carPosition = new Vector3(0f, 1500f, 0f);
            direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            // Aproximadamente las dimensiones del auto para la caja (modifica según sea necesario)
            float carWidth = 2.0f;  // Ancho de la caja
            float carHeight = 1.0f; // Altura de la caja
            float carLength = 4.0f; // Largo de la caja

            // array de vertices a lista, para manejo de colisiones
            //List<System.Numerics.Vector3> verticesList = new List<System.Numerics.Vector3>();
            /*
            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectAuto;

                    // Extraer los vértices del VertexBuffer
                    VertexBuffer vertexBuffer = meshPart.VertexBuffer;
                    VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[vertexBuffer.VertexCount];
                    vertexBuffer.GetData(vertices);

                    foreach (var vertex in vertices)
                    {
                        verticesList.Add(PositionToNumerics(vertex.Position));
                    }
                }
            }
            

            // transformo lista a span para parametro de convexHull
            Span<System.Numerics.Vector3> verticesSpan = CollectionsMarshal.AsSpan(verticesList);

            // Calcular el centroide
            Vector3 centroid = Vector3.Zero;
            foreach (var vertex in verticesList)
            {
                centroid += vertex;
            }
            centroid /= verticesList.Count;
            */

            this.simulation = simulation;
            // creacion de ConvexHull
            var initialPosition = carPosition;
            var initialOrientation = Quaternion.Identity;
            // transformo a system.numerics
            var numericPosition = PositionToNumerics(initialPosition);
            var numericQuaternion = QuaternionToNumerics(initialOrientation);
            var pose = new RigidPose(numericPosition, numericQuaternion);
            //var hullShape = new ConvexHull(verticesSpan, simulation.BufferPool, out System.Numerics.Vector3 hullCenter);
            //var inertia = hullShape.ComputeInertia(1500.0f);
            
            var boxShape = new Box(carWidth, carHeight, carLength);
            var inertia = boxShape.ComputeInertia(1000f);
            //bodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(pose, inertia, simulation.Shapes.Add(hullShape), 0.0f));
            bodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(pose, inertia, simulation.Shapes.Add(boxShape), 1f));
        }

        public Matrix Update(GameTime gameTime, Matrix carWorld)
        {
            // Obtener la posición del cuerpo desde la simulación física
            var bodyReference = simulation.Bodies.GetBodyReference(bodyHandle);
            var bodyPose = bodyReference.Pose;


            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                carSpeed = Math.Min(carSpeed + carAcceleration, carSpeedMax);
                //carPosition = carPosition + (direccionFrontal * elapsedTime * carSpeed);
                var moveDirection = PositionToNumerics(direccionFrontal * elapsedTime * carSpeed);
                bodyReference.Velocity.Linear += moveDirection;

            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                carSpeed = Math.Max(carSpeed - carAcceleration, carSpeedMin);
                //carPosition = carPosition + (direccionFrontal * elapsedTime * carSpeed);
                var moveDirection = PositionToNumerics(direccionFrontal * elapsedTime * carSpeed);
                bodyReference.Velocity.Linear += moveDirection;
            }

            /*
            if (keyboardState.IsKeyDown(Keys.W))
            {
                var moveForce = PositionToNumerics(direccionFrontal) * (carAcceleration * elapsedTime);
                bodyReference.ApplyLinearImpulse(moveForce);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                var moveForce = PositionToNumerics(-direccionFrontal) * (carAcceleration * elapsedTime);
                bodyReference.ApplyLinearImpulse(moveForce);
            }
            */
            if (carPosition.Y <= 0f & keyboardState.IsKeyDown(Keys.Space))
            {
                carVerticalSpeed = carJumpSpeed;
                //carPosition += Vector3.Up * carVerticalSpeed;
                bodyReference.Velocity.Linear += new System.Numerics.Vector3(0, carVerticalSpeed, 0);
            }
            else if (carPosition.Y > 0f)
            {
                carVerticalSpeed -= gravity * elapsedTime;
                //carPosition += Vector3.Up * carVerticalSpeed;
                bodyReference.Velocity.Linear += new System.Numerics.Vector3(0, carVerticalSpeed, 0);
            }

            // #region TODO Rotacion
            // Cuando multiplicamos la rotación con la Matrix el auto desaparece. Buscar Causa. Lo dejo así para que el problema sea más visible xd 


            if (keyboardState.IsKeyDown(Keys.A))
            {
                angle -= carSpinSpeed * elapsedTime;
                carRotation = Matrix.CreateFromQuaternion(new Quaternion(0, MathF.Sin(angle * 0.5f), 0, MathF.Cos(angle * 0.5f)));
                direccionFrontal = Vector3.Normalize(new Vector3
                {
                    X = MathF.Sin(angle),
                    Y = 0,
                    Z = MathF.Cos(angle)
                });

                // Rotar el cuerpo físico
                bodyReference.Pose.Orientation = QuaternionToNumerics(new Quaternion(0, MathF.Sin(angle * 0.5f), 0, MathF.Cos(angle * 0.5f)));
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                angle += carSpinSpeed * elapsedTime;
                carRotation = Matrix.CreateFromQuaternion(new Quaternion(0, MathF.Sin(angle * 0.5f), 0, MathF.Cos(angle * 0.5f)));

                direccionFrontal = Vector3.Normalize(new Vector3
                {
                    X = MathF.Sin(angle),
                    Y = 0,
                    Z = MathF.Cos(angle)
                });

                bodyReference.Pose.Orientation = QuaternionToNumerics(new Quaternion(0, MathF.Sin(angle * 0.5f), 0, MathF.Cos(angle * 0.5f)));
            }



            // #endregion
            // Actualizar la posición del auto en el mundo de MonoGame
            carPosition = new Vector3(bodyPose.Position.X, bodyPose.Position.Y, bodyPose.Position.Z);

            var random = new Random(Seed: 0);
            var scale = 1f + (0.1f - 0.05f) * random.NextSingle();
            carWorld = Matrix.CreateScale(scale) * carRotation * Matrix.CreateTranslation(carPosition);

            // #region TODO Efectos
            /*
                foreach (var mesh in autoJugador.Meshes)
                {
                    // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = effectoAuto;
                    }
                }
            */
            // #endregion
            Console.WriteLine($"Posición del auto: {simulation.Bodies.GetBodyReference(bodyHandle).Pose.Position}");
            return carWorld;
        }

        public void Draw(Matrix CarWorld, Matrix View, Matrix Projection)
        {
            var random = new Random(Seed: 0);
            var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
            effectAuto.Parameters["View"].SetValue(View);
            effectAuto.Parameters["Projection"].SetValue(Projection);


            foreach (ModelMesh mesh in Model.Meshes)
            {
                effectAuto.Parameters["DiffuseColor"].SetValue(color);
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * CarWorld);



                mesh.Draw();
            }
        }




        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }
        public static System.Numerics.Quaternion QuaternionToNumerics(Microsoft.Xna.Framework.Quaternion xnaQuat)
        {
            return new System.Numerics.Quaternion(xnaQuat.X, xnaQuat.Y, xnaQuat.Z, xnaQuat.W);
        }
    }
}
