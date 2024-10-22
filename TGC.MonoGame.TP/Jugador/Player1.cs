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
using Vector3 = Microsoft.Xna.Framework.Vector3;

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
        private Matrix rotationMatrix;

        public Matrix carWorld { get; set; }

        private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 1000f;
        private const float carSpeedMin = -700f;
        public float carJumpSpeed { get; set; }
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.3f;


        private System.Numerics.Vector3 carPosition { get; set; }
        public float CarSpeed { get; set; }
        private const float CarMaxSpeed = 500f;
        public float CarAcceleration { get; set; }
        private const float CarBrakeForce = 5000f;
        private float CarDeceleration = 500f;

        private float wheelRotationAngle;
        private float wheelSteeringAngle;
        private float maxWheelSteer = 0.5f;
        private float wheelSteerDelta = 0.1f;
        private float CarRotationY;
        private const float CarjumpStrength = 300f;
        private float elapsedTime;
        private Simulation simulation;
        private BodyHandle carBodyHandle;
        private BodyHandle wheelBodyHandle;
        SimpleCarController playerController;

        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        private GraphicsDevice graphicsDevice;

        private IPowerUp powerUp;

        public int power = -1;
        public float cooldownTime = 1.5f;  // Tiempo de cooldown (1.5 segundos)
        public float cooldownTimer = 0f;   // Contador para el cooldown
        public bool isOnCooldown = false;

        private float vida = 200;

        Texture2D texturaAuto;
        Texture2D texturaRueda;




        public Jugador(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice, SimpleCarController playerController, Vector3 posicion, float angulo)
        {
            //carPosition = PositionToNumerics(new Vector3(0f, 500f, 0f));
            //direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();
            //powerUp = new SuperJump(this);
            

            texturaAuto = content.Load<Texture2D>("texturas/colorRojo");
            texturaRueda = content.Load<Texture2D>("texturas/rueda");

            CarAcceleration = 500f;
            carJumpSpeed = 2000f;

            


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

            // LOGICA DE SIMULACION, TODO LO QUE ESTA ACA SON LOS OBJETOS DE COLISION LOGICOS
            // utilizo el graphicsDevice para dibujar las cajas
            this.graphicsDevice = graphicsDevice;
            this.simulation = simulation;
            this.playerController = playerController;

            //carBodyHandle = CrearCuerpoDelAutoEnSimulacion(simulation, PositionToNumerics(posicion), angulo);


        }

        private BodyHandle CrearCuerpoDelAutoEnSimulacion(Simulation simulation, System.Numerics.Vector3 posicionInicial, float anguloInicial)
        {
            // Crear el cuerpo del coche en la simulación con una posición y orientación iniciales
            var bodyDescription = BodyDescription.CreateDynamic(
                new RigidPose(posicionInicial, System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, anguloInicial)),
                new BodyInertia(),
                new CollidableDescription(),
                new BodyActivityDescription(0.01f)
            );

            return simulation.Bodies.Add(bodyDescription);
        }


        public void Update(GameTime gameTime, Simulation simulation)
        {

            // Caputo el estado del teclado.
            var keyboardState = Keyboard.GetState();
            // La logica debe ir aca.
            elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Obtener la referencia del cuerpo del auto en la simulación
            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carBodyReference.Awake = true;

/*
<<<<<<< HEAD

            // Capturar el estado del teclado
            if (keyboardState.IsKeyDown(Keys.Q) && !isOnCooldown)
            {
                powerUp.Apply();  // Aplica el power up
                // Incrementar el poder, pero resetearlo si excede 3
                if (power < 2){
                    power++;
                } else {
                    power = 0;
                }
                // Activar el cooldown
                isOnCooldown = true;
                cooldownTimer = 0f;  // Reiniciar el temporizador
            }

            // Si está en cooldown, actualizar el temporizador
            if (isOnCooldown)
            {
                // Incrementar el temporizador con el tiempo transcurrido
                cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Si el cooldown ha terminado (supera 1.5 segundos)
                if (cooldownTimer >= cooldownTime)
                {
                    isOnCooldown = false;  // Desactivar el cooldown
                }
            }

            // Movimiento hacia adelante
            if (keyboardState.IsKeyDown(Keys.W))
            {
                CarSpeed += CarAcceleration * elapsedTime;
                if (CarSpeed > CarMaxSpeed) CarSpeed = CarMaxSpeed; // Limitar velocidad máxima
                Console.WriteLine("CarSpeed (W): " + CarSpeed);
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                CarSpeed -= CarAcceleration * elapsedTime * CarBrakeForce;
                if (CarSpeed < -CarMaxSpeed / 2) CarSpeed = -CarMaxSpeed / 2; // Limitar reversa
                Console.WriteLine("CarSpeed (S): " + CarSpeed);
            }
            else
            {
                // Desacelerar cuando no se presionan las teclas
                CarSpeed -= CarSpeed > 0 ? CarDeceleration * elapsedTime : -CarDeceleration * elapsedTime;
                if (Math.Abs(CarSpeed) < 0.1f) CarSpeed = 0; // Detener el auto cuando está casi en reposo
                Console.WriteLine("CarSpeed (no key): " + CarSpeed);
            }
*/
            float steeringSum = 0;

            // Actualizar rotación del coche
            if (keyboardState.IsKeyDown(Keys.A))
            {
                steeringSum += 1;
                wheelSteeringAngle = Math.Min(wheelSteeringAngle + wheelSteerDelta, maxWheelSteer);

            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    steeringSum -= 1;
                    wheelSteeringAngle = Math.Max(wheelSteeringAngle - wheelSteerDelta, -maxWheelSteer);
                }
                else
                {
                    wheelSteeringAngle = 0;
                }
            }


            if (keyboardState.IsKeyDown(Keys.Space) && carPosition.Y < 100f)
            {
                carBodyReference.ApplyLinearImpulse(new System.Numerics.Vector3(0, carJumpSpeed, 0));
            }

            var targetSpeedFraction = keyboardState.IsKeyDown(Keys.W) ? 10f : keyboardState.IsKeyDown(Keys.S) ? -10f : 0;
            Console.WriteLine("TargetSpeed " + targetSpeedFraction);
            playerController.Update(simulation, 1 / 60f, steeringSum, targetSpeedFraction, keyboardState.IsKeyDown(Keys.LeftAlt));
            // Actualizar la posición y la matriz de mundo del auto
            carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carPosition = carBodyReference.Pose.Position;
            rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation); //PUEDE VENIR DE ACA
            carWorld = rotationMatrix * Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(carPosition);

            Console.WriteLine("posicion del auto: " + carPosition);
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
                effectAuto.Parameters["ModelTexture"].SetValue(texturaAuto);
                //effectAuto.Parameters["DiffuseColor"].SetValue(color);
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
                mesh.Draw();
            }

            foreach (ModelMesh rueda in ruedas)
            {   
                effectAuto.Parameters["ModelTexture"].SetValue(texturaRueda);
                //effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
                if (rueda.Name.Contains("WheelA") || rueda.Name.Contains("WheelB"))
                {
                    effectAuto.Parameters["World"].SetValue(Matrix.CreateRotationX(wheelRotationAngle) * Matrix.CreateRotationY(wheelSteeringAngle) * rueda.ParentBone.Transform * carWorld);
                }
                else
                {
                    effectAuto.Parameters["World"].SetValue(Matrix.CreateRotationX(wheelRotationAngle) * rueda.ParentBone.Transform * carWorld);
                }
                rueda.Draw();
            }

            // Dibujar las cajas de colisión del auto y las ruedas
            DrawCollisionBoxes(View, Projection);
        }

        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }

        public void DrawCollisionBoxes(Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Obtener la matriz de rotación del auto (supongo que tienes una variable CarRotationY para la rotación en Y)
            //var carRotationMatrix = Matrix.CreateRotationY(rotationMatrix); // Rotación del auto en Y (ajusta esto si tienes más rotaciones en otros ejes)

            // Crear la matriz de mundo del coche, que incluye rotación y traslación
            var carWorldMatrix = rotationMatrix * Matrix.CreateTranslation(carPosition + new System.Numerics.Vector3(0, 80f, 0));

            
            // Dibujar la caja de colisión del auto usando la matriz de mundo del auto
            //DrawBox(carWorldMatrix, new Vector3(40f, -30f, 100f), viewMatrix, projectionMatrix);
            /*
            // Dibujar las cajas de colisión de las ruedas
            foreach (var rueda in ruedas)
            {
                // Obtener la matriz de transformación local de la rueda dentro del coche
                var wheelMatrix = rueda.ParentBone.Transform;

                // Obtener la matriz de mundo del coche (posición + rotación)
                var carRotationMatrixWheels = Matrix.CreateRotationY(CarRotationY); // Supongo que tienes una variable CarRotationY para la rotación del coche en Y
                var carWorldMatrixWheels = carRotationMatrixWheels * Matrix.CreateTranslation(carPosition);

                // Transformar la posición local de la rueda a la posición en el mundo
                var wheelPositionInWorld = Vector3.Transform(Vector3.Zero, wheelMatrix * carWorldMatrixWheels);

                // Ajusta las dimensiones de las cajas de colisión según tus necesidades
                var wheelBox = new Box(100f, 100f, 100f); // Cambia las dimensiones según tu modelo

                // Dibujar la caja de colisión para cada rueda
                DrawBox(Matrix.CreateTranslation(wheelPositionInWorld), new Vector3(100f, 100f, 100f), viewMatrix, projectionMatrix);
            } */
        }

        private void ElevateWheel(BodyReference wheelBodyReference)
        {
            // Aquí ajustas la posición de la rueda en función de la colisión
            var wheelPosition = wheelBodyReference.Pose.Position;

            // Simula que la rueda se eleva al subir la rampa
            wheelPosition.Y += 50f;  // Puedes ajustar este valor en función de la altura de la rampa

            // Actualiza la nueva posición de la rueda en la simulación
            wheelBodyReference.Pose.Position = wheelPosition;
        }

        public void DrawBox(Matrix worldMatrix, Vector3 size, Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Crear un efecto básico para dibujar la caja
            BasicEffect effect = new BasicEffect(graphicsDevice);
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.VertexColorEnabled = true;

            // Definir los vértices de una caja (un cubo unitario que escalaremos)
            VertexPositionColor[] vertices = new VertexPositionColor[8];
            vertices[0] = new VertexPositionColor(new Vector3(-1, 1, 1), Color.Red);   // Front top left
            vertices[1] = new VertexPositionColor(new Vector3(1, 1, 1), Color.Red);    // Front top right
            vertices[2] = new VertexPositionColor(new Vector3(-1, -1, 1), Color.Red);  // Front bottom left
            vertices[3] = new VertexPositionColor(new Vector3(1, -1, 1), Color.Red);   // Front bottom right
            vertices[4] = new VertexPositionColor(new Vector3(-1, 1, -1), Color.Red);  // Back top left
            vertices[5] = new VertexPositionColor(new Vector3(1, 1, -1), Color.Red);   // Back top right
            vertices[6] = new VertexPositionColor(new Vector3(-1, -1, -1), Color.Red); // Back bottom left
            vertices[7] = new VertexPositionColor(new Vector3(1, -1, -1), Color.Red);  // Back bottom right

            // Escalar la caja en función del tamaño dado
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position *= size / 2f;
            }

            // Definir los índices que forman las líneas de la caja
            int[] indices = new int[]
            {
        0, 1, 1, 3, 3, 2, 2, 0,  // Front face
        4, 5, 5, 7, 7, 6, 6, 4,  // Back face
        0, 4, 1, 5, 2, 6, 3, 7   // Connecting edges
            };

            // Dibujar la caja usando el efecto básico
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertices,
                    0,
                    vertices.Length,
                    indices,
                    0,
                    indices.Length / 2
                );
            }
        }



    }
}