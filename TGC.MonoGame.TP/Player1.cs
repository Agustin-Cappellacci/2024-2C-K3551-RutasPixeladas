using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGC.MonoGame.TP.Content.Models
{
    
    
    class Jugador{
            public const string ContentFolder3D = "Models/";
            public const string ContentFolderEffects = "Effects/";
            private Model autoJugador {set; get;}
            private Vector3 PosicionInicial {set; get;}
            private FollowCamera FollowCamera { get; set; } 
            private Vector3 direccionFrontal { get; set; }

            public Matrix carWorld {get; set;}
            private Vector3 carPosition { get; set; }
            private Matrix carRotation { get; set; }
            private float carSpeed = 0f;
            private float carVerticalSpeed = 0f;
            private const float carAcceleration = 500f;
        //    private const float carAcceleratioSpeedMax = 0;
        //    private const float carAcceleratioSpeedMin = 0;
            private const float carSpeedMax = 1000f;
            private const float carSpeedMin = -400f;
            private const float carJumpSpeed = 50f;
            private const float gravity = 98f;
            private const float carSpinSpeed = 0.04f;
            private Effect effectoAuto {get; set;}
            


        public Jugador(ContentManager content){
            PosicionInicial = new Vector3(0f,0f,0f);
            direccionFrontal = Vector3.Forward;
            autoJugador = content.Load<Model>(ContentFolder3D+"autos/RacingCarA/RacingCar");
            effectoAuto = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
        }
        public void Update(GameTime gameTime){
            /*Acá empieza lo mio*/
                var elapsedTime = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
                // Caputo el estado del teclado.
                var keyboardState = Keyboard.GetState();

                // La logica debe ir aca.
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    carSpeed = Math.Min(carSpeed + carAcceleration, carSpeedMax);
                    carPosition = carPosition + (direccionFrontal * 500 * elapsedTime * carSpeed)  ;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    carSpeed = Math.Max(carSpeed - carAcceleration, carSpeedMin);
                    carPosition = carPosition + (-direccionFrontal * 500 * elapsedTime * carSpeed) ;
                }
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
                if (keyboardState.IsKeyDown(Keys.LeftControl)) // Habilidades especiales
                {
                    
                }
                if(carPosition.Y <= 0f & keyboardState.IsKeyDown(Keys.Space)){
                    carVerticalSpeed = carJumpSpeed;
                    carPosition += Vector3.Up * carVerticalSpeed ;
                } else if (carPosition.Y > 0f){
                    carVerticalSpeed -= gravity * elapsedTime;
                    carPosition += Vector3.Up * carVerticalSpeed ;
                }
            
                var random = new Random(Seed:0);
                var scala = 0.1f + (0.1f - 0.05f) * random.NextSingle();

                carWorld = Matrix.CreateScale(scala) * carRotation * Matrix.CreateTranslation(carPosition);
                FollowCamera.Update(gameTime, carWorld);

                
            
            // Assign the mesh effect
            // A model contains a collection of meshes
                foreach (var mesh in autoJugador.Meshes)
                {
                    // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = effectoAuto;
                    }
                }
        }

        public void Draw(Matrix view, Matrix Projection){
            var random = new Random(Seed:0);
            var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());  
            foreach(var mesh in autoJugador.Meshes){
                effectoAuto.Parameters["DiffuseColor"].SetValue(color);
                effectoAuto.Parameters["World"].SetValue(carWorld);
                effectoAuto.Parameters["View"].SetValue(FollowCamera.View);
                effectoAuto.Parameters["Projection"].SetValue(FollowCamera.Projection);
            }
        }
    }
}