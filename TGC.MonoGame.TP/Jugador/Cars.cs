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

    public abstract class AutoEnemigo {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        private Model CarModel { get; set; }
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
        SimpleCarController playerControllers;

        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        private GraphicsDevice graphicsDevice;

        private IPowerUp powerUp;

        private float vida = 200;





        // Constructor abstracto
        protected AutoEnemigo(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice,/* SimpleCarController playerController,*/ Vector3 posicion, float angulo) {
            
            carWorld = Matrix.Identity;
            var random = new Random();  // No hace falta un seed porque se usa una sola vez y se guarda en una variable.
            
            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();

            effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            var texture = content.Load<Texture2D>(ContentFolder3D + "autos/RacingCarA/TEX"); // Asegúrate de usar la ruta correcta
            effectAuto.Parameters["ModelTexture"].SetValue(texture);

            /*EffectCar2 = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            texture = content.Load<Texture2D>(ContentFolder3D + "autos/CombatVehicle/TEX"); // Asegúrate de usar la ruta correcta
            EffectCar2.Parameters["ModelTexture"].SetValue(texture);
        */  
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

        protected abstract void CargarModelo(ContentManager content);

        protected abstract void Update(GameTime gameTime, Simulation simulation);
        public void Draw(GameTime gametime, Matrix View, Matrix Projection) {
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

        }

        public static System.Numerics.Vector3 PositionToNumerics(Microsoft.Xna.Framework.Vector3 xnaVector3)
        {
            return new System.Numerics.Vector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
        }
    }

    class AutoEnemigoCombate : AutoEnemigo {
        private Matrix rotationMatrix;
        private System.Numerics.Vector3 carPosition { get; set; }
        private BodyHandle carBodyHandle;
        private Effect effectAuto { get; set; }
        private Model CarModel { get; set; }
        float Escala;
        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        SimpleCarController playerController;
        public AutoEnemigoCombate(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice, SimpleCarController playerController, Vector3 posicion, float angulo)
            : base(content, simulation, graphicsDevice, posicion, angulo + (float)Math.PI / 2) //Ajustar ángulo si es necesario
        {   
            this.playerController = playerController;

            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();

            CargarModelo(content);
            Escala = 0.004f + (0.004f - 0.001f) * new Random().NextSingle();

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

        protected override void Update(GameTime gameTime, Simulation simulation){
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            // Obtener la referencia del cuerpo del auto en la simulación
            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carBodyReference.Awake = true;
            
            playerController.Update(simulation, 1 / 60f, 0, 0, false);
            carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carPosition = carBodyReference.Pose.Position;
            rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation);
            carWorld = rotationMatrix * Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(carPosition - new System.Numerics.Vector3(0,15f,0));
            
        }

        protected override void CargarModelo(ContentManager content) {
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
    }

    class AutoEnemigoCarrera : AutoEnemigo {
        private Matrix rotationMatrix;
        private System.Numerics.Vector3 carPosition { get; set; }
        private BodyHandle carBodyHandle;        
        private Effect effectAuto { get; set; }
        private Model CarModel { get; set; }
        float Escala;
        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;
        SimpleCarController playerController;
        public AutoEnemigoCarrera(ContentManager content, Simulation simulation, GraphicsDevice graphicsDevice, SimpleCarController playerController, Vector3 posicion, float angulo)
            : base(content, simulation, graphicsDevice, posicion, angulo) // Ajustar ángulo si es necesario
        {   

            this.playerController = playerController;
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            ruedas = new List<ModelMesh>();
            restoAuto = new List<ModelMesh>();

            CargarModelo(content);
            Escala = 0.1f + (0.1f - 0.05f) * new Random().NextSingle();
        }
        
        protected override void Update(GameTime gameTime, Simulation simulation){
            
            float elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            var carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carBodyReference.Awake = true;

            playerController.Update(simulation, 1 / 60f, 0, 0, false);
            carBodyReference = simulation.Bodies.GetBodyReference(carBodyHandle);
            carPosition = carBodyReference.Pose.Position;
            rotationMatrix = Matrix.CreateFromQuaternion(carBodyReference.Pose.Orientation);
            carWorld = rotationMatrix * Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(carPosition);
        }
        protected override void CargarModelo(ContentManager content) {
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
                } else restoAuto.Add(mesh);

            }
        }
    }

    

}
