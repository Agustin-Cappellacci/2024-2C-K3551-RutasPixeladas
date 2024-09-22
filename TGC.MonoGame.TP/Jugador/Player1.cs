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
    class Jugador
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        private Model Model { get; set; }
        private Effect effectAuto { get; set; }

        // Jugabilidad
        private Vector3 direccionFrontal { get; set; }
        private Vector3 carPosition { get; set; }
        private Matrix carRotation { get; set; }
        private float carSpeed = 0f;
        private float carVerticalSpeed = 0f;
        private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
        private const float carSpeedMax = 1000f;
        private const float carSpeedMin = -700f;
        private const float carJumpSpeed = 50f;
        private const float gravity = 98f;
        private const float carSpinSpeed = 0.04f;


        public Jugador(ContentManager content)
        {
            carPosition = new Vector3(0f, 0f, 0f);
            direccionFrontal = Vector3.Forward;
            Model = content.Load<Model>(ContentFolder3D + "autos/RacingCarA/RacingCar");
            //effectAuto = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            effectAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");

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
                carPosition = carPosition + (direccionFrontal * elapsedTime * carSpeed);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                carSpeed = Math.Max(carSpeed - carAcceleration, carSpeedMin);
                carPosition = carPosition + (direccionFrontal * elapsedTime * carSpeed);
            }
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

            /*
            if (keyboardState.IsKeyDown(Keys.A))
            {
                carRotation *= Matrix.CreateRotationY(-carSpinSpeed);
                direccionFrontal = Vector3.Transform(Vector3.Forward, carRotation * elapsedTime);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                carRotation *= Matrix.CreateRotationY(carSpinSpeed);
                direccionFrontal = Vector3.Transform(Vector3.Forward, carRotation * elapsedTime);
            }
            
            */

            // #endregion

            var random = new Random(Seed: 0);
            var scale = 1f + (0.1f - 0.05f) * random.NextSingle();
            carWorld = Matrix.CreateScale(scale) * Matrix.CreateTranslation(carPosition);

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
    }
}
