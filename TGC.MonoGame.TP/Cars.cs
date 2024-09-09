using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGC.MonoGame.TP.Content.Models
{
    /// <summary>
    /// A Car Model to be drawn
    /// </summary>
    class Cars
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        private Model CarModel { get; set; }
        private Effect EffectCar { get; set; }


        // <summary>
        /// Creates a Car Model with a content manager to load resources.
        /// </summary>
        /// <param name="content">The Content Manager to load resources</param>
        public Cars (ContentManager content)
        {
            // Load the Car Model
            CarModel = content.Load<Model>(ContentFolder3D+"RacingCarA/RacingCar");

            // Load an effect that will be used to draw the scene
            EffectCar = content.Load<Effect>(ContentFolderEffects + "DiffuseColor");
            
            // Assign the mesh effect
            // A model contains a collection of meshes
           foreach (var mesh in CarModel.Meshes)
            {
                // Un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = EffectCar;
                }
            }

        }

        // <summary>
        /// Draws the City Scene
        /// </summary>
        /// <param name="gameTime">The Game Time for this frame</param>
        /// <param name="view">A view matrix, generally from a camera</param>
        /// <param name="projection">A projection matrix</param>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            EffectCar.Parameters["View"].SetValue(view);
            EffectCar.Parameters["Projection"].SetValue(projection);
            
            var random = new Random(Seed:0);
            
            
            for (int i = 0; i < 20; i++){
                
                var scala = random.NextSingle() * random.NextSingle();
                // var colorcito = new Vector3((CameraPosition.X) + random.NextSingle(), CameraPosition.Y + random.NextSingle(), CameraPosition.Z + random.NextSingle());
                var color = new Vector3(random.NextSingle(), random.NextSingle(), random.NextSingle());         /*Usamos verto3 porque es BasicEffect. Se usa vector4 si tenemos activado el AlphaShader*/

                var traslacion = new Vector3(
                -1000f + (100f - (-1000f)) * random.NextSingle(),
                0,
                -2000f + (1500f - (-2000f)) * random.NextSingle()
                );

                //SCALA DE 0.5 A 1
                //-770 A 650
                //-1200 A 1000

                foreach (var mesh in CarModel.Meshes)
                {
                    EffectCar.Parameters["DiffuseColor"].SetValue(color);
                    EffectCar.Parameters["World"].SetValue(mesh.ParentBone.Transform * Matrix.CreateScale(scala) * Matrix.CreateTranslation(traslacion));

                    mesh.Draw();
                }
            }
        }
    }
}
