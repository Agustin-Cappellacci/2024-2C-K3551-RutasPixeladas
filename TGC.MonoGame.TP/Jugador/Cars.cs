using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TGC.MonoGame.TP;
using Vector3 = Microsoft.Xna.Framework.Vector3;


namespace TGC.MonoGame.TP.Content.Models
{

    public abstract class AutoEnemigo
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
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


        public System.Numerics.Vector3 carPosition { get; set; }
        public Vector3 CarSpeed { get; set; }
        public BepuPhysics.BodyVelocity velocity;
        private const float CarMaxSpeed = 500f;
        public float CarAcceleration { get; set; }
        private const float CarBrakeForce = 5000f;
        private float CarDeceleration = 500f;

        protected float wheelRotationAngle;
        protected float wheelSteeringAngle;
        private float maxWheelSteer = 0.5f;
        private float wheelSteerDelta = 0.1f;
        private float CarRotationY;
        private const float CarjumpStrength = 300f;
        private float elapsedTime;
        private Simulation simulation;
        public BodyHandle carBodyHandle;
        private BodyHandle wheelBodyHandle;
        SimpleCarController playerControllers;

        // como esto es private no se dibujan los autos ya que nunca actualizan el modelo
        protected List<ModelMesh> ruedas;
        protected List<ModelMesh> restoAuto;
        public GraphicsDevice graphicsDevice;

        private IPowerUp powerUp;

        public float vida = 50;

        protected Model CarModel { get; set; }
        public BoundingBox ColisionCaja { get; set; }

        public Vector3 colorAuto = new Vector3(1f, 0f, 0f);
        public float velocidadIA = 10f;

        public bool estaMuerto = false;



        // Constructor abstracto
        protected AutoEnemigo(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice,/* SimpleCarController playerController,*/ Vector3 posicion, float angulo, BodyHandle bodyHandle)
        {

            carWorld = Matrix.Identity;
            var random = new Random();  // No hace falta un seed porque se usa una sola vez y se guarda en una variable.

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();
            carBodyHandle = bodyHandle;

            effectAuto = content.Load<Effect>(ContentFolderEffects + "ModelsTexture");
            var texture = content.Load<Texture2D>(ContentFolder3D + "autos/RacingCarA/TEX"); // Asegúrate de usar la ruta correcta
            effectAuto.Parameters["ModelTexture"].SetValue(texture);
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

        protected abstract void CargarModelo(ContentManager content);

        public abstract void Update(GameTime gameTime, Simulation simulation, SimpleCarController simpleCarController, Vector3 posicionJugador);
        public abstract void Draw(GameTime gametime, Matrix View, Matrix Projection);

        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }


