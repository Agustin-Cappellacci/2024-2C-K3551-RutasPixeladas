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

        // Modelos y Efectos
        private Model Model { get; set; }
        private Effect effectAuto { get; set; }

        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;

        // Jugabilidad
        private Microsoft.Xna.Framework.Vector3 direccionFrontal { get; set; }
        private Matrix carRotation = Matrix.CreateRotationY(0f);

        public Matrix carWorld { get; set; }
        private float carVerticalSpeed = 0f;
        private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 1000f;
        private const float carSpeedMin = -700f;
        private const float carJumpSpeed = 50f;
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.4f;

        private float rotacionRuedasDelanterasY {get; set;}
        private Vector3 CarPosition { get; set; }
        private float CarSpeed = 0f;

        private float angle = 0f;

        private System.Numerics.Vector3 carPosition { get; set; }

        private const float CarMaxSpeed = 1000f;
        private const float CarAcceleration = 500f;
        private const float CarBrakeForce = 800f;
        private float CarDeceleration = 500f;

        private float CarRotationY;
        private float CarVerticalVelocity;
        private const float CarjumpStrength = 300f;
        private float elapsedTime;

        private Matrix ruedaDelanteraIzqTransform {get; set;}
        private Matrix ruedaDelanteraDerTransform {get; set;}
        private Matrix ruedaTraseraIzqTransform {get; set;}
        private Matrix ruedaTraseraDerTransform {get; set;}
        
        
        
        private Matrix ruedaDelanteraTransform;

        private Matrix ruedaTraseraTransform;
        private Simulation simulation;

        private BodyHandle carBodyHandle;
        private BodyHandle wheelBodyHandle;



        public Jugador(ContentManager content, Simulation simulation)
        {
            carPosition = new System.Numerics.Vector3(0f, 10f, 0f);
            direccionFrontal = Microsoft.Xna.Framework.Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            var texture = content.Load<Texture2D>(ContentFolder3D + "autos/RacingCarA/TEX"); // Asegúrate de usar la ruta correcta
            effectAuto.Parameters["ModelTexture"].SetValue(texture);

            rotacionRuedasDelanterasY = 0;
            CarSpeed = 0f;
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
                if (CarSpeed > 0f)
                {
                    CarSpeed -= CarDeceleration * elapsedTime;
                }
                else if (CarSpeed < 0f)
                {
                    CarSpeed += CarDeceleration * elapsedTime; // Sumar para desacelerar en reversa
                }

                if (Math.Abs(CarSpeed) < 5f)        // Hay que probar varios valores. 0.1f no funciona bien, es casi imposible hacer que se detenga el auto 
                {
                    CarSpeed = 0f; // Detener el auto cuando está casi en reposo
                }
            }
            // Actualizar la velocidad del cuerpo en la simulación en base a la dirección
            var forwardDirection = System.Numerics.Vector3.Transform(System.Numerics.Vector3.UnitZ,
       System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, CarRotationY));
            carBodyReference.Velocity.Linear = forwardDirection * CarSpeed;


            // Almacenar las transformaciones de las ruedas delanteras y traseras
                                    
            if (keyboardState.IsKeyDown(Keys.A))
            {
                CarRotationY += carSpinSpeed * elapsedTime;  // Girar el coche a la izquierda
                rotacionRuedasDelanterasY = MathHelper.Clamp(rotacionRuedasDelanterasY + 1f, -0.05f, 0.05f); // Ajustar rotación de las ruedas delanteras
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                CarRotationY -= carSpinSpeed * elapsedTime;  // Girar el coche a la derecha
                rotacionRuedasDelanterasY = MathHelper.Clamp(rotacionRuedasDelanterasY - 1f, -0.05f, 0.05f); // Ajustar rotación de las ruedas delanteras

            }
            else
            {
                rotacionRuedasDelanterasY = 0;  // Las ruedas vuelven a su rotación original (cuando no se gira)
            }
        
            float radioRueda = 0.5f; // Ajusta el radio de la rueda
            float ruedaGiroVelocidad =  (CarSpeed * elapsedTime) / radioRueda;

            float distanciaEje = 0f; // Distancia lateral entre las ruedas (izquierda y derecha)
            float distanciaDelantera = 0f; // Distancia del centro del coche a las ruedas delanteras (eje Z)
            float distanciaTrasera = 0f; // Distancia del centro del coche a las ruedas traseras (eje Z)

            ruedaTraseraIzqTransform = 
            Matrix.CreateRotationX(ruedaGiroVelocidad) *  // 1. Rota la rueda sobre su eje X
            Matrix.CreateTranslation(-distanciaEje, 0, distanciaTrasera) *  // 2. Coloca la rueda trasera izquierda en su lugar
            Matrix.CreateRotationY(CarRotationY) *  // 3. Aplica la rotación del coche completo
            Matrix.CreateTranslation(CarPosition);  // 4. Traslada el coche completo a su posición en el mundo

                    // Rueda trasera derecha
            ruedaTraseraDerTransform =
            Matrix.CreateRotationX(ruedaGiroVelocidad) * // 1. Rota la rueda sobre su eje X
            Matrix.CreateTranslation(distanciaEje, 0, distanciaTrasera)*  // 2. Coloca la rueda trasera derecha en su lugar
            Matrix.CreateRotationY(CarRotationY)*  // 3. Aplica la rotación del coche completo
            Matrix.CreateTranslation(CarPosition);  // 4. Traslada el coche completo a su posición en el mundo

            // Rueda delantera izquierda
            ruedaDelanteraIzqTransform = 
            Matrix.CreateRotationX(ruedaGiroVelocidad) *  // 1. Rota la rueda sobre su eje X
            Matrix.CreateRotationY(rotacionRuedasDelanterasY) *  // 2. Aplica la rotación de la rueda delantera cuando gira el coche
            Matrix.CreateTranslation(-distanciaEje, 0, distanciaDelantera) *  // 3. Coloca la rueda delantera izquierda en su lugar
            Matrix.CreateRotationY(CarRotationY) *  // 4. Aplica la rotación del coche completo
            Matrix.CreateTranslation(CarPosition);  // 5. Traslada el coche completo a su posición en el mundo

            // Rueda delantera derecha
            ruedaDelanteraDerTransform = 
            Matrix.CreateRotationX(ruedaGiroVelocidad) *  // 1. Rota la rueda sobre su eje X
            Matrix.CreateRotationY(rotacionRuedasDelanterasY) *  // 2. Aplica la rotación de la rueda delantera cuando gira el coche
            Matrix.CreateTranslation(distanciaEje, 0, distanciaDelantera) *  // 3. Coloca la rueda delantera derecha en su lugar
            Matrix.CreateRotationY(CarRotationY) *  // 4. Aplica la rotación del coche completo
            Matrix.CreateTranslation(CarPosition); 

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
    var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
    var colorRueda = new Vector3(0, 0, 0);

    effectAuto.Parameters["View"].SetValue(View);
    effectAuto.Parameters["Projection"].SetValue(Projection);

    // Dibujar el cuerpo del coche
    foreach (ModelMesh mesh in restoAuto)
    {
        //effectAuto.Parameters["DiffuseColor"].SetValue(color);
        effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
        mesh.Draw();
    }

    // Dibujar cada rueda con su respectiva transformació
    foreach (ModelMesh rueda in ruedas)
    {
        //effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);
      
        if (rueda.Name.Contains("WheelA"))
        {
            // HEAD Aplicar transformación de la rueda delantera izquierda
            
            effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * ruedaDelanteraIzqTransform);
/*
            var random = new Random(Seed: 0);
            var color = new Microsoft.Xna.Framework.Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());
            var colorRueda = new Microsoft.Xna.Framework.Vector3(0, 0, 0);
            effectAuto.Parameters["View"].SetValue(View);
            effectAuto.Parameters["Projection"].SetValue(Projection);


            foreach (ModelMesh mesh in Model.Meshes)
            {
                //effectAuto.Parameters["DiffuseColor"].SetValue(color);
                effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * CarWorld);



                mesh.Draw();
            }

*/


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
        else if (rueda.Name.Contains("WheelB"))
        {
            // Aplicar transformación de la rueda delantera derecha
            effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * ruedaDelanteraDerTransform);
        }
        else if (rueda.Name.Contains("WheelC"))
        {
            // Aplicar transformación de la rueda trasera izquierda
            effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * ruedaTraseraIzqTransform);
        }
        else if (rueda.Name.Contains("WheelD"))
        {
            // Aplicar transformación de la rueda trasera derecha
            effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * ruedaTraseraDerTransform);
        }
        
        rueda.Draw();
    }
}
    }
}
