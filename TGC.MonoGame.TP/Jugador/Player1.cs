using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private float CarSpeed;
        private const float CarMaxSpeed = 1000f;
        private const float CarAcceleration = 200f;
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
        
        
        
        private List<ModelMesh> ruedas;
        private List<ModelMesh> restoAuto;

        public Jugador(ContentManager content)
        {
            carPosition = new Vector3(0f, 0f, 0f);
            direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

            rotacionRuedasDelanterasY = 0;

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
                } else restoAuto.Add(mesh); 
                
            }
        }

        public void Update(GameTime gameTime)
        {

            // Caputo el estado del teclado.
            var keyboardState = Keyboard.GetState();


            // La logica debe ir aca.

            elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            /*
            // logica para el salto, solo si aprieto espacio Y el auto esta en el suelo
            if (keyboardState.IsKeyDown(Keys.Space) & CarPosition.Y == 0f)
            {
                CarVerticalVelocity = CarjumpStrength;
            }
            // caida
            if (CarPosition.Y > 0f || CarVerticalVelocity > 0f)
            {
                CarVerticalVelocity -= gravity * elapsedTime;
                CarPosition += Vector3.UnitY * CarVerticalVelocity * elapsedTime;
                // Si se pasa de la posicion en y (a negativa) corrijo la posicion en y 
                if (CarPosition.Y < 0f)
                {
                    CarPosition = new Vector3(CarPosition.X, 0f, CarPosition.Z);
                    CarVerticalVelocity = 0;
                }
            }

            // Movimiento hacia adelante
            if (keyboardState.IsKeyDown(Keys.W))
            {
                CarSpeed += CarAcceleration * elapsedTime;
                Console.WriteLine(CarSpeed);
                if (CarSpeed < CarMaxSpeed)
                {
                    CarSpeed = CarMaxSpeed; // Limita la velocidad máxima
                }
            }
            // Movimiento hacia atrás
            if (keyboardState.IsKeyDown(Keys.S))
            {
                // si la velocidad es negativa (en realdidad ahi esta yendo hacia adelante)
                if (CarSpeed < 0)
                {
                    // aumento la fuerza de frenado
                    CarSpeed -= CarBrakeForce * elapsedTime;
                }
                else
                {
                    // si ya esta yendo en reversa, sigue con la aceleracion pero en reversa
                    CarSpeed -= CarAcceleration * elapsedTime;
                }

                // limito la velocidad maxima en reversa
                if (CarSpeed > -CarMaxSpeed / 2)
                {
                    CarSpeed = -CarMaxSpeed / 2;
                }

            }


            // Si no se presiona ninguna tecla, desacelera
            if (CarSpeed > 0)
            {
                CarSpeed -= CarDeceleration * elapsedTime;
                if (CarSpeed < 0)
                {
                    CarSpeed = 0; // Evita que se vuelva negativa la velocidad
                }
            }
            else if (CarSpeed < 0)
            {
                CarSpeed += CarDeceleration * elapsedTime;
                if (CarSpeed > 0)
                {
                    CarSpeed = 0; // Evita que se vuelva positiva la velocidad
                }
            }


            // Aplico la velocidad al movimiento del auto
            CarPosition += Vector3.Transform(Vector3.UnitZ * CarSpeed * elapsedTime, Matrix.CreateRotationY(CarRotationY));
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (CarSpeed != 0)
                    CarRotationY += 2f * elapsedTime;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                if (CarSpeed != 0)
                    CarRotationY -= 2f * elapsedTime;

            }

            carWorld = Matrix.CreateRotationY(CarRotationY) * Matrix.CreateTranslation(CarPosition.X, CarPosition.Y, CarPosition.Z);
            return carWorld;
            */

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
    Matrix.CreateTranslation(-distanciaEje, 0, distanciaTrasera) *  // 2. Coloca la rueda trasera izquierda en su lugar
    Matrix.CreateRotationX(ruedaGiroVelocidad) *  // 1. Rota la rueda sobre su eje X
    Matrix.CreateRotationY(CarRotationY) *  // 3. Aplica la rotación del coche completo
    Matrix.CreateTranslation(CarPosition);  // 4. Traslada el coche completo a su posición en el mundo

// Rueda trasera derecha
ruedaTraseraDerTransform = 
    Matrix.CreateRotationX(ruedaGiroVelocidad) *  // 1. Rota la rueda sobre su eje X
    Matrix.CreateTranslation(distanciaEje, 0, distanciaTrasera) *  // 2. Coloca la rueda trasera derecha en su lugar
    Matrix.CreateRotationY(CarRotationY) *  // 3. Aplica la rotación del coche completo
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
    CarPosition += Vector3.Transform(Vector3.UnitZ * CarSpeed * elapsedTime, Matrix.CreateRotationY(CarRotationY));

    // Actualizar la matriz del mundo del coche
    carWorld = Matrix.CreateRotationY(CarRotationY) * Matrix.CreateTranslation(CarPosition);
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
        effectAuto.Parameters["DiffuseColor"].SetValue(color);
        effectAuto.Parameters["World"].SetValue(mesh.ParentBone.Transform * carWorld);
        mesh.Draw();
    }

    // Dibujar cada rueda con su respectiva transformación
    foreach (ModelMesh rueda in ruedas)
    {
        effectAuto.Parameters["DiffuseColor"].SetValue(colorRueda);

        if (rueda.Name.Contains("WheelA"))
        {
            // Aplicar transformación de la rueda delantera izquierda
            effectAuto.Parameters["World"].SetValue(rueda.ParentBone.Transform * ruedaDelanteraIzqTransform);
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