        private DateTime ultimoDanio = DateTime.MinValue; // Rastrea el tiempo del último daño
        private const int intervaloDanioMs = 1000;
        public void recibirDanio(Jugador jugador, int danio)
        {
            if ((DateTime.Now - ultimoDanio).TotalMilliseconds >= intervaloDanioMs)
            {
                ultimoDanio = DateTime.Now; // Actualiza el tiempo del último daño
                vida = Math.Max(vida - danio, 0);
                if (vida == 0)
                {   
                    estaMuerto = true;
                    jugador.cantidadBajas++;
                    colorAuto = new Vector3(0, 0, 0);
                    velocidadIA = 0f;
                }
            }
        }
    }

    class AutoEnemigoCombate : AutoEnemigo
    {
        private Matrix rotationMatrix;
        public System.Numerics.Vector3 carPosition { get; set; }
        public Effect effectAuto { get; set; }
        public BodyHandle carBodyHandleCombat;

        private Random random = new Random();
        private float timeSinceLastRandomChange = 0f;
        private float randomSteering = 0f;
        private float randomSpeed = 0f;
        private float randomChangeInterval = 2f;





        float Escala;
        //private List<ModelMesh> ruedas;
        //private List<ModelMesh> restoAuto;
        public AutoEnemigoCombate(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice, Vector3 posicion, float angulo, BodyHandle carBodyHandle)
            : base(content, simulation, graphicsDevice, posicion, angulo + (float)Math.PI / 2, carBodyHandle) //Ajustar ángulo si es necesario
        {
            this.graphicsDevice = graphicsDevice;
            effectAuto = content.Load<Effect>(ContentFolderEffects + "diffuseColor2");
            CargarModelo(content);
            Escala = 0.004f + (0.004f - 0.001f) * new Random().NextSingle();
            this.carBodyHandleCombat = carBodyHandle;


            //carBodyHandle = CrearCuerpoDelAutoEnSimulacion(simulation, PositionToNumerics(posicion), angulo);
        }

        /*
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
       */

        public override void Update(GameTime gameTime, Simulation simulation, SimpleCarController simpleCarController, Vector3 posicionJugador)
        {
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            timeSinceLastRandomChange += elapsedTime;

            // Obtener la referencia del cuerpo del auto en la simulación
            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandleCombat);
            carBodyReference.Awake = true;

            // Vector hacia el objetivo
            float distanceToPlayer = Vector3.Distance(posicionJugador, carBodyReference.Pose.Position);
            if (distanceToPlayer < 1000f)
            {
                Vector3 directionToTarget = Vector3.Normalize(posicionJugador - carBodyReference.Pose.Position);


                // Calcular la rotación del auto actual
                Vector3 forwardVector = Vector3.Transform(Vector3.Backward, Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation));
                float dotProduct = Vector3.Dot(forwardVector, directionToTarget);
                float crossProduct = Vector3.Cross(forwardVector, directionToTarget).Y;

                // Ajustar el ángulo de dirección basado en los productos
                float steeringSum = Math.Clamp(crossProduct, -1f, 1f);

                // Agregar un rango de ángulo para el movimiento hacia adelante
                float angleThreshold = 0.7f; // Rango de ángulo para mover hacia adelante

                bool isFlipped = rotationMatrix.Up.Y < 0; // Si Y es negativo, el auto está al revés
                if (isFlipped)
                {
                    // Rota el auto para enderezarlo
                    carBodyReference.Pose.Orientation = System.Numerics.Quaternion.Identity;
                    carBodyReference.Pose.Position = new System.Numerics.Vector3(
                    carBodyReference.Pose.Position.X,
                        carBodyReference.Pose.Position.Y + 1, // Ajusta la altura si es necesario
                        carBodyReference.Pose.Position.Z
                    );
                }
                // Si está volcado y se presiona "R"


                // Si el auto está dentro del rango angular para moverse hacia adelante, mueve el auto
                float targetSpeedFraction = (dotProduct > angleThreshold) ? velocidadIA : (dotProduct < -angleThreshold) ? -velocidadIA / 2 : velocidadIA / 5;

                simpleCarController.Update(simulation, 1 / 60f, velocidadIA > 0 ? steeringSum : 0, targetSpeedFraction, false);
            }
            else
            {
                // **Random Movement**

                // Change direction at set intervals
                if (timeSinceLastRandomChange >= randomChangeInterval)
                {
                    // Generate random steering between -1 and 1
                    randomSteering = (float)(random.NextDouble() * 2 - 1);

                    // Generate random speed between 0 and 10
                    randomSpeed = (float)(random.NextDouble() * 10f);

                    // Reset the timer
                    timeSinceLastRandomChange = 0f;
                }

                // Update the car controller with random values
                simpleCarController.Update(simulation, 1 / 60f, randomSteering, randomSpeed, false);

            }
            carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandleCombat);
            carPosition = carBodyReference.Pose.Position;
            rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation) * Matrix.CreateRotationY(MathHelper.ToRadians(90));
            carWorld = rotationMatrix * Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(carPosition);
            ColisionCaja = new BoundingBox(carBodyReference.BoundingBox.Min, carBodyReference.BoundingBox.Max);
            CarSpeed = carBodyReference.Velocity.Linear;
            velocity = carBodyReference.Velocity;
        }

        protected override void CargarModelo(ContentManager content)
        {
            this.CarModel = content.Load<Model>(ContentFolder3D + "autos/CombatVehicle/Vehicle");

            foreach (var mesh in CarModel.Meshes)
            {
                Console.WriteLine(mesh.Name); // Verifica los nombres de los meshes
            }

            foreach (var mesh in CarModel.Meshes)
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
        }
        public override void Draw(GameTime gametime, Matrix View, Matrix Projection)
        {
            var random = new Random(Seed: 0);
            var color = colorAuto;
            var colorRueda = new Microsoft.Xna.Framework.Vector3(0, 0, 0);
            effectAuto.Parameters["View"].SetValue(View);
            effectAuto.Parameters["Projection"].SetValue(Projection);
            var modelMeshesBaseTransforms = new Matrix[CarModel.Bones.Count];
            CarModel.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);
            foreach (ModelMesh mesh in restoAuto)
            {
                effectAuto.Parameters["DiffuseColor"]?.SetValue(colorAuto);
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
                mesh.Draw();
            }

            //DrawBoundingBox(ColisionCaja, graphicsDevice, View, Projection);

            foreach (ModelMesh rueda in ruedas)
            {
                effectAuto.Parameters["DiffuseColor"]?.SetValue(colorRueda);
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
        }

        public void DrawBoundingBox(BoundingBox boundingBox, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            var corners = boundingBox.GetCorners();
            var vertices = new VertexPositionColor[24];

            // Define color para las líneas del bounding box
            var color = Color.Red;

            // Asigna los vértices de las líneas de cada borde del bounding box
            int[] indices = { 0, 1, 1, 2, 2, 3, 3, 0, // Bottom face
                      4, 5, 5, 6, 6, 7, 7, 4, // Top face
                      0, 4, 1, 5, 2, 6, 3, 7  // Vertical edges
                    };


            for (int i = 0; i < indices.Length; i++)
            {
                vertices[i] = new VertexPositionColor(corners[indices[i]], color);
            }

            var basicEffect = new BasicEffect(graphicsDevice)
            {
                World = Matrix.Identity,
                View = view,
                Projection = projection,
                VertexColorEnabled = true
            };

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 12);
            }
        }

    }

    class AutoEnemigoCarrera : AutoEnemigo
    {
        private Matrix rotationMatrix;
        public System.Numerics.Vector3 carPosition { get; set; }

        public Effect effectAuto { get; set; }

        float Escala;
        //private List<ModelMesh> ruedas;
        //private List<ModelMesh> restoAuto;
        SimpleCarController playerController;
        public AutoEnemigoCarrera(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice, Vector3 posicion, float angulo, BodyHandle carBodyHandle)
            : base(content, simulation, graphicsDevice, posicion, angulo, carBodyHandle) // Ajustar ángulo si es necesario
        {
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            CargarModelo(content);
            Escala = 0.1f + (0.1f - 0.05f) * new Random().NextSingle();
        }

        public override void Update(GameTime gameTime, Simulation simulation, SimpleCarController simpleCarController, Vector3 posicionJugador)
        {
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Obtener la referencia del cuerpo del auto en la simulación
            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carBodyReference.Awake = true;

            // Vector hacia el objetivo
            Vector3 directionToTarget = Vector3.Normalize(posicionJugador - carBodyReference.Pose.Position);

            // Calcular la rotación del auto actual
            Vector3 forwardVector = Vector3.Transform(Vector3.Backward, Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation));
            float dotProduct = Vector3.Dot(forwardVector, directionToTarget);
            float crossProduct = Vector3.Cross(forwardVector, directionToTarget).Y;

            // Ajustar el ángulo de dirección basado en los productos
            float steeringSum = Math.Clamp(crossProduct, -1f, 1f);

            // Agregar un rango de ángulo para el movimiento hacia adelante
            float angleThreshold = 0.7f; // Rango de ángulo para mover hacia adelante


            // Si el auto está dentro del rango angular para moverse hacia adelante, mueve el auto
            float targetSpeedFraction = (dotProduct > angleThreshold) ? 10f : (dotProduct < -angleThreshold) ? -5f : 0f;

            simpleCarController.Update(simulation, 1 / 60f, steeringSum, targetSpeedFraction, false);
            carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carPosition = carBodyReference.Pose.Position;
            rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation) * Matrix.CreateRotationY(MathHelper.ToRadians(90));
            carWorld = rotationMatrix * Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(carPosition);
            ColisionCaja = new BoundingBox(carBodyReference.BoundingBox.Min, carBodyReference.BoundingBox.Max);
            CarSpeed = carBodyReference.Velocity.Linear;
            velocity = carBodyReference.Velocity;
        }
        protected override void CargarModelo(ContentManager content)
        {
            this.CarModel = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");



            foreach (ModelMesh mesh in CarModel.Meshes)
            {
                Console.WriteLine(mesh.Name);
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
        }

        public override void Draw(GameTime gametime, Matrix View, Matrix Projection)
        {
            throw new NotImplementedException();
        }
    }



}
