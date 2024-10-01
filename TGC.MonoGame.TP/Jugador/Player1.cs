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

        public Matrix carWorld { get; set; }

        private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 1000f;
        private const float carSpeedMin = -700f;
        public float carJumpSpeed {get; set;}
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.3f;


        private System.Numerics.Vector3 carPosition { get; set; }
        public float CarSpeed {get; set;}
        private const float CarMaxSpeed = 500f;
        public float CarAcceleration {get; set;}
        private const float CarBrakeForce = 5000f;
        private float CarDeceleration = 500f;

        private float wheelRotationAngle;
        private float wheelSteeringAngle;
        private float maxWheelSteer = 0.7f;
        private float wheelSteerDelta = 0.2f;
        private float CarRotationY;
        private const float CarjumpStrength = 300f;
        private float elapsedTime;
        private Simulation simulation;
        private BodyHandle carBodyHandle;
        private BodyHandle wheelBodyHandle;

        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        private GraphicsDevice graphicsDevice;

        private IPowerUp powerUp;



        public Jugador(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice)
        {
            carPosition = PositionToNumerics(new Vector3(0f, 500f, 0f));
            direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();
            //powerUp = new SuperJump(this);
            powerUp = new SuperSpeed(this);


            CarAcceleration = 500f;
            carJumpSpeed = 500f;
            

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
            // Crear una caja de colisión para el cuerpo principal del auto
            var carBox = new Box(200f, 10f, 500f); // Dimension del auto
            carBodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(carPosition + new System.Numerics.Vector3(0, 80f, 0)), carBox.ComputeInertia(50f),
            simulation.Shapes.Add(carBox), 1f));


            // Crear cajas de colisión para cada rueda
            foreach (var rueda in ruedas)
            {
                // Extraer la posición local de la rueda desde el modelo
                var wheelMatrix = rueda.ParentBone.Transform;
                var wheelPosition = Vector3.Transform(carPosition, wheelMatrix); // Convertir la posición de la rueda al espacio del mundo
                var wheelBox = new Box(100f, 100f, 100f); // Dimension de las ruedas

                // Agregar cada rueda a la simulación
                wheelBodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(PositionToNumerics(wheelPosition)),
                wheelBox.ComputeInertia(1f), // para la masa de las ruedas
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
            carBodyReference.Awake = true;
            

            // Capturar el estado del teclado
            if (keyboardState.IsKeyDown(Keys.Q)){
                powerUp.Apply();
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

            // Actualizar rotación del coche
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (CarSpeed != 0)
                {
                    if (CarSpeed >= 0)
                    {
                        CarRotationY += carSpinSpeed * elapsedTime;
                    }
                    else
                    {
                        CarRotationY -= carSpinSpeed * elapsedTime;
                    }
                }
                wheelSteeringAngle = Math.Min(wheelSteeringAngle + wheelSteerDelta, maxWheelSteer);

            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                if (CarSpeed != 0)
                {
                    if (CarSpeed >= 0)
                    {
                        CarRotationY -= carSpinSpeed * elapsedTime;
                    }
                    else
                    {
                        CarRotationY += carSpinSpeed * elapsedTime;
                    }
                }
                wheelSteeringAngle = Math.Max(wheelSteeringAngle - wheelSteerDelta, -maxWheelSteer);
            }
            else
            {
                wheelSteeringAngle = 0;
            }

            if (keyboardState.IsKeyDown(Keys.Space) && carPosition.Y<100f){
                carBodyReference.ApplyLinearImpulse(new System.Numerics.Vector3(0, carJumpSpeed, 0));
            }

            float wheelRotationDelta = CarSpeed * 0.0005f; // Ajusta este factor para que el giro sea proporcional.
            wheelRotationAngle += wheelRotationDelta;
            var currentVelocity = carBodyReference.Velocity.Linear;
            // Actualizar la velocidad del cuerpo en la simulación en base a la dirección
            var forwardDirection = System.Numerics.Vector3.Transform(System.Numerics.Vector3.UnitZ,
            System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, CarRotationY));

            // ESTA LOGICA ES PARA EVITAR PROBLEMAS CON LA GRAVEDAD Y ESAS COSAS, CUANDO APLICA LA VELOCIDAD, DESCARTA LA COMPONENTE VERTICAL
            // Proyectar la velocidad actual en la dirección hacia adelante
            
            // Separar la componente vertical (gravedad)
            var verticalVelocity = new System.Numerics.Vector3(0, currentVelocity.Y, 0);
            // Obtener la velocidad en el plano horizontal (XZ)
            var horizontalVelocity = new System.Numerics.Vector3(currentVelocity.X, 0, currentVelocity.Z);
            var forwardVelocity = System.Numerics.Vector3.Dot(horizontalVelocity, forwardDirection);
            // Eliminar el componente lateral para evitar deslizamiento, solo en el plano XZ
            var lateralDirection = horizontalVelocity - forwardVelocity * forwardDirection;
            horizontalVelocity -= lateralDirection * 0.9f; // Ajusta el valor para controlar el deslizamiento
            // Volver a combinar la velocidad horizontal con la vertical
            carBodyReference.Velocity.Linear = horizontalVelocity + verticalVelocity;


            // Aplicar la nueva velocidad en la dirección hacia adelante
            carBodyReference.ApplyLinearImpulse(forwardDirection * CarSpeed);
            // Aplicar la rotación al cuerpo físico
            carBodyReference.Pose.Orientation = System.Numerics.Quaternion.CreateFromAxisAngle(new System.Numerics.Vector3(0, 1, 0), CarRotationY);
            // Actualizar la posición y la matriz de mundo del auto
            carPosition = carBodyReference.Pose.Position;
            var rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation);
            carWorld = rotationMatrix * Matrix.CreateTranslation(carPosition);




            var frontLeftWheelPosition = Vector3.Transform(ruedas[0].ParentBone.Transform.Translation, carWorld);
            var frontRightWheelPosition = Vector3.Transform(ruedas[1].ParentBone.Transform.Translation, carWorld);
            var backLeftWheelPosition = Vector3.Transform(ruedas[2].ParentBone.Transform.Translation, carWorld);
            var backRightWheelPosition = Vector3.Transform(ruedas[3].ParentBone.Transform.Translation, carWorld);

            var vector1 = frontRightWheelPosition - frontLeftWheelPosition;
            var vector2 = backLeftWheelPosition - frontLeftWheelPosition;

            var normal = Vector3.Cross(vector1, vector2);
            normal.Normalize(); // Asegúrate de normalizar el vector normal

            normal.Y = 0;

            var targetOrientation = Microsoft.Xna.Framework.Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Vector3.Zero, normal, Vector3.Up));

            System.Numerics.Quaternion targetOrientationNumerics = new System.Numerics.Quaternion(
                targetOrientation.X,
                targetOrientation.Y,
                targetOrientation.Z,
                targetOrientation.W
            );

            carBodyReference.Pose.Orientation = targetOrientationNumerics;

            if (carPosition.Y < -3f)
            {
                carWorld = Matrix.Identity * Matrix.CreateTranslation(new Vector3(0f,300f,0f));

            }
            else
            {
                carWorld = rotationMatrix * Matrix.CreateTranslation(carPosition);
            }
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
                effectAuto.Parameters["DiffuseColor"].SetValue(color);
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
                mesh.Draw();
            }

            foreach (ModelMesh rueda in ruedas)
            {
                effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
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
            var carRotationMatrix = Matrix.CreateRotationY(CarRotationY); // Rotación del auto en Y (ajusta esto si tienes más rotaciones en otros ejes)

            // Crear la matriz de mundo del coche, que incluye rotación y traslación
            var carWorldMatrix = carRotationMatrix * Matrix.CreateTranslation(carPosition + new System.Numerics.Vector3(0, 80f, 0));

            // Dibujar la caja de colisión del auto usando la matriz de mundo del auto
            DrawBox(carWorldMatrix, new Vector3(200f, 100f, 550f), viewMatrix, projectionMatrix);
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
            }
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