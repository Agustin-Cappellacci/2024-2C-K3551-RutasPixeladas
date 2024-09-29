using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private Microsoft.Xna.Framework.Vector3 direccionFrontal { get; set; }
        private Matrix carRotation = Matrix.CreateRotationY(0f);

        public Matrix carWorld { get; set; }
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

        private System.Numerics.Vector3 carPosition { get; set; }
        private float CarSpeed;
        private const float CarMaxSpeed = 1000f;
        private const float CarAcceleration = 500f;
        private const float CarBrakeForce = 800f;
        private float CarDeceleration = 500f;

        private float CarRotationY;
        private float CarVerticalVelocity;
        private const float CarjumpStrength = 300f;
        private float elapsedTime;

        private Matrix ruedaDelanteraTransform;

        private Matrix ruedaTraseraTransform;
        private Simulation simulation;

        private BodyHandle carBodyHandle;
        private BodyHandle wheelBodyHandle;

        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;

        public Jugador(ContentManager content, Simulation simulation)
        {
            carPosition = new System.Numerics.Vector3(0f, 10f, 0f);
            direccionFrontal = Microsoft.Xna.Framework.Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();

            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // Aquí verificas si el nombre del mesh corresponde a una rueda
                foreach (var meshPart in mesh.MeshParts)
                {

                    meshPart.Effect = effectAuto;
                }
                if (mesh.Name.Contains("Wheel"))
                {
                    ruedas.Add(mesh);
                }
                else restoAuto.Add(mesh);

            }

            this.simulation = simulation;
            // Crear una caja de colisión para el cuerpo principal del auto
            var carBox = new Box(2f, 1f, 4f); // Cambia las dimensiones según tu modelo
            carBodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(carPosition), carBox.ComputeInertia(1000f),
            simulation.Shapes.Add(carBox), 1f));

            // Crear cajas de colisión para cada rueda
            foreach (var rueda in ruedas)
            {
                // Extraer la posición local de la rueda desde el modelo
                var wheelMatrix = rueda.ParentBone.Transform;
                var wheelPosition = Microsoft.Xna.Framework.Vector3.Transform(carPosition, wheelMatrix); // Convertir la posición de la rueda al espacio del mundo
                // Ajusta las posiciones y dimensiones de las cajas según tus necesidades
                var wheelBox = new Box(0.5f, 0.5f, 0.5f); // Cambia las dimensiones según tu modelo
                //System.Numerics.Vector3 wheelPosition = carPosition + new System.Numerics.Vector3(0, -0.5f, 1); // Ejemplo de posición

                // Agregar cada rueda a la simulación
                wheelBodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(PositionToNumerics(wheelPosition)),
                wheelBox.ComputeInertia(10f), // Cambia la masa según sea necesario
                simulation.Shapes.Add(wheelBox), 1f));
            }
        }

        public void Update(GameTime gameTime)
        {

            // Caputo el estado del teclado.
            var keyboardState = Keyboard.GetState();
            // La logica debe ir aca.
            elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Obtener la referencia del cuerpo del auto en la simulación
            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            // Capturar el estado del teclado

            // Movimiento hacia adelante
            if (keyboardState.IsKeyDown(Keys.W))
            {
                CarSpeed += CarAcceleration * elapsedTime;
                if (CarSpeed > CarMaxSpeed) CarSpeed = CarMaxSpeed; // Limitar velocidad máxima
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                CarSpeed -= CarAcceleration * elapsedTime;
                if (CarSpeed < -CarMaxSpeed / 2) CarSpeed = -CarMaxSpeed / 2; // Limitar reversa
            }
            else
            {
                // Desacelerar cuando no se presionan las teclas
                CarSpeed -= CarSpeed > 0 ? CarDeceleration * elapsedTime : -CarDeceleration * elapsedTime;
                if (Math.Abs(CarSpeed) < 0.1f) CarSpeed = 0; // Detener el auto cuando está casi en reposo
            }
            // Actualizar la velocidad del cuerpo en la simulación en base a la dirección
            var forwardDirection = System.Numerics.Vector3.Transform(System.Numerics.Vector3.UnitZ,
       System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, CarRotationY));
            carBodyReference.Velocity.Linear = forwardDirection * CarSpeed;

            // Actualizar rotación del coche
            if (keyboardState.IsKeyDown(Keys.A))
            {
                CarRotationY += carSpinSpeed * elapsedTime;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                CarRotationY -= carSpinSpeed * elapsedTime;
            }

            // Movimiento del coche en base a la velocidad
            //carPosition += System.Numerics.Vector3.Transform(System.Numerics.Vector3.UnitZ * CarSpeed * elapsedTime, Matrix4x4.CreateRotationY(CarRotationY));
            // Aplicar la rotación al cuerpo físico
            carBodyReference.Pose.Orientation = System.Numerics.Quaternion.CreateFromAxisAngle(new System.Numerics.Vector3(0, 1, 0), CarRotationY);
            carPosition = carBodyReference.Pose.Position;
            // Actualizar la posición de las ruedas basadas en la posición general del auto
            // Actualizar la matriz del mundo del auto
            var rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation);
            carWorld = rotationMatrix * Matrix.CreateTranslation(carPosition);
        }

        public void Draw(Matrix View, Matrix Projection)
        {
            var random = new Random(Seed: 0);
            var color = new Microsoft.Xna.Framework.Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
            var colorRueda = new Microsoft.Xna.Framework.Vector3(0, 0, 0);
            effectAuto.Parameters["View"].SetValue(View);
            effectAuto.Parameters["Projection"].SetValue(Projection);


            foreach (ModelMesh mesh in restoAuto)
            {
                effectAuto.Parameters["DiffuseColor"].SetValue(color);
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
                mesh.Draw();
            }

            foreach (ModelMesh rueda in ruedas)
            {
                effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
                if (rueda.Name.Contains("WheelA") || rueda.Name.Contains("WheelB"))
                {
                    effectAuto.Parameters["World"].SetValue(ruedaDelanteraTransform);
                }
                else
                {
                    effectAuto.Parameters["World"].SetValue(ruedaTraseraTransform);
                }
                rueda.Draw();
            }
        }

        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }
    }
}
