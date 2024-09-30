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
        private float wheelRotationAngle;
        private float wheelSteeringAngle;
        private float maxWheelSteer = 0.7f;

        private float wheelSteerDelta = 0.2f;
            
      
        private float carSpeed = 0f;
        private float carVerticalSpeed = 0f;
        private const float carAcceleration = 50f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 500f;
        private const float carSpeedMin = -500f;
        private const float carJumpSpeed = 50f;
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.4f;
        private float angle = 0f;


        public Jugador(ContentManager content)
        {
            carPosition = new Vector3(0f, 0f, 0f);
            direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            var texture = content.Load<Texture2D>(ContentFolder3D + "autos/RacingCarA/TEX"); // Asegúrate de usar la ruta correcta
            effectAuto.Parameters["ModelTexture"].SetValue(texture);

            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectAuto;
                }
            }
        }

        public Matrix Update(GameTime gameTime, Matrix carWorld)
        {

            var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            {
                carSpeed = Math.Min(carSpeed + carAcceleration, carSpeedMax);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                carSpeed = Math.Max(carSpeed - carAcceleration, carSpeedMin);
            }
            carSpeed = carSpeed * 0.97f;
            if (Math.Abs(carSpeed) < 10) {
                carSpeed = 0;
            }
            carPosition = carPosition + (direccionFrontal * elapsedTime * carSpeed);
            if (carPosition.Y <= 0f & keyboardState.IsKeyDown(Keys.Space))
            {
                carVerticalSpeed = carJumpSpeed;
                carPosition += Vector3.Up * carVerticalSpeed;
            }
            else if (carPosition.Y > 0f)
            {
                carVerticalSpeed -= gravity * elapsedTime;
                carPosition += Vector3.Up * carVerticalSpeed;
            }

            // #region TODO Rotacion
            // Cuando multiplicamos la rotación con la Matrix el auto desaparece. Buscar Causa. Lo dejo así para que el problema sea más visible xd 

            
            if (keyboardState.IsKeyDown(Keys.A))
            {
                if (carSpeed != 0) {
                    angle += carSpinSpeed * elapsedTime;
                    carRotation = Matrix.CreateFromQuaternion(new Quaternion(0, MathF.Sin(angle * 0.5f), 0, MathF.Cos(angle * 0.5f)));
                    direccionFrontal = Vector3.Normalize(new Vector3
                    {
                        X = MathF.Sin(angle),
                        Y = 0,
                        Z = MathF.Cos(angle)
                    }); 
                }
                wheelSteeringAngle = Math.Min(wheelSteeringAngle + wheelSteerDelta, maxWheelSteer);
                
            } else if (keyboardState.IsKeyDown(Keys.D))
            {
                if (carSpeed != 0) {
                    angle -= carSpinSpeed * elapsedTime;
                    carRotation = Matrix.CreateFromQuaternion(new Quaternion(0, MathF.Sin(angle * 0.5f), 0, MathF.Cos(angle * 0.5f)));

                    direccionFrontal = Vector3.Normalize(new Vector3
                    {
                        X = MathF.Sin(angle),
                        Y = 0,
                        Z = MathF.Cos(angle)
                    });
                }
                wheelSteeringAngle = Math.Max(wheelSteeringAngle - wheelSteerDelta, -maxWheelSteer);
            } else {
                wheelSteeringAngle = 0;
            }

            float wheelRotationDelta = carSpeed * 0.0005f; // Ajusta este factor para que el giro sea proporcional.
            wheelRotationAngle += wheelRotationDelta;
            
           

            // #endregion

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
            return carWorld;
        }

        public void Draw(Matrix CarWorld, Matrix View, Matrix Projection)
        {
            var random = new Random(Seed: 0);
            var color = new Vector3(1.0f, 0.0f, 0.0f);
            effectAuto.Parameters["View"].SetValue(View);
            effectAuto.Parameters["Projection"].SetValue(Projection);


            foreach (ModelMesh mesh in Model.Meshes)
            {
                Matrix world = mesh.ParentBone.Transform * CarWorld;

                // Verifica si la mesh es una de las ruedas y aplica una rotación adicional
                if (mesh.Name == "WheelA" || mesh.Name == "WheelB" || mesh.Name == "WheelC" || mesh.Name == "WheelD")
                {
                    // Crear una matriz de rotación alrededor del eje X (giro de la rueda)
                    Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationAngle);
                    if (mesh.Name == "WheelA" || mesh.Name == "WheelB") {
                        wheelRotation = wheelRotation * Matrix.CreateRotationY(wheelSteeringAngle);
                    }

                    // Aplicar la rotación de la rueda al mundo
                    world = wheelRotation * world;
                }

                effectAuto.Parameters["World"].SetValue(world);

                mesh.Draw();
            }
        }
    }
}
